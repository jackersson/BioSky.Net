using System;
using System.Collections.Generic;
using System.Drawing;

namespace BioContracts.IrisDevices
{
  public enum CaptureState
  {
     Complete
   , Abort
   , Capturing
  }

  public interface IIrisDeviceObserver
  {
    void OnFrame( Bitmap left,  Bitmap right);

    void OnIrisQualities(IList<EyeScore> scores);

    void OnEyesDetected(bool detected);

    void OnState(CaptureState captureState);

    void OnError(Exception ex);

    void OnMessage(string message);

    void OnReady(bool isReady);
  }
}
