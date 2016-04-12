using BioService;

namespace BioContracts.Locations
{
  public enum LocationDevice
  {
      Card
    , AccessDevice
    , CaptureDevice
    , FingerprintDevice
    , IrisDevice
  }
  public interface ILocationDeviceObserver
  {
    void Stop();
    void Start(Location location);
    bool IsDeviceOk { get; }
  }
}
