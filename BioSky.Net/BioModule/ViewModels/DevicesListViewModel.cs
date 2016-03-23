
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
      _accessDevices  = new LocationAccessDevicesViewModel(locator);
      _captureDevices = new LocationCaptureDevicesViewModel(locator);
    }

    protected override void OnActivate()
    {
      ActivateItem(_accessDevices);
      ActivateItem(_captureDevices);

      _captureDevices.DeviceChanged += DevicesChanged;
      _accessDevices .DeviceChanged += DevicesChanged;
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
      
      DeactivateItem(_accessDevices, false);
      DeactivateItem(_captureDevices, false);

      base.OnDeactivate(close);
    }

    public void Update(Location location)
    {
      _accessDevices.Update(location);
      _captureDevices.Update(location);
    }

    private void OnAnyDeviceChanged()
    {
      if (AnyDeviceChanged != null)
        AnyDeviceChanged(null, EventArgs.Empty);
    }

    public bool DeviceChanged
    {
      get { return _captureDevices.IsDeviceChanged || _accessDevices.IsDeviceChanged; }
    }

    public bool CanApply
    {
      get { return !string.IsNullOrEmpty( _captureDevices.DesiredCaptureDeviceName) 
                || !string.IsNullOrEmpty( _accessDevices.DesiredCaptureDeviceName); }
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

    public event EventHandler AnyDeviceChanged;
  }
}
