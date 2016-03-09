
using BioContracts;
using BioModule.Utils;
using BioService;
using Caliburn.Micro;

namespace BioModule.ViewModels
{
  public class DevicesListViewModel : Conductor<IScreen>.Collection.AllActive, IUpdatable
  {
    public DevicesListViewModel(IProcessorLocator locator)
    {
      _accessDevices = new LocationAccessDevicesViewModel(locator);
      _captureDevices = new LocationCaptureDevicesViewModel(locator);
    }

    protected override void OnActivate()
    {
      ActivateItem(_accessDevices);
      ActivateItem(_captureDevices);
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      DeactivateItem(_accessDevices, false);
      DeactivateItem(_captureDevices, false);
      base.OnDeactivate(close);
    }

    public void Update(Location location)
    {
      _accessDevices.Update(location);
      _captureDevices.Update(location);
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
  }
}
