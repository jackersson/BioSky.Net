using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioEngine.CaptureDevices
{
  public interface ICaptureDeviceConnectivity
  {
    FilterInfo CaptureDeviceConnected(string deviceName);
  }
}
