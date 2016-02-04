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
using BioFaceService;

using Google.Protobuf.Collections;
using BioContracts;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class LocationAccessDevicesViewModel : Screen, IUpdatable
  {
    public LocationAccessDevicesViewModel(IProcessorLocator locator)
    {
      DisplayName = "Access Devices";

      _locator    = locator ;
      _bioService = _locator.GetProcessor<IServiceManager>();
      _bioEngine  = _locator.GetProcessor<IBioEngine>();

      DragableWithDisabledItem disabledDragable = new DragableWithDisabledItem();
      DragableWithRemoveItem   removeDragable   = new DragableWithRemoveItem();

      DevicesList = new DragablListBoxViewModel(disabledDragable);

      DevicesInList = new DragablListBoxViewModel(removeDragable);
      DevicesInList.ItemRemoved += DevicesList.ItemDropped;

      DevicesOutList = new DragablListBoxViewModel(removeDragable);
      DevicesOutList.ItemRemoved += DevicesList.ItemDropped;

      _bioEngine.Database().PersonHolder.DataChanged += RefreshData;

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
        DragableItem dragableItem = new DragableItem() { ItemContext = item, ItemEnabled = true, DisplayName = item.Portname };

        if (_location == null)
          continue;
        
        if(item.Locationid == _location.Id)
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

    public void Update(Location location)
    {
      _location = location;
      RefreshData();
    }
    public void Apply()
    {

    }

    private          Location          _location  ;
    private readonly IProcessorLocator _locator   ;
    private readonly IBioEngine        _bioEngine ;
    private readonly IServiceManager   _bioService;

  }   
}
