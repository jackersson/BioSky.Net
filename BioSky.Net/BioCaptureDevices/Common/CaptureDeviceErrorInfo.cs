using AForge.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioCaptureDevices.Common
{
  public class CaptureDeviceErrorInfo
  {
    private static volatile CaptureDeviceErrorInfo _instance;
    public static CaptureDeviceErrorInfo Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (syncObject)
          {
            if (_instance == null)
              _instance = new CaptureDeviceErrorInfo();
          }
        }
        return _instance;
      }
    }

    private CaptureDeviceErrorInfo() { }

    public string GetErrorMessage(ReasonToFinishPlaying exeption) { return GetErrorMessage((int)exeption); }

    public string GetErrorMessage(int error)
    {
      string szMessage;
      switch (error)
      {
        case END_OF_STREAM_REACHED:
          szMessage = "End of stream reached";
          break;

        case STOPED_BY_USER:
          szMessage = "Stoped by user";
          break;

        case DEVICE_LOST:
          szMessage = "Device lost";
          break;

        case VIDEO_SOURCE_ERROR:
          szMessage = "Video source error";
          break;

        default:
          szMessage = string.Format("Error code: {0}", error);
          break;
      }
      return szMessage;
    }


    public const int END_OF_STREAM_REACHED = 0;
    public const int STOPED_BY_USER        = 1;
    public const int DEVICE_LOST           = 2;
    public const int VIDEO_SOURCE_ERROR    = 3;

    private static object syncObject = new object();
  }
}
