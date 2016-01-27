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

namespace BioModule.ViewModels
{
  public class LocationAccessDevicesViewModel : Screen
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

      _bioEngine.Database().DataChanged += LocationAccessDevicesViewModel_DataChanged;

      AccessDeviceList accessDevices = _bioEngine.Database().AccessDevices;
     // OnAccessDevicesChanged(accessDevices);
    }

    protected async override void OnActivate()
    {
      await _bioService.DatabaseService.LocationRequest    (new CommandLocation());   
      await _bioService.DatabaseService.AccessDeviceRequest(new CommandAccessDevice());      
    }

    public void LocationAccessDevicesViewModel_DataChanged( object sender, EventArgs args)
    {       
       OnAccessDevicesChanged(_bioEngine.Database().AccessDevices);
    }

    public void AddToGeneralDeviceList(DragableItem item, bool isEnabled = true)
    {
      if (item == null)
        return;

      DragableItem newItem = item.Clone();
      newItem.ItemEnabled = isEnabled;
      DevicesList.Add(newItem);
    }

    private void OnAccessDevicesChanged( AccessDeviceList accessDevices )
    {
      DevicesInList.Clear();
      DevicesOutList.Clear();
      DevicesList.Clear();

      if (_location == null)
        return;

      foreach (AccessDevice item in accessDevices.AccessDevices)
      {
        if (item.Locationid != _location.Id)
          continue;

        DragableItem dragableItem = new DragableItem() { ItemContext = item, ItemEnabled = true, DisplayName = item.Portname };

        if (DevicesList.ContainsItem(dragableItem))        
          continue;        

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
          /*
          case AccessDevice.Types.AccessDeviceType.DeviceNone:
           // DevicesList.Add(dragableItem);
           *  AddToGeneralDeviceList(dragableItem, true);
            break; */
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
      OnAccessDevicesChanged(_bioEngine.Database().AccessDevices);
    }

    private          Location          _location  ;
    private readonly IProcessorLocator _locator   ;
    private readonly IBioEngine        _bioEngine ;
    private readonly IServiceManager   _bioService;

  }   
}
