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
using BioModule.DragDrop;

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
    public LocationAccessDevicesViewModel(IProcessorLocator locator, IWindowManager windowManager)
    {
      DisplayName = "AccessDevices";

      _locator       = locator ;
      _windowManager = windowManager;

      _bioService    = _locator.GetProcessor<IServiceManager>();
      _bioEngine     = _locator.GetProcessor<IBioEngine>();


      _captureDevicesList = new ObservableCollection<LocationItem>();
      _accessDevicesList   = new ObservableCollection<LocationItem>();

      AccessDevicesNames = _bioEngine.AccessDeviceEngine().GetAccessDevicesNames();
      AccessDevicesNames.CollectionChanged += AccessDevicesNames_CollectionChanged;

      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;

      RefreshData();
      RefreshCaptureDevices();
    }

    #region Update

    private void AccessDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshData();
    }
    public void Update(Location location)
    {
      _location = location.Clone();
      RefreshData();
      RefreshCaptureDevices();
    }
    public void RefreshData()
    {
      AccessDevicesList.Clear();

      foreach (AccessDevice item in _bioEngine.Database().AccessDeviceHolder.Data)
      {
        LocationItem dragableItem = new LocationItem() { ItemContext = item.Clone(), ItemEnabled = true, DisplayName = item.Portname };

        if (_location.Id <= 0)
        {
          if (item.Type == AccessDevice.Types.AccessDeviceType.DeviceNone)
            AddToGeneralDeviceList(dragableItem, false, true);
          else
          {
            dragableItem.ItemInUse = true;
            AddToGeneralDeviceList(dragableItem, false, true);
          }

          continue;
        }
        else
        {
          if (item.Locationid == _location.Id)
          {
            switch (item.Type)
            {
              case AccessDevice.Types.AccessDeviceType.DeviceIn:
                AddToGeneralDeviceList(dragableItem, true, true);
                break;
              case AccessDevice.Types.AccessDeviceType.DeviceNone:
                AddToGeneralDeviceList(dragableItem, false, true);
                break;
            }
          }
          else
          {
            dragableItem.ItemInUse = true;
            AddToGeneralDeviceList(dragableItem, false, true);
          }
        }
      }

      foreach (string deviceName in AccessDevicesNames)
      {
        if (!IsDeviceUsed(deviceName, true))
        {
          AccessDevice device = new AccessDevice() { Portname = deviceName };
          LocationItem dragableItem = new LocationItem() { ItemContext = device, ItemEnabled = true, DisplayName = device.Portname };
          AddToGeneralDeviceList(dragableItem, false, true);
        }
      }
    }

    public void RefreshCaptureDevices()
    {
      CaptureDevicesList.Clear();

      foreach (CaptureDevice item in _bioEngine.Database().CaptureDeviceHolder.Data)
      {
        LocationItem dragableItem = new LocationItem() { ItemContext = item, ItemEnabled = true, DisplayName = item.Devicename };

        if (_location.Id <= 0)
        {
          if (item.Locationid <= 0)
            AddToGeneralDeviceList(dragableItem, false, false);
          else
          {
            dragableItem.ItemInUse = true;
            AddToGeneralDeviceList(dragableItem, false, false);
          }

          continue;
        }
        else
        {
          if (item.Locationid == _location.Id)          
            AddToGeneralDeviceList(dragableItem, true, false);          
          else
          {
            dragableItem.ItemInUse = true;
            AddToGeneralDeviceList(dragableItem, false, false);
          }
        }
      }

      foreach (string deviceName in CaptureDevicesNames)
      {
        if (!IsDeviceUsed(deviceName, false))
        {
          CaptureDevice device = new CaptureDevice() { Devicename = deviceName, };
          LocationItem dragableItem = new LocationItem() { ItemContext = device, ItemEnabled = true, DisplayName = device.Devicename };
          AddToGeneralDeviceList(dragableItem, false, false);
        }
      }
    }

    public bool IsDeviceUsed(string deviceName, bool isAccessDevices)
    {
      if (isAccessDevices)
      {
        foreach (LocationItem item in AccessDevicesList)
        {
          if (item.DisplayName == deviceName)
            return true;
          else
            continue;
        }
        return false;
      }
      else
      {
        foreach (LocationItem item in CaptureDevicesList)
        {
          if (item.DisplayName == deviceName)
            return true;
          else
            continue;
        }
        return false;
      }
    }
    public void AddToGeneralDeviceList(LocationItem item, bool isEnabled = true, bool isAccessDevice = true)
    {
      if (item == null)
        return;

      LocationItem newItem = item.Clone();
      newItem.ItemEnabled = isEnabled;

      if(isAccessDevice)
        AccessDevicesList.Add(newItem);      
      else
        CaptureDevicesList.Add(newItem);      
    }



 
    #endregion

    #region BioService
    public RepeatedField<CaptureDevice> GetCaptureDevices()
    {
      RepeatedField<CaptureDevice> captureDevices = new RepeatedField<CaptureDevice>();

      foreach (LocationItem item in CaptureDevicesList)
      {
        if(item.ItemEnabled == true)
        {
          CaptureDevice captureDevice = (CaptureDevice)item.ItemContext;
          captureDevices.Add(captureDevice);
        }

      }

      return captureDevices;
    }

    public RepeatedField<AccessDevice> GetAccessDevices()
    {
      RepeatedField<AccessDevice> accessDevices = new RepeatedField<AccessDevice>();

      foreach (LocationItem item in AccessDevicesList)
      {
        if (item.ItemEnabled == true)
        {
          AccessDevice accessDevice = (AccessDevice)item.ItemContext;
          accessDevice.Type = AccessDevice.Types.AccessDeviceType.DeviceIn;
          accessDevices.Add(accessDevice);
        }

      }

      return accessDevices;
    }

    #endregion

    #region Interface

    public void OnRemove(string source)
    {
      switch (source)
      {
        case "CaptureDevice":
          if (SelectedCaptureDevice.ItemInUse == true)
            _windowManager.ShowDialog(new CustomTextDialogViewModel("Warning", "Camera used by another Location", DialogStatus.Error));
          else
            SelectedCaptureDevice.ItemEnabled = false;

          break;
        case "AccessDevice":
          if(SelectedAccessDevice.ItemInUse == true)
            _windowManager.ShowDialog(new CustomTextDialogViewModel("Warning", "Device used by another Location", DialogStatus.Error));
          else
            SelectedAccessDevice.ItemEnabled = false;

          break;
      }
    }

    public void OnActivate(string source)
    {
      switch (source)
      {
        case "CaptureDevice":
          if (SelectedCaptureDevice.ItemInUse == true)
            _windowManager.ShowDialog(new CustomTextDialogViewModel("Warning", "Camera used by another Location", DialogStatus.Error));
          else
          {
            foreach (LocationItem item in CaptureDevicesList)
              item.ItemEnabled = false;
            SelectedCaptureDevice.ItemEnabled = true;
          }         

          break;
        case "AccessDevice":
          if (SelectedAccessDevice.ItemInUse == true)
            _windowManager.ShowDialog(new CustomTextDialogViewModel("Warning", "Device used by another Location", DialogStatus.Error));
          else
          {
            foreach (LocationItem item in AccessDevicesList)
              item.ItemEnabled = false;
            SelectedAccessDevice.ItemEnabled = true;
          }

          break;
      }
    }
    public void EnableItem(LocationItem sourceitem, ObservableCollection<LocationItem> generalList)
    {
      foreach (LocationItem item in generalList)
      {
        if (item.DisplayName == sourceitem.DisplayName)
          item.ItemEnabled = true;
      }
    }
    public void OnMouseRightButtonDownAccess(LocationItem IsDragableItem)
    {      
      MenuRemoveStatus = (SelectedAccessDevice != null);
      SelectedAccessDevice = IsDragableItem;
    }
    public void OnSelectionChangeAccess()
    {
      MenuRemoveStatus = (SelectedAccessDevice != null);
    }
    public void OnMouseRightButtonDownCapture(LocationItem IsDragableItem)
    {
      MenuRemoveStatus = (SelectedCaptureDevice != null);
      SelectedCaptureDevice = IsDragableItem;
    }
    public void OnSelectionChangeCapture()
    {
      MenuRemoveStatus = (SelectedCaptureDevice != null);
    }

    public void Apply()
    {
    }
    public void Remove(bool all)
    {
    }
    #endregion

    #region UI
    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshCaptureDevices();
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

    private LocationItem _selectedAccessDevice;
    public LocationItem SelectedAccessDevice
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

    private LocationItem _selectedCaptureDevice;
    public LocationItem SelectedCaptureDevice
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

    private ObservableCollection<LocationItem> _accessDevicesList;
    public ObservableCollection<LocationItem> AccessDevicesList
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

    private ObservableCollection<LocationItem> _captureDevicesList;
    public ObservableCollection<LocationItem> CaptureDevicesList
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


    #endregion

    #region Global Variables
    private          Location          _location     ;
    private readonly IProcessorLocator _locator      ;
    private readonly IBioEngine        _bioEngine    ;
    private readonly IServiceManager   _bioService   ;
    private          IWindowManager    _windowManager;

    #endregion
  }

  public class LocationItem : PropertyChangedBase
  {
    public object ItemContext { get; set; }

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
          NotifyOfPropertyChange(() => DisplayName);
        }
      }
    }

    private bool _itemInUse;
    public bool ItemInUse
    {
      get { return _itemInUse; }
      set
      {
        if (_itemInUse != value)
        {
          _itemInUse = value;
          NotifyOfPropertyChange(() => ItemInUse);
          NotifyOfPropertyChange(() => DisplayName);
        }
      }
    }

    public LocationItem Clone()
    {
      return new LocationItem()
      {
        ItemContext = this.ItemContext
      , ItemEnabled = this.ItemEnabled
      , DisplayName = this.DisplayName
      , ItemInUse = this.ItemInUse

      };
    }

    public string DisplayName
    {
      get;
      set;
    }
  }
}
