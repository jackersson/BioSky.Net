﻿using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public class TrackLocation : IObserver<AccessDeviceActivity>
  {

    public TrackLocation(IProcessorLocator locator, Location location)
    {
      _accessDeviceEngine = locator.GetProcessor<IAccessDeviceEngine>();
      _database           = locator.GetProcessor<IBioSkyNetRepository>();

      Update(location);
    }

    public void Update(Location location)
    {
      _location = location;
    }

    public long LocationID
    {
      get { return _location.Id; }
    }

    public void Start()
    {

      List<AccessDevice> access_devices = _database.AccessDevices.AccessDevices
                                          .Where(ad => ad.Locationid == _location.Id)
                                          .ToList();

      //foreach (AccessDevice ac in access_devices)      
        //_accessDeviceEngine.Add(ac.Portname);
      
      
      List<CaptureDevice> capture_devices = _database.CaptureDevices.CaptureDevices
                                            .Where( cap => cap.Locationid == _location.Id)
                                            .ToList();      

      //_accessDeviceEngine.Add(_location.Devices_IN_);
      Subscribe(this);
    }

    public void Subscribe(IObserver<AccessDeviceActivity> observer)
    {
      //_accessDeviceEngine.Subscribe(observer, _location.Devices_IN_);
    }

    public void Unsubscribe(IObserver<AccessDeviceActivity> observer)
    {

    }

    public void Stop()
    {
      //_accessDeviceEngine.Remove(_location.Devices_IN_);
    }

    public void OnNext(AccessDeviceActivity value)
    {
      throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
      throw new NotImplementedException();
    }

    public void OnCompleted()
    {
      throw new NotImplementedException();
    }

    public object ScreenViewModel { get; set; }

    public string Caption
    {
      get { return _location.LocationName; }
    }

    private bool _isChecked;

    public bool IsChecked
    {
      get { return _isChecked; }
      set
      {
        if (_isChecked == value)
          return;
        _isChecked = value;
      }
    }

    private Location _location;
    private readonly IAccessDeviceEngine  _accessDeviceEngine;
    private readonly IBioSkyNetRepository _database;
    
  }
}