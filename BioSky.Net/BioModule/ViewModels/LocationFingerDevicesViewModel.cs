using BioData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BioModule.ViewModels;
using System.Windows.Input;

using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using BioService;

using Google.Protobuf.Collections;
using BioContracts;
using BioModule.Utils;
using BioContracts.FingerprintDevices;

namespace BioModule.ViewModels
{
  public class LocationFingerDevicesViewModel : Screen, IUpdatable
  {
    public LocationFingerDevicesViewModel(IProcessorLocator locator)
    {
      DisplayName = "FingerDevices";

      _locator = locator;

      _bioEngine = _locator.GetProcessor<IBioEngine>();
      _database = _locator.GetProcessor<IBioSkyNetRepository>();

      FingerDevicesNames = new AsyncObservableCollection<string>(); 
    }

    #region Update

    protected override void OnActivate()
    {
      _bioEngine.FingerprintDeviceEngine().GetDevicesNames().CollectionChanged += FingerDevicesNames_CollectionChanged;
      RefreshData();
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      _bioEngine.FingerprintDeviceEngine().GetDevicesNames().CollectionChanged -= FingerDevicesNames_CollectionChanged;
      base.OnDeactivate(close);
    }

    private void FingerDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshData();
    }
    public void Update(Location location)
    {
      CurrentLocation = location;
      RefreshData();
    }
    public void RefreshConnectedDevices()
    {
      AsyncObservableCollection<FingerprintDeviceInfo> temp = _bioEngine.FingerprintDeviceEngine().GetDevicesNames();
      foreach (FingerprintDeviceInfo device in temp)
      {
        if (!string.IsNullOrEmpty(device.Name) && !FingerDevicesNames.Contains(device.Name))
          FingerDevicesNames.Add(device.Name);
      }
    }
    
    public FingerprintDevice GetDevice()
    {     
      if (string.Equals(DesiredDeviceName, ActiveDeviceName))
        return null;

      bool hasDesiredDeviceName = !string.IsNullOrEmpty(DesiredDeviceName);
      bool hasActiveDeviceName  = !string.IsNullOrEmpty(ActiveDeviceName);

      string deviceName       = !hasDesiredDeviceName && hasActiveDeviceName ? CurrentLocation.FingerprintDevice.Devicename : DesiredDeviceName;
      EntityState entityState = hasDesiredDeviceName ? EntityState.Added : EntityState.Deleted;

      return new FingerprintDevice() { EntityState = entityState, Devicename = deviceName };
    }
      

    public void OnMouseRightButtonDown(string deviceItem) { MenuRemoveStatus = false; SelectedFingerDevice = null; }


    public void RefreshData()
    {

      if (!IsActive)
        return;

      FingerDevicesNames.Clear();
      foreach (string devicename in _database.Locations.FingerprintDevices)
      {
        if (!string.IsNullOrEmpty(devicename) && !FingerDevicesNames.Contains(devicename))
          FingerDevicesNames.Add(devicename);
      }

      RefreshConnectedDevices();
    }

    private void OnDeviceChanged()
    {
      if (DeviceChanged != null)
        DeviceChanged(null, EventArgs.Empty);
    }

    #endregion

    #region Interface

    public void OnRemove(string source)
    {
      if (DesiredDeviceName == SelectedFingerDevice)
        DesiredDeviceName = string.Empty;
    }

    public void OnActive(string source)
    {
      DesiredDeviceName = SelectedFingerDevice;
    }

    public void Apply() { }

    public bool CanApply
    {
      get
      {
        return !string.IsNullOrEmpty(DesiredDeviceName);
      }
    }

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

    public bool IsDeviceChanged
    {
      get { return DesiredDeviceName != ActiveDeviceName; }
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
      get { return _desiredDeviceName; }
      set
      {
        _desiredDeviceName = value;
        OnDeviceChanged();
        NotifyOfPropertyChange(() => DesiredDeviceName);
      }
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

          if (value != null)
          {
            ActiveDeviceName = value.FingerprintDevice == null ? string.Empty : value.FingerprintDevice.Devicename;
            DesiredDeviceName = ActiveDeviceName;
          }
        }
      }
    }

    private string _selectedFingerDevice;
    public string SelectedFingerDevice
    {
      get { return _selectedFingerDevice; }
      set
      {
        if (_selectedFingerDevice != value)
        {
          _selectedFingerDevice = value;
          MenuRemoveStatus = value == null ? false : true;
          NotifyOfPropertyChange(() => SelectedFingerDevice);
        }
      }
    }

    private AsyncObservableCollection<string> _fingerDevicesNames;
    public AsyncObservableCollection<string> FingerDevicesNames
    {
      get { return _fingerDevicesNames; }
      set
      {
        if (_fingerDevicesNames != value)
        {
          _fingerDevicesNames = value;
          NotifyOfPropertyChange(() => FingerDevicesNames);
        }
      }
    }



    #endregion

    #region Global Variables    
    private readonly IProcessorLocator    _locator  ;
    private readonly IBioEngine           _bioEngine;
    private readonly IBioSkyNetRepository _database ;

    public event EventHandler DeviceChanged;

    #endregion
  }
}
