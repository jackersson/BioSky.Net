using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Common
{
  public enum DevicesInfoEnum
  {
     CONNECTING_TO_DEVICE = 0
   , CANNOT_UPLOAD_PHOTO  = 1
   , ENROLLMENT_STARTED   = 2
   , ENROLLMENT_FINISHED  = 3
  }
  public class DevicesInfo
  {
    private static volatile DevicesInfo _instance;
    public static DevicesInfo Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (syncObject)
          {
            if (_instance == null)
              _instance = new DevicesInfo();
          }
        }
        return _instance;
      }
    }

    private DevicesInfo() { }

    public string GetErrorMessage(DevicesInfoEnum exeption) { return GetErrorMessage((int)exeption); }

    public string GetErrorMessage(int error)
    {
      string szMessage;
      switch (error)
      {
        case CONNECTING_TO_DEVICE:
          szMessage = "Connecting to Device...";
          break;

        case CANNOT_UPLOAD_PHOTO:
          szMessage = "Cannot uplodad Photo";
          break;

        case ENROLLMENT_STARTED:
          szMessage = "Enrollment started";
          break;

        case ENROLLMENT_FINISHED:
          szMessage = "Enrollment finished";
          break;

        default:
          szMessage = string.Format("Error code: {0}", error);
          break;
      }
      return szMessage;
    }


    public const int CONNECTING_TO_DEVICE = 0;
    public const int CANNOT_UPLOAD_PHOTO  = 1;
    public const int ENROLLMENT_STARTED   = 2;
    public const int ENROLLMENT_FINISHED  = 3;

    private static object syncObject = new object();
  }
}
