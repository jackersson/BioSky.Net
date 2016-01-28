﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BioContracts
{
  public interface IBioEngine
  {
    void Stop();

    IBioSkyNetRepository Database();

    IAccessDeviceEngine AccessDeviceEngine();

    ICaptureDeviceEngine CaptureDeviceEngine();

    ITrackLocationEngine TrackLocationEngine();
  }
}
