using BioContracts;
using BioContracts.Common;
using BioModule.ResourcesLoader;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioModule.ViewModels
{  
  public class CardEnrollmentBarViewModel : Screen
  { 
    public CardEnrollmentBarViewModel( IProcessorLocator locator )
    {
      _deviceEngine = locator.GetProcessor<IAccessDeviceEngine>();

      DevicesNames = _deviceEngine.GetAccessDevicesNames();

      DeviceObserver = new TrackLocationAccessDeviceObserver(locator);
      
      DeviceConnected = false;
    }

    private void DevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }

    public string AvaliableDevicesCount
    {
      get { return String.Format("Available Devices ({0})", _devicesNames.Count); }
    }

    protected override void OnActivate()
    {
      DevicesNames = _deviceEngine.GetAccessDevicesNames();

      DevicesNames.CollectionChanged   += DevicesNames_CollectionChanged;
      DeviceObserver.AccessDeviceState += DeviceObserver_AccessDeviceState;
  
      base.OnActivate();
    }    

    protected override void OnDeactivate( bool close )
    {      
      DevicesNames.CollectionChanged   -= DevicesNames_CollectionChanged;
      DeviceObserver.AccessDeviceState -= DeviceObserver_AccessDeviceState;
      base.OnDeactivate(close);
    }

    private void DeviceObserver_AccessDeviceState(bool status)
    {
      DeviceConnected = status;
    }
    
    private TrackLocationAccessDeviceObserver _deviceObserver;
    public TrackLocationAccessDeviceObserver DeviceObserver
    {
      get { return _deviceObserver; }
      private set
      {
        if (_deviceObserver != value)        
          _deviceObserver = value;            
      }
    }

    private AsyncObservableCollection<string> _devicesNames;
    public AsyncObservableCollection<string> DevicesNames
    {
      get { return _devicesNames; }
      set
      {
        if (_devicesNames != value)
        {
          _devicesNames = value;
          NotifyOfPropertyChange(() => DevicesNames);
        }
      }
    }

    public BitmapSource DeviceConnectedIcon
    {
      get { return DeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }

    private string _selectedDevice;
    public string SelectedDevice
    {
      get { return _selectedDevice; }
      set
      {
        if (_selectedDevice != value)
        {
          _selectedDevice = value;

          DeviceObserver.Update(_selectedDevice);
          NotifyOfPropertyChange(() => SelectedDevice);
          //Subscribe();
        }
      }
    }

    private bool _deviceConnected;
    public bool DeviceConnected
    {
      get { return _deviceConnected; }
      set
      {
        if (_deviceConnected != value)
        {
          _deviceConnected = value;
          NotifyOfPropertyChange(() => DeviceConnectedIcon);
        }
      }
    }

    private readonly IAccessDeviceEngine _deviceEngine;
  }
}
