using BioData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioModule.DragDrop;


namespace BioModule.ViewModels
{
  public class LocationCaptureDevicesViewModel : Screen
  {
    public LocationCaptureDevicesViewModel()
    {
      DisplayName = "Capture Devices";

      DragableWithDisabledItem disabledDragable = new DragableWithDisabledItem();
      DragableWithRemoveItem   removeDragable   = new DragableWithRemoveItem  ();

      DevicesList   = new DragablListBoxViewModel(disabledDragable);
      DevicesInList = new DragablListBoxViewModel(removeDragable);
      DevicesInList.ItemRemoved += DevicesList.ItemDropped;
          
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
  }
}
