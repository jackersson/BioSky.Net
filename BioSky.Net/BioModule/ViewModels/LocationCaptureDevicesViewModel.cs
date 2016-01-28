using BioData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioModule.DragDrop;
using BioContracts;
using BioFaceService;

namespace BioModule.ViewModels
{
  public class LocationCaptureDevicesViewModel : Screen
  {
    public LocationCaptureDevicesViewModel(IProcessorLocator locator)
    {
      DisplayName = "Capture Devices";

      _locator    = locator;
      _bioService = _locator.GetProcessor<IServiceManager>();
      _bioEngine  = _locator.GetProcessor<IBioEngine>();

      DragableWithDisabledItem disabledDragable = new DragableWithDisabledItem();
      DragableWithRemoveItem   removeDragable   = new DragableWithRemoveItem  ();

      DevicesList   = new DragablListBoxViewModel(disabledDragable);
      DevicesInList = new DragablListBoxViewModel(removeDragable);
      DevicesInList.ItemRemoved += DevicesList.ItemDropped;

      _bioEngine.Database().CaptureDevicesChanged += LocationCaptureDevicesViewModel_DataChanged;         
    }

    protected async override void OnActivate()
    {
      await _bioService.DatabaseService.CaptureDeviceRequest(new CommandCaptureDevice());
    }

    public void LocationCaptureDevicesViewModel_DataChanged(object sender, EventArgs args)
    {
      OnCaptureDevicesChanged(_bioEngine.Database().CaptureDevices);
    }
    public void AddToGeneralDeviceList(DragableItem item, bool isEnabled = true)
    {
      if (item == null)
        return;

      DragableItem newItem = item.Clone();
      newItem.ItemEnabled = isEnabled;
      DevicesList.Add(newItem);
    }
    private void OnCaptureDevicesChanged(CaptureDeviceList captureDevices)
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
    }

    private          Location          _location  ;
    private readonly IProcessorLocator _locator   ;
    private readonly IBioEngine        _bioEngine ;
    private readonly IServiceManager   _bioService;
  }
}
