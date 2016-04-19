using AForge.Video.DirectShow;
using BioContracts.Locations;
using System.Drawing;

namespace BioContracts.CaptureDevices
{
  public interface ICaptureDeviceObserver
  {
    void OnFrame(ref Bitmap frame);

    void OnStop( bool stopped, string message, LocationDevice device );

    void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all);    
  }
}
