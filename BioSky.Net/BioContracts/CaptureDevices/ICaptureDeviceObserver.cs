using AForge.Video.DirectShow;
using BioContracts.Common;
using BioContracts.Locations;
using System;
using System.Drawing;

namespace BioContracts.CaptureDevices
{
  public enum CaptureDeviceFinishPlayingEnum
  {
      EndOfStreamReached = 0
    , StoppedByUser      = 1
    , DeviceLost         = 2
    , VideoSourceError   = 3
  }
  public interface ICaptureDeviceObserver
  {
    void OnFrame(ref Bitmap frame);

    void OnStop( bool stopped, ErrorMessage message, LocationDevice device );

    void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all);

    void OnMessage(string message);   
  }
}
