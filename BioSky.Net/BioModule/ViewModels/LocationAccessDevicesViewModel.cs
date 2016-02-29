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

      _locator       = locator ;   
          
      _bioEngine     = _locator.GetProcessor<IBioEngine>();
      
      _accessDevicesList  = new AsyncObservableCollection<AccessDeviceItem>();

      AccessDevicesNames = _bioEngine.AccessDeviceEngine().GetAccessDevicesNames();      
    }

    #region Update

    protected override void OnActivate()
    {
      AccessDevicesNames.CollectionChanged += AccessDevicesNames_CollectionChanged;
      RefreshData();
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      AccessDevicesNames.CollectionChanged -= AccessDevicesNames_CollectionChanged;
      base.OnDeactivate(close);
    }   

    private void AccessDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshConnectedDevices();
    }
    public void Update(Location location)
    {
      _location = location;
      RefreshData();      
    }
    public void RefreshConnectedDevices()
    {
      foreach (string deviceName in AccessDevicesNames)
      {
        if (!IsDeviceUsed(deviceName))
        {
          AccessDevice     device     = new AccessDevice()     { Portname = deviceName };
          AccessDeviceItem deviceItem = new AccessDeviceItem() { ItemContext = device
                                                               , ItemActive  = false                                                               
                                                               , ItemEnabled = true };
          AccessDevicesList.Add(deviceItem);
        }
      }
    }

    public void RefreshData()
    {
      AccessDevicesList.Clear();

      foreach (AccessDevice item in _bioEngine.Database().AccessDeviceHolder.Data)
      {
        bool state = (_location.Id == item.Locationid);
        AccessDeviceItem deviceItem  = new AccessDeviceItem() {  ItemContext = item
                                                               , ItemActive  = state
                                                               , ItemEnabled = state      };

        AccessDevicesList.Add(deviceItem);        
      }

      RefreshConnectedDevices();
    }
   
    public bool IsDeviceUsed(string deviceName)
    {      
      foreach (AccessDeviceItem item in AccessDevicesList)
      {
        if (item.ItemContext.Portname == deviceName)
          return true;          
      }
      return false;
    }
         
    #endregion

    #region BioService


    public RepeatedField<AccessDevice> GetAccessDevices()
    {
      RepeatedField<AccessDevice> accessDevices = new RepeatedField<AccessDevice>();

      foreach (AccessDeviceItem item in AccessDevicesList)
      {
        if (item.ItemActive)
        {
          AccessDevice accessDevice = item.ItemContext;       
          accessDevices.Add(accessDevice);
        }
      }

      return accessDevices;
    }

    #endregion

    #region Interface

    public void OnRemove(string source)
    {      
       SelectedAccessDevice.ItemEnabled = false;      
    }

    public void OnActivate(string source)
    {        
       foreach (AccessDeviceItem item in AccessDevicesList)
         item.ItemEnabled = false;
       SelectedAccessDevice.ItemEnabled = true;        
    }
    
    public void OnMouseRightButtonDownAccess(AccessDeviceItem deviceItem)
    {      
      MenuRemoveStatus = (SelectedAccessDevice != null);
      SelectedAccessDevice = deviceItem;
    } 

    public void Apply()  {}   
    
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

    private AccessDeviceItem _selectedAccessDevice;
    public AccessDeviceItem SelectedAccessDevice
    {
      get { return _selectedAccessDevice; }
      set
      {
        if (_selectedAccessDevice != value)
        {
          _selectedAccessDevice = value;

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

    private AsyncObservableCollection<AccessDeviceItem> _accessDevicesList;
    public AsyncObservableCollection<AccessDeviceItem> AccessDevicesList
    {
      get { return _accessDevicesList; }
      set
      {
        if (_accessDevicesList != value)
        {
          _accessDevicesList = value;
          NotifyOfPropertyChange(() => AccessDevicesList);
        }
      }
    }
    #endregion

    #region Global Variables

    private          Location          _location     ;
    private readonly IProcessorLocator _locator      ;
    private readonly IBioEngine        _bioEngine    ;
  

    #endregion
  }
  public class AccessDeviceItem : DeviceItemBase<AccessDevice> { }
}
