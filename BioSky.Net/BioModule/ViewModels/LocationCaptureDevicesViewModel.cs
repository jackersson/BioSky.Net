using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioModule.Utils;
using BioService;
using Caliburn.Micro;
using Google.Protobuf.Collections;
using BioContracts;

namespace BioModule.ViewModels
{
  public class LocationCaptureDevicesViewModel : Screen, IUpdatable
  {
    public LocationCaptureDevicesViewModel(IProcessorLocator locator)
    {
      _locator = locator;

      _bioEngine = _locator.GetProcessor<IBioEngine>();

      _captureDevicesList = new AsyncObservableCollection<CaptureDeviceItem>();

      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();
    }

    #region Update
    public void RefreshConnectedDevices()
    {
      foreach (string deviceName in CaptureDevicesNames)
      {
        if (!IsDeviceUsed(deviceName))
        {
          CaptureDevice device = new CaptureDevice() { Devicename = deviceName };
          CaptureDeviceItem deviceItem = new CaptureDeviceItem()
          {
            ItemContext = device
          , ItemActive = false
          , ItemEnabled = true
          };
          CaptureDevicesList.Add(deviceItem);
        }
      }
    }

    public void RefreshData()
    {
      CaptureDevicesList.Clear();

      foreach (CaptureDevice item in _bioEngine.Database().CaptureDeviceHolder.Data)
      {
        bool state = (_location.Id == item.Locationid);
        CaptureDeviceItem deviceItem = new CaptureDeviceItem()
        {
          ItemContext = item
        , ItemActive = state
        , ItemEnabled = state
        };

        CaptureDevicesList.Add(deviceItem);
      }

      RefreshConnectedDevices();
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
    public void Update(Location location)
    {
      _location = location;
      RefreshData();
    }
    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshConnectedDevices();
    }
    #endregion

    #region Interface
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
    public void Apply() { }
    #endregion

    #region BioService

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

    #endregion

    #region Global Variables

    private Location _location;
    private readonly IProcessorLocator _locator;
    private readonly IBioEngine _bioEngine;

    #endregion
  }
  public class CaptureDeviceItem : DeviceItemBase<CaptureDevice> { }

}
