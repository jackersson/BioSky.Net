using BioContracts.FingerprintDevices;

namespace BioFingerprintDevices.Common
{
  public class FutronicDevicesInfo
  {
    private static volatile FutronicDevicesInfo _instance;
    public static FutronicDevicesInfo Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (syncObject)
          {
            if (_instance == null)
              _instance = new FutronicDevicesInfo();
          }
        }
        return _instance;
      }
    }
    
    private FutronicDevicesInfo() { }
 
    public FingerprintDeviceInfo GetScannerInfoByCompatibility(int deviceCompatibility)
    {
      FingerprintDeviceInfo info = new FingerprintDeviceInfo() { LiveCheck = false };

      switch (deviceCompatibility)
      {
        case 0:
        case 1:
        case 4:
        case 11:
          info.Name = "FS80";
          info.LiveCheck = true;
          break;
        case 5:
          info.Name = "FS88";
          info.LiveCheck = true;
          break;
        case 7:
          info.Name = "FS50";
          break;
        case 8:
          info.Name = "FS60";
          break;
        case 9:
          info.Name = "FS25";
          info.LiveCheck = true;
          break;
        case 10:
          info.Name = "FS10";
          break;
        case 13:
          info.Name = "FS80H";
          info.LiveCheck = true;
          break;
        case 14:
          info.Name = "FS88H";
          info.LiveCheck = true;
          break;
        case 15:
          info.Name = "FS64";
          break;
        case 16:
          info.Name = "FS26E";
          break;
        case 17:
          info.Name = "FS80HS";
          break;
        case 18:
          info.Name = "FS26";
          break;
        default:
          info.Name = "Unknown device";
          break;
      }
      return info;
    }

    private static object syncObject = new object();
  }
}
