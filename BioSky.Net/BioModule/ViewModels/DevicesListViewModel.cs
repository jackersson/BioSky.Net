
using BioContracts;
using BioModule.Utils;
using BioService;
using Caliburn.Micro;
using System;

namespace BioModule.ViewModels
{
  public class DevicesListViewModel : Conductor<IScreen>.Collection.AllActive, IUpdatable
  {
    public DevicesListViewModel(IProcessorLocator locator)
    {
      _accessDevices  = new LocationAccessDevicesViewModel (locator);
      _captureDevices = new LocationCaptureDevicesViewModel(locator);
      _fingerDevices  = new LocationFingerDevicesViewModel (locator);
    }

    protected override void OnActivate()
    {
      _captureDevices.DeviceChanged += DevicesChanged;
      _accessDevices .DeviceChanged += DevicesChanged;
      _fingerDevices .DeviceChanged += DevicesChanged;
      base.OnActivate();
    }

    private void DevicesChanged(object sender, EventArgs e)
    {       
      NotifyOfPropertyChange(() => DeviceChanged);
    } 

    protected override void OnDeactivate(bool close)
    {
      _captureDevices.DeviceChanged -= DevicesChanged;
      _accessDevices .DeviceChanged -= DevicesChanged;
      _fingerDevices .DeviceChanged -= DevicesChanged;

      base.OnDeactivate(close);
    }

    public void Update(Location location)
    {
      _accessDevices .Update(location);
      _captureDevices.Update(location);
      _fingerDevices .Update(location);
    }

    private void OnAnyDeviceChanged()
    {
      if (AnyDeviceChanged != null)
        AnyDeviceChanged(null, EventArgs.Empty);
    }

    public bool DeviceChanged
    {
      get { return _captureDevices.IsDeviceChanged || _accessDevices.IsDeviceChanged || _fingerDevices.IsDeviceChanged; }
    }

    public bool CanApply
    {
      get { return !string.IsNullOrEmpty( _captureDevices.DesiredDeviceName) 
                || !string.IsNullOrEmpty( _accessDevices.DesiredDeviceName)
                || !string.IsNullOrEmpty( _fingerDevices.DesiredDeviceName); }
    }

    public void Apply() { }

    private readonly LocationAccessDevicesViewModel _accessDevices;
    public LocationAccessDevicesViewModel AccessDevices
    {
      get { return _accessDevices; }
    }

    private readonly LocationCaptureDevicesViewModel _captureDevices;
    public LocationCaptureDevicesViewModel CaptureDevices
    {
      get { return _captureDevices; }
    }

    private readonly LocationFingerDevicesViewModel _fingerDevices;
    public LocationFingerDevicesViewModel FingerDevices
    {
      get { return _fingerDevices; }
    }

    public event EventHandler AnyDeviceChanged;
  }
}
