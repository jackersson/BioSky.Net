using BioContracts.Common;
using BioContracts.Services;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using AForge.Video.DirectShow;

namespace BioContracts
{
  public interface ILocationDeviceObserver
  {
    void Stop();
    void Start(Location location);
    bool IsDeviceOk { get; }
  }

  public class LocationAccessDeviceObserver : ILocationDeviceObserver, IAccessDeviceObserver
  {
    public LocationAccessDeviceObserver(IProcessorLocator locator)
    {
      _accessDeviceEngine = locator.GetProcessor<IAccessDeviceEngine>();
    }
    public void Stop()
    {
      _accessDeviceEngine.Unsubscribe(this);
      if (!string.IsNullOrEmpty(_deviceName))
        _accessDeviceEngine.Remove(_deviceName);
    }

    public void Start(string deviceName)
    {
      if (_deviceName == deviceName && _accessDeviceEngine.IsDeviceActive(deviceName))
        return;

      if (string.IsNullOrEmpty(_deviceName))
        return;

      _deviceName = deviceName;

      _accessDeviceEngine.Add(_deviceName);
      _accessDeviceEngine.Subscribe(this, _deviceName);
    }

    public void Start(Location location)
    {
      _location = location;

      if (location == null)
        return;

      if (location.AccessDevice != null)
        Start(location.AccessDevice.Portname);
    }

    public void OnCardDetected(string cardNumber) { }

    public void OnError(Exception ex) { IsDeviceOk = false; }

    public void OnReady(bool isReady) { IsDeviceOk = isReady; }

    private bool _isDeviceOk;
    public bool IsDeviceOk
    {
      get { return _isDeviceOk; }
      set
      {
        if (_isDeviceOk != value)
          _isDeviceOk = value;
      }
    }

    private string   _deviceName;
    private Location _location  ;
    private readonly IAccessDeviceEngine _accessDeviceEngine;
  }

  public class LocationCaptureDeviceObserver : ILocationDeviceObserver,  ICaptureDeviceObserver
  {
    public LocationCaptureDeviceObserver(IProcessorLocator locator)
    {
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
    }

    public void Stop()
    {
      _captureDeviceEngine.Unsubscribe(this);
      if (!string.IsNullOrEmpty(_deviceName))
        _captureDeviceEngine.Remove(_deviceName);
    }

    public void Start(string deviceName)
    {
      if (_deviceName == deviceName && _captureDeviceEngine.IsDeviceActive(deviceName))
        return;

      if (string.IsNullOrEmpty(_deviceName))
        return;
            
      _deviceName = deviceName;

      _captureDeviceEngine.Add(_deviceName);
      _captureDeviceEngine.Subscribe(this, _deviceName);
    }

    public void Start(Location location)
    {
      _location = location;

      if (location == null)
        return;

      if (location.CaptureDevice != null)
        Start(location.CaptureDevice.Devicename);
    }

    public void OnFrame(ref Bitmap frame) { }

    public void OnStop(bool stopped, string message) { IsDeviceOk = !stopped; }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) { IsDeviceOk = started; }
    
    private bool _isDeviceOk;
    public bool IsDeviceOk
    {
      get { return _isDeviceOk; }
      set
      {
        if (_isDeviceOk != value)        
          _isDeviceOk = value;       
      }
    }

    private string   _deviceName;
    private Location _location  ;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
  }

  enum LocationDevice
  {
       Card
     , AccessDevice
     , CaptureDevice
     , FingerprintDevice
     , IrisDevice
  }

  public class TrackLocation : PropertyChangedBase
  {   
    public TrackLocation(IProcessorLocator locator, Location location)
    {
      _locator = locator;
     
      _database            = locator.GetProcessor<IBioSkyNetRepository>();     
      _bioService          = locator.GetProcessor<IServiceManager>().DatabaseService;
      _notifier            = locator.GetProcessor<INotifier>();
      _accessDeviceEngine  = locator.GetProcessor<IAccessDeviceEngine>();
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      
      //_veryfier = new Verifyer(_locator);

      _devices = new Dictionary<LocationDevice, ILocationDeviceObserver>();

       Update(location);
    }

    public void Update(Location location)
    {
      if (location == null)
        return;
      _previousLocation = _currentLocation;

      _currentLocation  = location;
      Start();
    }

    private void Start()
    {
      Init();

      foreach (KeyValuePair<LocationDevice, ILocationDeviceObserver> pair in _devices)
        pair.Value.Start(_currentLocation); 
    }

    private void Init()
    {
      if (_currentLocation == null)
        return;

      if (_currentLocation.AccessDevice != null)
      {
        if (!_devices.ContainsKey(LocationDevice.AccessDevice))
          _devices.Add(LocationDevice.AccessDevice, new LocationAccessDeviceObserver(_locator));         
      }

      if (_currentLocation.CaptureDevice != null)
      {
        if (!_devices.ContainsKey(LocationDevice.AccessDevice))
          _devices.Add(LocationDevice.CaptureDevice, new LocationCaptureDeviceObserver(_locator));        
      }


    }
 /*
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
   */

    public void Stop()
    {
      foreach (KeyValuePair<LocationDevice, ILocationDeviceObserver> pair in _devices)
        pair.Value.Stop();
    }

    public IScreen ScreenViewModel { get; set; }

    public string Caption
    {
      get { return _currentLocation.LocationName; }
    }

    public Location CurrentLocation
    {
      get { return _currentLocation; }
    }

    /*
    public void OnCardDetected(string cardNumber)
    {      
      if (_veryfier.Busy)
        return;

      _visitor = null;

      Person person = _database.Persons.GetPersonByCardNumber(cardNumber);

      _visitor = new Visitor() { Locationid   = _currentLocation.Id                                      
                               , Time         = DateTime.Now.Ticks                                           
                               , CardNumber   = cardNumber   };


      if (person != null)      
        _visitor.Personid = person.Id;            

     // _veryfier.VerificationDone -= OnVisitorVerified;
     // _veryfier.VerificationDone += OnVisitorVerified; 

      //TODO check on NULL
     // _veryfier.Start(CurrentLocation.CaptureDevice.Devicename, person);     
    }  
    */
   
    #region UI   
   
    #endregion

    #region Global Variables  
   // private Verifyer _veryfier;
    private Location _currentLocation ;
    private Location _previousLocation;
    //private Visitor  _visitor  ;

    private Dictionary<LocationDevice, ILocationDeviceObserver> _devices;
    
    private readonly IDatabaseService     _bioService         ;
    private readonly IBioSkyNetRepository _database           ;
    private readonly IProcessorLocator    _locator            ;
    private readonly INotifier            _notifier           ;
    private readonly IAccessDeviceEngine  _accessDeviceEngine ;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    #endregion
  }
}
