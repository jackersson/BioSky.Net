namespace BioContracts
{
  public interface IDeviceConnectivity<T>
  {
    bool IsDeviceConnected(string name);
    T GetDeviceInfo(string deviceName);
  }
}
