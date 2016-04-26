//using BioData;
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

namespace BioModule.ViewModels
{
  public class LocationAccessDevicesViewModel : Screen, IUpdatable
  {
    public LocationAccessDevicesViewModel(IProcessorLocator locator)
    {
      DisplayName = "AccessDevices";

      _locator = locator;

      _bioEngine = _locator.GetProcessor<IBioEngine>();
      _database  = _locator.GetProcessor<IBioSkyNetRepository>();

      AccessDevicesNames = new AsyncObservableCollection<string>(); //_bioEngine.AccessDeviceEngine().GetAccessDevicesNames();
    }

    #region Update

    protected override void OnActivate()
    {
      _bioEngine.AccessDeviceEngine().GetDevicesNames().CollectionChanged += AccessDevicesNames_CollectionChanged;
      RefreshData();
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      _bioEngine.AccessDeviceEngine().GetDevicesNames().CollectionChanged -= AccessDevicesNames_CollectionChanged;
      base.OnDeactivate(close);
    }

    private void AccessDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
      AsyncObservableCollection<string> temp = _bioEngine.AccessDeviceEngine().GetDevicesNames();
      foreach (string deviceName in temp)
      {
        if (!string.IsNullOrEmpty(deviceName) && !AccessDevicesNames.Contains(deviceName))
          AccessDevicesNames.Add(deviceName);
      }
    }

   

    public AccessDevice GetDevice()
    {
      if (string.Equals(DesiredDeviceName, ActiveDeviceName))
        return null;

      bool hasDesiredDeviceName = !string.IsNullOrEmpty(DesiredDeviceName);
      bool hasActiveDeviceName  = !string.IsNullOrEmpty(ActiveDeviceName );

      string portName = !hasDesiredDeviceName && hasActiveDeviceName ? CurrentLocation.AccessDevice.Portname : DesiredDeviceName;
      EntityState entityState = hasDesiredDeviceName ? EntityState.Added : EntityState.Deleted;

      return new AccessDevice() { EntityState = entityState, Portname = portName };      
    }

    public void OnMouseRightButtonDown(string deviceItem) { MenuRemoveStatus = false; SelectedAccessDevice = null; }


    public void RefreshData()
    {

      if (!IsActive)
        return;

      AccessDevicesNames.Clear();
      foreach (string portname in _database.Locations.AccessDevices)
      {
        if (!string.IsNullOrEmpty(portname) && !AccessDevicesNames.Contains(portname))
          AccessDevicesNames.Add(portname);
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
      if (DesiredDeviceName == SelectedAccessDevice)
        DesiredDeviceName = string.Empty;
    }

    public void OnActive(string source)
    {
      DesiredDeviceName = SelectedAccessDevice;
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
    public bool CanApply
    {
      get
      {
        return !string.IsNullOrEmpty(DesiredDeviceName);
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
            ActiveDeviceName = value.AccessDevice == null ? string.Empty : value.AccessDevice.Portname;
            DesiredDeviceName = ActiveDeviceName;
          }
        }
      }
    }

    private string _selectedAccessDevice;
    public string SelectedAccessDevice
    {
      get { return _selectedAccessDevice; }
      set
      {
        if (_selectedAccessDevice != value)
        {
          _selectedAccessDevice = value;
          MenuRemoveStatus = value == null ? false : true;
          NotifyOfPropertyChange(() => SelectedAccessDevice);
        }
      }
    }

    private AsyncObservableCollection<string> _accessDevicesNames;
    public AsyncObservableCollection<string> AccessDevicesNames
    {
      get { return _accessDevicesNames; }
      set
      {
        if (_accessDevicesNames != value)
        {
          _accessDevicesNames = value;
          NotifyOfPropertyChange(() => AccessDevicesNames);
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
