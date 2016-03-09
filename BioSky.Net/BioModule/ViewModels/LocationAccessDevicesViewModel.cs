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
  /*
  public class LocationCaptureDevicesViewModel : Screen, IUpdatable
  {
    public LocationCaptureDevicesViewModel(IProcessorLocator locator)
    {    
      _locator = locator;
    
      _bioEngine  = _locator.GetProcessor<IBioEngine>();
      
      _captureDevicesList = new AsyncObservableCollection<CaptureDeviceItem>();
           
      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();   
    }

    public void RefreshConnectedDevices()
    {
      foreach (string deviceName in CaptureDevicesNames)
      {
        if (!IsDeviceUsed(deviceName))
        {
          CaptureDevice device         = new CaptureDevice()     { Devicename = deviceName };
          CaptureDeviceItem deviceItem = new CaptureDeviceItem() { ItemContext = device
                                                                 , ItemActive  = false                                                               
                                                                 , ItemEnabled = true };
          CaptureDevicesList.Add(deviceItem);
        }
      }
    }

    public void RefreshData()
    {
      CaptureDevicesList.Clear();

      /*
      foreach (CaptureDevice item in _bioEngine.Database().CaptureDeviceHolder.Data)
      {
        bool state = (_location.Id == item.Locationid);
        CaptureDeviceItem deviceItem  = new CaptureDeviceItem() {  ItemContext = item
                                                                 , ItemActive  = state
                                                                 , ItemEnabled = state      };

        CaptureDevicesList.Add(deviceItem);        
      }
      

      RefreshConnectedDevices();
    }

    public void Update(Location location)
    {
      _location = location;
      RefreshData();
    }

    protected override void OnActivate()
    {
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;
      RefreshData();
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      CaptureDevicesNames.CollectionChanged -= CaptureDevicesNames_CollectionChanged;

      base.OnDeactivate(close);
    }
      
    public RepeatedField<CaptureDevice> GetCaptureDevices()
    {
      RepeatedField<CaptureDevice> captureDevices = new RepeatedField<CaptureDevice>();

      foreach (CaptureDeviceItem item in CaptureDevicesList)
      {
        if (item.ItemActive == true)
        {
          CaptureDevice captureDevice = item.ItemContext;
          captureDevices.Add(captureDevice);
        }
      }

      return captureDevices;
    }

    private AsyncObservableCollection<CaptureDeviceItem> _captureDevicesList;
    public AsyncObservableCollection<CaptureDeviceItem> CaptureDevicesList
    {
      get { return _captureDevicesList; }
      set
      {
        if (_captureDevicesList != value)
        {
          _captureDevicesList = value;
          NotifyOfPropertyChange(() => CaptureDevicesList);
        }
      }
    }

    public bool IsDeviceUsed(string deviceName)
    {
       foreach (CaptureDeviceItem item in CaptureDevicesList)
       {
         if (item.ItemContext.Devicename == deviceName)
           return true;        
       }
       return false;      
    }

    private CaptureDeviceItem _selectedCaptureDevice;
    public CaptureDeviceItem SelectedCaptureDevice
    {
      get { return _selectedCaptureDevice; }
      set
      {
        if (_selectedCaptureDevice != value)
        {
          _selectedCaptureDevice = value;
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

    public void OnRemove()
    {    
      if (SelectedCaptureDevice != null)
        SelectedCaptureDevice.ItemEnabled = false;     
    }

    public void OnMouseRightButtonDownCapture(CaptureDeviceItem deviceItem)
    {
      MenuRemoveStatus = (SelectedCaptureDevice != null);
      SelectedCaptureDevice = deviceItem;
    }
  
    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshConnectedDevices();
    }

    public void Apply() {}

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

    #region Global Variables

    private Location _location;
    private readonly IProcessorLocator _locator  ;
    private readonly IBioEngine        _bioEngine;    

    #endregion
  }

  */
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
      /*
      foreach (AccessDevice item in _bioEngine.Database().AccessDeviceHolder.Data)
      {
        bool state = (_location.Id == item.Locationid);
        AccessDeviceItem deviceItem  = new AccessDeviceItem() {  ItemContext = item
                                                               , ItemActive  = state
                                                               , ItemEnabled = state      };

        AccessDevicesList.Add(deviceItem);        
      }
      */
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

  /*
  public class DevicesListViewModel : Conductor<IScreen>.Collection.AllActive, IUpdatable
  {
    public DevicesListViewModel(IProcessorLocator locator)
    {
      _accessDevices  = new LocationAccessDevicesViewModel(locator);
      _captureDevices = new LocationCaptureDevicesViewModel(locator);
    }

    protected override void OnActivate()
    {
      ActivateItem(_accessDevices);
      ActivateItem(_captureDevices);
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      DeactivateItem(_accessDevices , false);
      DeactivateItem(_captureDevices, false);
      base.OnDeactivate(close);
    }

    public void Update(Location location )
    {
      _accessDevices .Update(location);
      _captureDevices.Update(location);
    }

    public void Apply() {}

    private readonly LocationAccessDevicesViewModel  _accessDevices;
    public LocationAccessDevicesViewModel AccessDevices
    {
      get { return _accessDevices; }
    }

    private readonly LocationCaptureDevicesViewModel _captureDevices;
    public LocationCaptureDevicesViewModel CaptureDevices
    {
      get { return _captureDevices; }
    }
  }


  public class DeviceItemBase<T> : PropertyChangedBase
  {
    public T ItemContext { get; set; }

    private bool _itemActive;
    public bool ItemActive
    {
      get { return _itemActive; }
      set
      {
        if (_itemActive != value)
        {
          _itemActive = value;
          NotifyOfPropertyChange(() => ItemActive);         
        }
      }
    }

    private bool _itemEnabled;
    public bool ItemEnabled
    {
      get { return _itemEnabled; }
      set
      {
        if (_itemEnabled != value)
        {
          _itemEnabled = value;
          NotifyOfPropertyChange(() => ItemEnabled);  
        }
      }
    }

    public DeviceItemBase<T> Clone()
    {
      return new DeviceItemBase<T>()
      {
          ItemContext = this.ItemContext
        , ItemActive  = this.ItemActive      
        , ItemEnabled = this.ItemEnabled
      };
    }    
  }

  public class AccessDeviceItem : DeviceItemBase<AccessDevice>{}
  public class CaptureDeviceItem : DeviceItemBase<CaptureDevice> { }
  */

  public class AccessDeviceItem : DeviceItemBase<AccessDevice> { }
}
