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
      DisplayName = "CaptureDevices";

      _locator = locator;

      _bioEngine = _locator.GetProcessor<IBioEngine>();
      _database  = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier  = _locator.GetProcessor<INotifier>();
      CaptureDevicesNames = new AsyncObservableCollection<string>();
    }

    #region Update
    public void RefreshConnectedDevices()
    {
      AsyncObservableCollection<string> temp = _bioEngine.CaptureDeviceEngine().GetDevicesNames();
      foreach (string deviceName in temp)
      {       
        if (!string.IsNullOrEmpty(deviceName) && !CaptureDevicesNames.Contains(deviceName))
          CaptureDevicesNames.Add(deviceName);        
      }      
    }

    public void RefreshData()
    {
      if (!IsActive)
        return;

      try
      {
        CaptureDevicesNames.Clear();
        foreach (string devicename in _database.Locations.CaptureDevices)
        {
          if (!string.IsNullOrEmpty(devicename) && !CaptureDevicesNames.Contains(devicename))
            CaptureDevicesNames.Add(devicename);
        }
      }
      catch (Exception ex)
      {
        _notifier.Notify(ex);
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
      RefreshData();
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
      if (DesiredDeviceName == SelectedCaptureDevice)
        DesiredDeviceName = string.Empty;
    }

    public void OnActive(string source)
    {
      DesiredDeviceName = SelectedCaptureDevice;
    }

    private void OnDeviceChanged()
    {
      if (DeviceChanged != null)
        DeviceChanged(null, EventArgs.Empty);      
    }

    public bool CanApply
    {
      get
      {
        return !string.IsNullOrEmpty(DesiredDeviceName);
      }
    }
    
    public CaptureDevice GetDevice()
    {
      if (string.Equals(DesiredDeviceName, ActiveDeviceName))
        return null;

      bool hasDesiredDeviceName = !string.IsNullOrEmpty(DesiredDeviceName);
      bool hasActiveDeviceName = !string.IsNullOrEmpty(ActiveDeviceName);

      string deviceName = !hasDesiredDeviceName && hasActiveDeviceName ? CurrentLocation.CaptureDevice.Devicename : DesiredDeviceName;
      EntityState entityState = hasDesiredDeviceName ? EntityState.Added : EntityState.Deleted;

      return new CaptureDevice() { EntityState = entityState, Devicename = deviceName };
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

    private string _activeDeviceName;
    public string ActiveDeviceName
    {
      get { return _activeDeviceName; }
      set
      {
        if (_activeDeviceName != value)
        {
          _activeDeviceName = value;
          NotifyOfPropertyChange(() => ActiveDeviceName);
        }
      }
    }

    private string _desiredDeviceName;
    public string DesiredDeviceName
    {
      get  { return _desiredDeviceName; }
      set
      {        
        _desiredDeviceName = value;
        OnDeviceChanged();
        NotifyOfPropertyChange(() => DesiredDeviceName); 
      }
    }
        
    public bool IsDeviceChanged
    {
      get { return DesiredDeviceName != ActiveDeviceName;  }
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
            ActiveDeviceName = value.CaptureDevice == null ? string.Empty : value.CaptureDevice.Devicename;
            DesiredDeviceName = ActiveDeviceName;
          }         
        }
      }
    }
    #endregion

    #region Global Variables    
    private readonly IProcessorLocator    _locator  ;
    private readonly IBioEngine           _bioEngine;
    private readonly IBioSkyNetRepository _database ;
    private readonly INotifier            _notifier ;
    public event EventHandler DeviceChanged;
    #endregion
  }
}
