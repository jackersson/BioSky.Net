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

      DragableWithDisabledItem disabledDragable = new DragableWithDisabledItem();
      DragableWithRemoveItem   removeDragable   = new DragableWithRemoveItem();

      DevicesList = new DragablListBoxViewModel(disabledDragable);

      DevicesInList = new DragablListBoxViewModel(removeDragable);
      DevicesInList.ItemRemoved += DevicesList.ItemDropped;

      DevicesOutList = new DragablListBoxViewModel(removeDragable);
      DevicesOutList.ItemRemoved += DevicesList.ItemDropped;

      AccessDevicesNames = _bioEngine.AccessDeviceEngine().GetAccessDevicesNames();
      AccessDevicesNames.CollectionChanged += AccessDevicesNames_CollectionChanged;


     // _bioEngine.Database().PersonHolder.DataChanged += RefreshData;

      RefreshData();
    }    

    public void AddToGeneralDeviceList(DragableItem item, bool isEnabled = true)
    {
      if (item == null)
        return;

      DragableItem newItem = item.Clone();
      newItem.ItemEnabled = isEnabled;
      DevicesList.Add(newItem);
    }

    public void RefreshData()
    {
      DevicesInList.Clear();
      DevicesOutList.Clear();
      DevicesList.Clear();

      foreach (AccessDevice item in _bioEngine.Database().AccessDeviceHolder.Data)
      {
        DragableItem dragableItem = new DragableItem() { ItemContext = item.Clone(), ItemEnabled = true, DisplayName = item.Portname };

        if (_location.Id <= 0)
        {
          if (item.Type == AccessDevice.Types.AccessDeviceType.DeviceNone)
            AddToGeneralDeviceList(dragableItem, true);
          else
            AddToGeneralDeviceList(dragableItem, false);

          continue;
        }
        else
        {
          if (item.Locationid == _location.Id)
          {
            switch (item.Type)
            {
              case AccessDevice.Types.AccessDeviceType.DeviceIn:
                DevicesInList.Add(dragableItem);
                AddToGeneralDeviceList(dragableItem, false);
                break;
              case AccessDevice.Types.AccessDeviceType.DeviceOut:
                DevicesOutList.Add(dragableItem);
                AddToGeneralDeviceList(dragableItem, false);
                break;
              case AccessDevice.Types.AccessDeviceType.DeviceNone:
                AddToGeneralDeviceList(dragableItem, true);
                break;
            }
          }
        }     
      }

      foreach(string deviceName in AccessDevicesNames)
      {
        AccessDevice device = new AccessDevice() { Portname = deviceName };
        DragableItem dragableItem = new DragableItem() { ItemContext = device, ItemEnabled = true, DisplayName = device.Portname };
        AddToGeneralDeviceList(dragableItem, true);
      }
    }

    public RepeatedField<AccessDevice> GetAccessDevices()
    {
      RepeatedField<AccessDevice> accessDevices = new RepeatedField<AccessDevice>();

      foreach (DragableItem item in DevicesInList.DragableItems)
      {
        AccessDevice accessDevice = (AccessDevice)item.ItemContext;
        accessDevice.Type = AccessDevice.Types.AccessDeviceType.DeviceIn;
        accessDevices.Add(accessDevice);
      }

      foreach (DragableItem item in DevicesOutList.DragableItems)
      {
        AccessDevice accessDevice = (AccessDevice)item.ItemContext;
        accessDevice.Type = AccessDevice.Types.AccessDeviceType.DeviceOut;
        accessDevices.Add(accessDevice);
      }

      return accessDevices;
    }

    private void AccessDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshData();
    }

    public void Update(Location location)
    {
      _location = location.Clone();
      RefreshData();
    }
    public void Apply()
    {

      
    }
    public void Remove(bool all)
    {

    }

    #region UI
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

    private DragablListBoxViewModel _devicesList;
    public DragablListBoxViewModel DevicesList
    {
      get { return _devicesList; }
      set
      {
        if (_devicesList != value)
        {
          _devicesList = value;
          NotifyOfPropertyChange(() => DevicesList);
        }
      }
    }

    private DragablListBoxViewModel _devicesInList;
    public DragablListBoxViewModel DevicesInList
    {
      get { return _devicesInList; }
      set
      {
        if (_devicesInList != value)
        {
          _devicesInList = value;
          NotifyOfPropertyChange(() => DevicesInList);
        }
      }
    }

    private DragablListBoxViewModel _devicesOutList;
    public DragablListBoxViewModel DevicesOutList
    {
      get { return _devicesOutList; }
      set
      {
        if (_devicesOutList != value)
        {
          _devicesOutList = value;
          NotifyOfPropertyChange(() => DevicesOutList);
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
}
