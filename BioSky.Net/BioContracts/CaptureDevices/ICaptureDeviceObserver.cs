using AForge.Video.DirectShow;
using BioContracts.Locations;
using System;
using System.Drawing;

namespace BioContracts.CaptureDevices
{
  public interface ICaptureDeviceObserver
  {
    void OnFrame(ref Bitmap frame);

    void OnStop( bool stopped, Exception message, LocationDevice device );

    void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all);

    void OnMessage(string message);   
  }
}
