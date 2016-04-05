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
      RefreshConnectedDevices();
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
        if (!AccessDevicesNames.Contains(deviceName))
          AccessDevicesNames.Add(deviceName);
      }
    }

    public AccessDevice GetDevice()
    {
      AccessDevice result = null;
      if (DesiredCaptureDeviceName == ActiveCaptureDeviceName)
        return result;

      if (DesiredCaptureDeviceName == string.Empty && ActiveCaptureDeviceName != string.Empty)
        return new AccessDevice() { Id = CurrentLocation.AccessDevice.Id, EntityState = EntityState.Deleted };

      if (DesiredCaptureDeviceName != string.Empty && ActiveCaptureDeviceName == string.Empty)
        return new AccessDevice() { EntityState = EntityState.Added };

      return result;
    }

    public void OnMouseRightButtonDown(string deviceItem) { MenuRemoveStatus = false; SelectedAccessDevice = null; }


    public void RefreshData()
    {

      if (!IsActive)
        return;

      foreach (AccessDevice ac in _database.Locations.AccessDevices)
      {
        if (ac == null)
          continue;
        if (!AccessDevicesNames.Contains(ac.Portname))
          AccessDevicesNames.Add(ac.Portname);
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
      if (DesiredCaptureDeviceName == SelectedAccessDevice)
        DesiredCaptureDeviceName = string.Empty;
    }

    public void OnActive(string source)
    {
      DesiredCaptureDeviceName = SelectedAccessDevice;
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
      get { return DesiredCaptureDeviceName != ActiveCaptureDeviceName; }
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
      get { return _desiredCaptureDeviceName; }
      set
      {      
         _desiredCaptureDeviceName = value;
         OnDeviceChanged();
         NotifyOfPropertyChange(() => DesiredCaptureDeviceName);        
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
            ActiveCaptureDeviceName = value.AccessDevice == null ? string.Empty : value.AccessDevice.Portname;
            DesiredCaptureDeviceName = ActiveCaptureDeviceName;
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
