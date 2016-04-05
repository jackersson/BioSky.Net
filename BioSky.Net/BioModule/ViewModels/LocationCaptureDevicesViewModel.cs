﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioModule.Utils;
using BioService;
using Caliburn.Micro;
using Google.Protobuf.Collections;
using BioContracts;

namespace BioModule.ViewModels
{
  public class LocationCaptureDevicesViewModel : Screen, IUpdatable
  {
    public LocationCaptureDevicesViewModel(IProcessorLocator locator)
    {
      _locator = locator;

      _bioEngine = _locator.GetProcessor<IBioEngine>();
      _database  = _locator.GetProcessor<IBioSkyNetRepository>();

      CaptureDevicesNames = new AsyncObservableCollection<string>();
    }

    #region Update
    public void RefreshConnectedDevices()
    {
      AsyncObservableCollection<string> temp = _bioEngine.CaptureDeviceEngine().GetDevicesNames();
      foreach (string deviceName in temp)
      {       
        if (!CaptureDevicesNames.Contains(deviceName))
          CaptureDevicesNames.Add(deviceName);        
      }      
    }

    public void RefreshData()
    {
      if (!IsActive)
        return;
 

      foreach (CaptureDevice cd in _database.Locations.CaptureDevices)
      {
        if (!CaptureDevicesNames.Contains(cd.Devicename))
        CaptureDevicesNames.Add(cd.Devicename);
      }

      RefreshConnectedDevices();      
    }

    public void OnMouseRightButtonDown(string deviceItem) { MenuRemoveStatus = false; SelectedCaptureDevice = null; }

    public void Update(Location location)
    {      
      CurrentLocation = location;
      RefreshData();
    }
    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshConnectedDevices();
    }
    #endregion

    #region Interface
    protected override void OnActivate()
    {
      base.OnActivate();
      _bioEngine.CaptureDeviceEngine().GetDevicesNames().CollectionChanged += CaptureDevicesNames_CollectionChanged;
      RefreshData(); 
    }

    protected override void OnDeactivate(bool close)
    {
      _bioEngine.CaptureDeviceEngine().GetDevicesNames().CollectionChanged -= CaptureDevicesNames_CollectionChanged;

      base.OnDeactivate(close);
    }

    public void OnRemove(string source)
    {
      if (DesiredCaptureDeviceName == SelectedCaptureDevice)
        DesiredCaptureDeviceName = string.Empty;
    }

    public void OnActive(string source)
    {
      DesiredCaptureDeviceName = SelectedCaptureDevice;
    }

    private void OnDeviceChanged()
    {
      if (DeviceChanged != null)
        DeviceChanged(null, EventArgs.Empty);      
    }     

    public CaptureDevice GetDevice()
    {
      CaptureDevice result = null;
      if (DesiredCaptureDeviceName == ActiveCaptureDeviceName)
        return result;

      if (DesiredCaptureDeviceName == string.Empty && ActiveCaptureDeviceName != string.Empty)
        return new CaptureDevice() { Id = CurrentLocation.CaptureDevice.Id, EntityState = EntityState.Deleted };
      
      if (DesiredCaptureDeviceName != string.Empty && ActiveCaptureDeviceName == string.Empty)
        return new CaptureDevice() { EntityState = EntityState.Added };

      return result;
    }

    public void Apply() { }
    #endregion
        
    #region UI

    private bool _menuRemoveStatus;
    public bool MenuRemoveStatus
    {
      get { return _menuRemoveStatus; }
      set
      {
        if (_menuRemoveStatus != value)
        {
          _menuRemoveStatus = value;
          NotifyOfPropertyChange(() => MenuRemoveStatus);
        }
      }
    }

    private string _selectedCaptureDevice;
    public string SelectedCaptureDevice
    {
      get { return _selectedCaptureDevice; }
      set
      {
        if (_selectedCaptureDevice != value)
        {
          _selectedCaptureDevice = value;
          MenuRemoveStatus = value != null;
          NotifyOfPropertyChange(() => SelectedCaptureDevice);
        }
      }
    }
    
    private AsyncObservableCollection<string> _captureDevicesNames;
    public AsyncObservableCollection<string> CaptureDevicesNames
    {
      get { return _captureDevicesNames; }
      set
      {
        if (_captureDevicesNames != value)
        {
          _captureDevicesNames = value;
          NotifyOfPropertyChange(() => CaptureDevicesNames);
        }
      }
    }

    private string _activeCaptureDeviceName;
    public string ActiveCaptureDeviceName
    {
      get { return _activeCaptureDeviceName; }
      set
      {
        if (_activeCaptureDeviceName != value)
        {
          _activeCaptureDeviceName = value;
          NotifyOfPropertyChange(() => ActiveCaptureDeviceName);
        }
      }
    }

    private string _desiredCaptureDeviceName;
    public string DesiredCaptureDeviceName
    {
      get  { return _desiredCaptureDeviceName; }
      set
      {        
        _desiredCaptureDeviceName = value;
        OnDeviceChanged();
        NotifyOfPropertyChange(() => DesiredCaptureDeviceName); 
      }
    }
        
    public bool IsDeviceChanged
    {
      get { return DesiredCaptureDeviceName != ActiveCaptureDeviceName;  }
    }

    private Location _currentLocation;
    public Location CurrentLocation
    {
      get { return _currentLocation; }
      set
      {
        if (_currentLocation != value)
        {
          _currentLocation = value;

          NotifyOfPropertyChange(() => CurrentLocation);

          if ( value != null)
          {          
            ActiveCaptureDeviceName = value.CaptureDevice == null ? string.Empty : value.CaptureDevice.Devicename;
            DesiredCaptureDeviceName = ActiveCaptureDeviceName;
          }         
        }
      }
    }
    #endregion

    #region Global Variables    
    private readonly IProcessorLocator _locator  ;
    private readonly IBioEngine        _bioEngine;
    private readonly IBioSkyNetRepository _database;

    public event EventHandler DeviceChanged;
    #endregion
  }
}
