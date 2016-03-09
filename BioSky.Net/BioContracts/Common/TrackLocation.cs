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
      StopCaptureDevice();

      if (_location.CaptureDevices == null || _location.CaptureDevices.Count == 0)
        return;
            
      CaptureDevice newCaptureDevice = _location.CaptureDevices.FirstOrDefault();

      if (newCaptureDevice.Devicename == CaptureDeviceObserver.DeviceName)
        return;
           
      CaptureDeviceObserver.Update(newCaptureDevice.Devicename);
      CaptureDeviceObserver.Subscribe(OnFrameChanged);         
    }

    private async void OnVisitorVerified(Photo photo, Person person)
    {
      try
      {
        _veryfier.VerificationDone -= OnVisitorVerified;

        bool status = (photo != null && person != null && photo.Personid == person.Id);
        _visitor.Personid = status ? person.Id : 0;

        if (photo != null)        
          _visitor.Personid = photo.Personid;       
        
        _visitor.Status = status ? Result.Success : Result.Failed;

        await _bioService.VisitorDataClient.Add(_visitor);
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

      if (_location.CaptureDevices == null || _location.CaptureDevices.Count == 0)
        return;

      AccessDevice newAccessDevice = _location.AccessDevices.FirstOrDefault();

      if (newAccessDevice.Portname == AccessDeviceObserver.DeviceName)
        return;
      
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

      _visitor = null;

      Person person = _database.Persons.GetPersonByCardNumber(cardNumber);

      _visitor = new Visitor() { Locationid   = _location.Id                                      
                               , Time         = DateTime.Now.Ticks                                           
                               , CardNumber   = cardNumber   };


      if (person != null)      
        _visitor.Personid = person.Id;            

      _veryfier.VerificationDone -= OnVisitorVerified;
      _veryfier.VerificationDone += OnVisitorVerified; 
      _veryfier.Start(CaptureDeviceObserver.DeviceName, person);     
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

    private Visitor _visitor  ;
    
    private readonly IDatabaseService     _bioService         ;
    private readonly IBioSkyNetRepository _database           ;
    private readonly IProcessorLocator    _locator            ;
    private readonly INotifier            _notifier           ;

    #endregion
  }
}
