using ScanAPIHelper;

namespace BioFingerprintDevices.Common
{
  public class FingerprintDeviceErrorInfo
  {
    private static volatile FingerprintDeviceErrorInfo _instance;
    public static FingerprintDeviceErrorInfo Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (syncObject)
          {
            if (_instance == null)
              _instance = new FingerprintDeviceErrorInfo();
          }
        }
        return _instance;
      }
    }

    private FingerprintDeviceErrorInfo() { }
    public string GetErrorMessage(ScanAPIException exeption) {  return GetErrorMessage(exeption.ErrorCode); }

    public string GetErrorMessage(int error)
    {
      string szMessage;
      switch (error)
      {
        case FTR_ERROR_EMPTY_FRAME:
          szMessage = "Touch scanner, please";
          break;

        case FTR_ERROR_MOVABLE_FINGER:
          szMessage = "Stop any finger movements";
          break;

        case FTR_ERROR_NO_FRAME:
          szMessage = "Fake Finger detected";
          break;

        case FTR_ERROR_HARDWARE_INCOMPATIBLE:
          szMessage = "Incompatible Hardware";
          break;

        case FTR_ERROR_FIRMWARE_INCOMPATIBLE:
          szMessage = "Incompatible Firmware";
          break;

        case FTR_ERROR_INVALID_AUTHORIZATION_CODE:
          szMessage = "Invalid Authorization Code";
          break;

        case ERROR_NOT_ENOUGH_MEMORY:
          szMessage = "Error code ERROR_NOT_ENOUGH_MEMORY";
          break;

        case ERROR_NO_SYSTEM_RESOURCES:
          szMessage = "Error code ERROR_NO_SYSTEM_RESOURCES";
          break;

        case ERROR_TIMEOUT:
          szMessage = "Error code ERROR_TIMEOUT";
          break;

        case ERROR_NOT_READY:
          szMessage = "Error code ERROR_NOT_READY";
          break;

        case ERROR_BAD_CONFIGURATION:
          szMessage = "Error code ERROR_BAD_CONFIGURATION";
          break;

        case ERROR_INVALID_PARAMETER:
          szMessage = "Error code ERROR_INVALID_PARAMETER";
          break;

        case ERROR_CALL_NOT_IMPLEMENTED:
          szMessage = "Error code ERROR_CALL_NOT_IMPLEMENTED";
          break;

        case ERROR_NOT_SUPPORTED:
          szMessage = "Error code ERROR_NOT_SUPPORTED";
          break;

        case ERROR_WRITE_PROTECT:
          szMessage = "Error code ERROR_WRITE_PROTECT";
          break;

        case ERROR_MESSAGE_EXCEEDS_MAX_SIZE:
          szMessage = "Error code ERROR_MESSAGE_EXCEEDS_MAX_SIZE";
          break;

        case ERROR_DISCONNECTED:
          szMessage = "Error code ERROR_DISCONNECTED";
          break;

        default:
          szMessage = string.Format("Error code: {0}", error);
          break;
      }
      return szMessage;
    }

    public const int FTR_ERROR_EMPTY_FRAME                = 4306      ; /* ERROR_EMPTY */
    public const int FTR_ERROR_MOVABLE_FINGER             = 0x20000001;
    public const int FTR_ERROR_NO_FRAME                   = 0x20000002;
    public const int FTR_ERROR_USER_CANCELED              = 0x20000003;
    public const int FTR_ERROR_HARDWARE_INCOMPATIBLE      = 0x20000004;
    public const int FTR_ERROR_FIRMWARE_INCOMPATIBLE      = 0x20000005;
    public const int FTR_ERROR_INVALID_AUTHORIZATION_CODE = 0x20000006;



    public const int ERROR_NO_MORE_ITEMS             = 259 ; // ERROR_NO_MORE_ITEMS
    public const int ERROR_NOT_ENOUGH_MEMORY         = 8   ; // ERROR_NOT_ENOUGH_MEMORY
    public const int ERROR_NO_SYSTEM_RESOURCES       = 1450; // ERROR_NO_SYSTEM_RESOURCES
    public const int ERROR_TIMEOUT                   = 1460; // ERROR_TIMEOUT
    public const int ERROR_NOT_READY                 = 21  ; // ERROR_NOT_READY
    public const int ERROR_BAD_CONFIGURATION         = 1610; // ERROR_BAD_CONFIGURATION
    public const int ERROR_INVALID_PARAMETER         = 87  ; // ERROR_INVALID_PARAMETER
    public const int ERROR_CALL_NOT_IMPLEMENTED      = 120 ; // ERROR_CALL_NOT_IMPLEMENTED
    public const int ERROR_NOT_SUPPORTED             = 50  ; // ERROR_NOT_SUPPORTED
    public const int ERROR_WRITE_PROTECT             = 19  ; // ERROR_WRITE_PROTECT
    public const int ERROR_MESSAGE_EXCEEDS_MAX_SIZE  = 4336; // ERROR_MESSAGE_EXCEEDS_MAX_SIZE

    public const int ERROR_DISCONNECTED = 2;
    public const int DEVICE_OK          = 0;

    private static object syncObject = new object();
  }
}
