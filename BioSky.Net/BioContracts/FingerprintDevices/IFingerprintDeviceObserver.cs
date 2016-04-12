using System;
using System.Drawing;

namespace BioContracts.FingerprintDevices
{
  public interface IFingerprintDeviceObserver
  {
    void OnFrame(ref Bitmap frame);

    void OnError(Exception ex);

    void OnMessage(string message);

    void OnReady(bool isReady);
  }
}
