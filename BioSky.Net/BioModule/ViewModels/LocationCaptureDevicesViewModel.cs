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

      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;
  
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

      foreach (string deviceName in CaptureDevicesNames)
      {
        CaptureDevice device = new CaptureDevice() { Devicename = deviceName, };
        DragableItem dragableItem = new DragableItem() { ItemContext = device, ItemEnabled = true, DisplayName = device.Devicename };
        AddToGeneralDeviceList(dragableItem, true);
      }
    }

    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshData();
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
