using System;

namespace BioContracts
{
  public interface IAccessDeviceObserver
  {
    void OnCardDetected(string cardNumber);

    void OnError(Exception ex);

    void OnReady(bool isReady);
  }
}
