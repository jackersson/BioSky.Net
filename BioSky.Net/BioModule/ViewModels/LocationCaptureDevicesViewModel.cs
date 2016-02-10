using BioData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioModule.DragDrop;
using BioContracts;
using BioService;
using BioModule.Utils;
using Google.Protobuf.Collections;

namespace BioModule.ViewModels
{
  public class LocationCaptureDevicesViewModel : Screen, IUpdatable
  {
    public LocationCaptureDevicesViewModel(IProcessorLocator locator, IWindowManager windowManager)
    {
      DisplayName = "CaptureDevices";

      _locator    = locator;
      _bioService = _locator.GetProcessor<IServiceManager>();
      _bioEngine  = _locator.GetProcessor<IBioEngine>();

      DragableWithDisabledItem disabledDragable = new DragableWithDisabledItem();
      DragableWithRemoveItem   removeDragable   = new DragableWithRemoveItem  ();

      DevicesList   = new DragablListBoxViewModel(disabledDragable);
      DevicesInList = new DragablListBoxViewModel(removeDragable);
      DevicesInList.ItemRemoved += DevicesList.ItemDropped;

      /*
      foreach (CaptureDevice item in _bioEngine.Database().CaptureDeviceHolder.Data)
      {
        DragableItem dragableItem = new DragableItem() { ItemContext = item, ItemEnabled = true, DisplayName = item.Devicename };
        AddToGeneralDeviceList(dragableItem);
      }
      */
  
    }

    public void AddToGeneralDeviceList(DragableItem item, bool isEnabled = true)
    {
      if (item == null)
        return;

      DragableItem newItem = item.Clone();
      newItem.ItemEnabled = isEnabled;
      DevicesList.Add(newItem);
    }
    /*private void OnCaptureDevicesChanged(CaptureDeviceList captureDevices)
    {
      foreach (CaptureDevice item in captureDevices.CaptureDevices)
      {
        DragableItem dragableItem = new DragableItem() { ItemContext = item, ItemEnabled = true, DisplayName = item.Devicename };

        if (DevicesList.ContainsItem(dragableItem))
        {
          return;
        }

        AddToGeneralDeviceList(dragableItem);
      }
    }*/

    public void RefreshData()
    {
      DevicesInList.Clear();
      DevicesList.Clear();

      foreach (CaptureDevice item in _bioEngine.Database().CaptureDeviceHolder.Data)
      {
        DragableItem dragableItem = new DragableItem() { ItemContext = item, ItemEnabled = true, DisplayName = item.Devicename };

        if (_location.Id <= 0)
        {
          if (item.Locationid <= 0)
            AddToGeneralDeviceList(dragableItem, true);
          else
            AddToGeneralDeviceList(dragableItem, false);

          continue;
        }
        else
        {
          if (item.Locationid == _location.Id)
          {
            DevicesInList.Add(dragableItem);
            AddToGeneralDeviceList(dragableItem, false);
          }
          else if (item.Locationid <= 0)          
            AddToGeneralDeviceList(dragableItem, true);          
          else
            AddToGeneralDeviceList(dragableItem, false);          
        }
      }
    }

    public RepeatedField<CaptureDevice> GetCaptureDevices()
    {
      RepeatedField<CaptureDevice> captureDevices = new RepeatedField<CaptureDevice>();

      foreach (DragableItem item in DevicesInList.DragableItems)
      {
        CaptureDevice captureDevice = (CaptureDevice)item.ItemContext;
        captureDevices.Add(captureDevice);
      }

      return captureDevices;
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

    public void Update(Location location)
    {
      _location = location;
      RefreshData();
    }
    public void Apply()
    {

    }
    public void Remove(bool all)
    {

    }

    private          Location          _location  ;
    private readonly IProcessorLocator _locator   ;
    private readonly IBioEngine        _bioEngine ;
    private readonly IServiceManager   _bioService;
  }
}
