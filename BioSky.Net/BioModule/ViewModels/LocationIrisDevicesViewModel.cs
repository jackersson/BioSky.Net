//using BioData;
using Caliburn.Micro;
using System;
using BioService;
using BioContracts;
using BioModule.Utils;
using BioContracts.FingerprintDevices;

namespace BioModule.ViewModels
{
  public class LocationIrisDevicesViewModel : Screen, IUpdatable
  {
    public LocationIrisDevicesViewModel(IProcessorLocator locator)
    {
      DisplayName = "IrisDevices";

      _locator = locator;

      _bioEngine = _locator.GetProcessor<IBioEngine>();
      _database = _locator.GetProcessor<IBioSkyNetRepository>();

      IrisDevicesNames = new AsyncObservableCollection<string>();
    }

    #region Update

    protected override void OnActivate()
    {
      _bioEngine.IrisDeviceEngine().GetDevicesNames().CollectionChanged += IrisDevicesNames_CollectionChanged;
      RefreshData();
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      _bioEngine.IrisDeviceEngine().GetDevicesNames().CollectionChanged -= IrisDevicesNames_CollectionChanged;
      base.OnDeactivate(close);
    }

    private void IrisDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
      AsyncObservableCollection<string> temp = _bioEngine.IrisDeviceEngine().GetDevicesNames();
      foreach (string device in temp)
      {
        if (!string.IsNullOrEmpty(device) && !IrisDevicesNames.Contains(device))
          IrisDevicesNames.Add(device);
      }
    }

    public IrisDevice GetDevice()
    {
      if (string.Equals(DesiredDeviceName, ActiveDeviceName))
        return null;

      bool hasDesiredDeviceName = !string.IsNullOrEmpty(DesiredDeviceName);
      bool hasActiveDeviceName  = !string.IsNullOrEmpty(ActiveDeviceName);

      string deviceName = !hasDesiredDeviceName && hasActiveDeviceName ? CurrentLocation.IrisDevice.Devicename : DesiredDeviceName;
      EntityState entityState = hasDesiredDeviceName ? EntityState.Added : EntityState.Deleted;

      return new IrisDevice() { EntityState = entityState, Devicename = deviceName };
    }


    public void OnMouseRightButtonDown(string deviceItem) { MenuRemoveStatus = false; SelectedIrisDevice = null; }


    public void RefreshData()
    {
      if (!IsActive)
        return;

      IrisDevicesNames.Clear();
      foreach (string devicename in _database.Locations.IrisDevices)
      {
        if (!string.IsNullOrEmpty(devicename) && !IrisDevicesNames.Contains(devicename))
          IrisDevicesNames.Add(devicename);
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
      if (DesiredDeviceName == SelectedIrisDevice)
        DesiredDeviceName = string.Empty;
    }

    public void OnActive(string source)
    {
      DesiredDeviceName = SelectedIrisDevice;
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
            ActiveDeviceName = value.IrisDevice == null ? string.Empty : value.IrisDevice.Devicename;
            DesiredDeviceName = ActiveDeviceName;
          }
        }
      }
    }

    private string _selectedIrisDevice;
    public string SelectedIrisDevice
    {
      get { return _selectedIrisDevice; }
      set
      {
        if (_selectedIrisDevice != value)
        {
          _selectedIrisDevice = value;
          MenuRemoveStatus = value == null ? false : true;
          NotifyOfPropertyChange(() => SelectedIrisDevice);
        }
      }
    }

    private AsyncObservableCollection<string> _fingerIrisNames;
    public AsyncObservableCollection<string> IrisDevicesNames
    {
      get { return _fingerIrisNames; }
      set
      {
        if (_fingerIrisNames != value)
        {
          _fingerIrisNames = value;
          NotifyOfPropertyChange(() => IrisDevicesNames);
        }
      }
    }



    #endregion

    #region Global Variables    
    private readonly IProcessorLocator _locator;
    private readonly IBioEngine _bioEngine;
    private readonly IBioSkyNetRepository _database;

    public event EventHandler DeviceChanged;

    #endregion
  }
}
