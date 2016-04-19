using BioContracts.BioTasks;
using System;
using System.Drawing;

namespace BioContracts.Locations
{
  public interface IFullLocationObserver : IVerificationObserver
  {
    void OnError(Exception ex, LocationDevice device);

    void OnOk(bool ok);

    void OnCaptureDeviceFrameChanged(ref Bitmap frame);

  }
}
