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



namespace BioModule.ViewModels
{
  public class LocationAccessDevicesViewModel : Screen
  {
    public LocationAccessDevicesViewModel()
    {
      DisplayName = "Access Devices";

      DragableWithDisabledItem disabledDragable = new DragableWithDisabledItem();
      DragableWithRemoveItem removeDragable = new DragableWithRemoveItem();

      DevicesList = new DragablListBoxViewModel(disabledDragable);

      DevicesInList = new DragablListBoxViewModel(removeDragable);
      DevicesInList.ItemRemoved += DevicesList.ItemDropped;

      DevicesOutList = new DragablListBoxViewModel(removeDragable);
      DevicesOutList.ItemRemoved += DevicesList.ItemDropped;

      AccessDevice num22 = new AccessDevice() { PortName = ("COM" + 5) };
      DragableItem num222 = new DragableItem() { ItemContext = num22, ItemEnabled = false, DisplayName = num22.PortName };

      for (int i = 0; i != 20; i++)
      {
        AccessDevice num = new AccessDevice() { PortName = ("COM" + i) };
        DragableItem num2 = new DragableItem() { ItemContext = num, ItemEnabled = true, DisplayName = num.PortName };
        DevicesList.Add(num2);
      }

      DevicesList.Add(num222);
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

    }
  }   
}
