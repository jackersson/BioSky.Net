using BioContracts.AccessDevices;
using BioService;

namespace BioContracts.Locations.Observers
{
  public class LocationAccessDeviceObserver : ILocationDeviceObserver
  {
    public LocationAccessDeviceObserver(IProcessorLocator locator, IAccessDeviceObserver observer)
    {
      _observer = observer;

      _accessDeviceEngine = locator.GetProcessor<IAccessDeviceEngine>();
    }
    public void Stop()
    {
      _accessDeviceEngine.Unsubscribe(_observer);
      _deviceName = null;
    }

    public void Start(string deviceName)
    {
      if (_deviceName == deviceName)
        return;

      if (string.IsNullOrEmpty(deviceName))
        return;

      _deviceName = deviceName;

      _accessDeviceEngine.Subscribe(_observer, _deviceName);
    }

    public void Start(Location location)
    {
      _location = location;

      if (location == null)
        return;

      if (location.AccessDevice != null)
        Start(location.AccessDevice.Portname);
    }    

    public bool IsDeviceOk {
      get { return _accessDeviceEngine.IsDeviceActive(_deviceName); }
    }

    public string DeviceName
    {
      get { return _deviceName; }
    }

    private string _deviceName;
    private Location _location;
    private readonly IAccessDeviceEngine _accessDeviceEngine;
    private readonly IAccessDeviceObserver _observer;
  }
}
