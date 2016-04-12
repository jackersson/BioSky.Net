﻿using AForge.Video.DirectShow;
using System.Drawing;

namespace BioContracts.CaptureDevices
{
  public interface ICaptureDeviceObserver
  {
    void OnFrame(ref Bitmap frame);

    void OnStop( bool stopped, string message );

    void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all);    
  }
}