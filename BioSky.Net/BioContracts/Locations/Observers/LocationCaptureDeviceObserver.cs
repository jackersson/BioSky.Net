using BioContracts.CaptureDevices;
using BioService;

namespace BioContracts.Locations.Observers
{
  public class LocationCaptureDeviceObserver : ILocationDeviceObserver
  {
    public LocationCaptureDeviceObserver(IProcessorLocator locator, ICaptureDeviceObserver observer)
    {
      _observer = observer;

      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
    }

    public void Stop()
    {
      _captureDeviceEngine.Unsubscribe(_observer);
      _deviceName = null;
    }

    public void Start(string deviceName)
    {
      if (_deviceName == deviceName)
        return;

      if (string.IsNullOrEmpty(deviceName))
        return;      

      _deviceName = deviceName;
      _captureDeviceEngine.Subscribe(_observer, _deviceName);
    }

    public void Start(Location location)
    {
      _location = location;

      if (location == null)
        return;

      if (location.CaptureDevice != null)
        Start(location.CaptureDevice.Devicename);
    }

    public bool IsDeviceOk
    {
      get { return _captureDeviceEngine.IsDeviceActive(_deviceName); }
    }

    public string DeviceName
    {
      get { return _deviceName; }
    }

    private string _deviceName;
    private Location _location;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly ICaptureDeviceObserver _observer;
  }

}
