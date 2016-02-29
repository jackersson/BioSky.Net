using BioContracts.Common;
using BioContracts.Services;
using BioService;
using Caliburn.Micro;
using System;
using System.Drawing;
using System.Linq;

namespace BioContracts
{
  public class TrackLocation : PropertyChangedBase
  {   
    public TrackLocation(IProcessorLocator locator, Location location)
    {
      _locator = locator;
     
      _database            = locator.GetProcessor<IBioSkyNetRepository>();     
      _bioService          = locator.GetProcessor<IServiceManager>().DatabaseService;
      _notifier            = locator.GetProcessor<INotifier>();

      CaptureDeviceObserver = new TrackLocationCaptureDeviceObserver(locator);
      AccessDeviceObserver  = new TrackLocationAccessDeviceObserver (locator);     

      _veryfier = new Verifyer(_locator);     

      Update(location);
    }

    public void Update(Location location)
    {
      if (location == null)
        return;

      _location = location;
      RefreshData();
    }

    private void RefreshData()
    {
      RefreshCaptureDevices();
      RefreshAccessDevices ();
    }

    private void RefreshCaptureDevices()
    {
      
      CaptureDevice newCaptureDevice = _database.CaptureDeviceHolder.Data
                                      .Where(cap => cap.Locationid == _location.Id)
                                      .FirstOrDefault();

      if (newCaptureDevice.Devicename == CaptureDeviceObserver.DeviceName)
        return;
      
      StopCaptureDevice();      
      CaptureDeviceObserver.Update(newCaptureDevice.Devicename);
      CaptureDeviceObserver.Subscribe(OnFrameChanged);       
    }

    private async void OnVisitorVerified(Visitor visitor)
    {
      try
      {
        _veryfier.VisitorVerified -= OnVisitorVerified;
        await _bioService.VisitorDataClient.Add(visitor);
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }
    }

    private void OnFrameChanged(object sender, ref Bitmap bitmap)
    {
      if (FrameChanged != null)      
        FrameChanged(sender,ref  bitmap);      
    }

    private void StopAccessDevice()
    {      
      AccessDeviceObserver.AccessDeviceState -= OnAccessDeviceStateChanged;
      AccessDeviceObserver.Stop();
    }

    private void StopCaptureDevice( )
    {
      CaptureDeviceObserver.Unsubscribe(OnFrameChanged);
      CaptureDeviceObserver.Stop();    
    }

    private void RefreshAccessDevices()
    {
      StopAccessDevice();

      AccessDevice newAccessDevice = _database.AccessDeviceHolder.Data
                                     .Where(ad => ad.Locationid == _location.Id).FirstOrDefault();

      if (newAccessDevice.Portname == AccessDeviceObserver.DeviceName)
        return;

      StopCaptureDevice();
      AccessDeviceObserver.Update(newAccessDevice.Portname);
      AccessDeviceObserver.AccessDeviceState += OnAccessDeviceStateChanged;
      AccessDeviceObserver.CardDetected      += OnCardDetected            ;          
    }

    private void OnAccessDeviceStateChanged(bool status )
    {
      AccessDevicesStatus = status;      
    }

    public void Stop()
    {
      StopAccessDevice();
      StopCaptureDevice();
    }

    public IScreen ScreenViewModel { get; set; }

    public string Caption
    {
      get { return _location.LocationName; }
    }

    public Location CurrentLocation
    {
      get { return _location; }
    }

    private void OnCardDetected(TrackLocationAccessDeviceObserver sender, string cardNumber)
    {      
      if (_veryfier.Busy)
        return;
      
      Card card = _database.CardHolder.GetValue(cardNumber);

      Visitor visitor  = new Visitor() { Locationid   = _location.Id
                                       , EntityState  = EntityState.Added
                                       , Time         = DateTime.Now.Ticks                                           
                                       , CardNumber   = cardNumber   };
      
      if (card != null)
      {        
        Person person = _database.PersonHolder.GetValue(card.Personid);
        if (person != null)        
          visitor.Personid = person.Id;         
      }

      _veryfier.VisitorVerified -= OnVisitorVerified;
      _veryfier.VisitorVerified += OnVisitorVerified; 
      _veryfier.Start(CaptureDeviceObserver.DeviceName, visitor);     
    }

    #region UI
    private TrackLocationCaptureDeviceObserver _captureDeviceObserver;
    public TrackLocationCaptureDeviceObserver CaptureDeviceObserver
    {
      get { return _captureDeviceObserver; }
      private set
      {
        if (_captureDeviceObserver != value)
          _captureDeviceObserver = value;
      }
    }

    private TrackLocationAccessDeviceObserver _accessDeviceObserver;
    public TrackLocationAccessDeviceObserver AccessDeviceObserver
    {
      get { return _accessDeviceObserver; }
      private set
      {
        if (_accessDeviceObserver != value)
          _accessDeviceObserver = value;
      }
    }


    private bool _accessDevicesStatus;
    public bool AccessDevicesStatus
    {
      get { return _accessDevicesStatus; }
      set
      {
        if (_accessDevicesStatus != value)
        {
          _accessDevicesStatus = value;
          NotifyOfPropertyChange(() => AccessDevicesStatus);    
        }
      }
    }

    #endregion

    #region Global Variables
    public event FrameEventHandler          FrameChanged;

    private Verifyer _veryfier;
    private Location _location;
    
    private readonly IDatabaseService     _bioService         ;
    private readonly IBioSkyNetRepository _database           ;
    private readonly IProcessorLocator    _locator            ;
    private readonly INotifier            _notifier           ;

    #endregion
  }
}
