using BioContracts.Common;
using BioContracts.Services;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using AForge.Video.DirectShow;
using BioContracts.CaptureDevices;
using BioContracts.Locations;
using BioContracts.Locations.BioTasks;
using BioContracts.BioTasks;
using BioContracts.Locations.Observers;
using BioContracts.AccessDevices;

namespace BioContracts
{
  
  public class TrackLocation : PropertyChangedBase, IBioObservable<IFullLocationObserver>
                             , ICaptureDeviceObserver, IAccessDeviceObserver, IVerificationObserver
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
      _observer = new BioObserver<IFullLocationObserver>();
      _devices  = new Dictionary<LocationDevice, ILocationDeviceObserver>();
      _verifyer = new TrackLocationVerification(locator);

      _verifyer.Subscribe(this);

       Update(location);
    }

    public void Update(Location location)
    {
      if (location == null)
        return;

      if (_devices.Count > 0)
        Stop();

      _currentLocation = location;

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
          _devices.Add(LocationDevice.AccessDevice, new LocationAccessDeviceObserver(_locator, this));         
      }

      if (_currentLocation.CaptureDevice != null)
      {
        if (!_devices.ContainsKey(LocationDevice.CaptureDevice))
          _devices.Add(LocationDevice.CaptureDevice, new LocationCaptureDeviceObserver(_locator, this));        
      }
    }
    
    public bool IsOk()
    {     
      foreach ( KeyValuePair<LocationDevice, ILocationDeviceObserver> par in _devices)
      {
       bool state = par.Value.IsDeviceOk;
       if (!state)
          return state;
      }

      return _devices.Count > 0 ? true : false;
    }

    public void Subscribe(IFullLocationObserver observer) {
      _observer.Subscribe(observer);     
      observer.OnOk(IsOk());
    }

    public void Unsubscribe(IFullLocationObserver observer){
      _observer.Unsubscribe(observer);      
    }

    public void UnsubscribeAll() {
      _observer.UnsubscribeAll();
    }

    public void OnFrame(ref Bitmap frame) {
      foreach (KeyValuePair<int, IFullLocationObserver> observer in _observer.Observers)      
        observer.Value.OnCaptureDeviceFrameChanged(ref frame);      
    }

    public void OnStop(bool stopped, string message, LocationDevice device) {
      if (_observer.Observers.Count <= 0)
        return;    
      OnError(new Exception(message), device);
    }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) {
      OnReady(started);
    }

    public void OnCardDetected(string cardNumber){
      _verifyer.StartByCard(cardNumber, CurrentLocation);
      //foreach (KeyValuePair<int, IFullLocationObserver> observer in _observer.Observers)
      //  observer.Value.OnStartVerificationByCard(cardNumber);    
    }

    public void OnError(Exception ex, LocationDevice device)
    {
      foreach (KeyValuePair<int, IFullLocationObserver> observer in _observer.Observers)
        observer.Value.OnError(ex, device);
    }

    public void OnReady(bool isReady){
      foreach (KeyValuePair<int, IFullLocationObserver> observer in _observer.Observers)
        observer.Value.OnOk(IsOk());       
    }

    public string GetAccessDeviceName {
      get { return _currentLocation.AccessDevice == null ? "" : _currentLocation.AccessDevice.Portname; }
    }
    public string GetCaptureDeviceName {
      get { return _currentLocation.CaptureDevice == null ? "" : _currentLocation.CaptureDevice.Devicename; }
    }

    public void Stop()
    {
      foreach (KeyValuePair<LocationDevice, ILocationDeviceObserver> pair in _devices)
      {
        pair.Value.Stop();        
        OnError(new Exception(), pair.Key);
      }
    }

    public void OnVerificationFailure(Exception ex)
    {
      foreach (KeyValuePair<int, IFullLocationObserver> observer in _observer.Observers)
        observer.Value.OnVerificationFailure(ex);

      _accessDeviceEngine.Execute(AccessDeviceCommands.CommandReady, GetAccessDeviceName);
    }

    public void OnVerificationSuccess(bool state)
    {
      foreach (KeyValuePair<int, IFullLocationObserver> observer in _observer.Observers)
        observer.Value.OnVerificationSuccess(state);

      _accessDeviceEngine.Execute(AccessDeviceCommands.CommandAccess, GetAccessDeviceName);

      _accessDeviceEngine.Execute(AccessDeviceCommands.CommandReady, GetAccessDeviceName);
    }

    public void OnVerificationProgress(int progress)
    {
      foreach (KeyValuePair<int, IFullLocationObserver> observer in _observer.Observers)
        observer.Value.OnVerificationProgress( progress);    
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

    public IScreen ScreenViewModel { get; set; }

    public string Caption {
      get { return _currentLocation.LocationName; }
    }

    public Location CurrentLocation {
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


    private BioObserver<IFullLocationObserver>    _observer;
    private TrackLocationVerification _verifyer;
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

  public class DeviceStateHolder
  {
    public bool IsOK { get; private set; }
    string Message { get; set; }

    public void Update(bool state, string message = "")
    {
      IsOK = state;
      Message = message;
    }

    public void Reset()
    {
      IsOK = false;
      Message = "";
    }
  }
}
