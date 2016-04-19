using BioContracts.Locations;
using System;

namespace BioContracts
{
  public interface IAccessDeviceObserver
  {
    void OnCardDetected(string cardNumber);

    void OnError(Exception ex, LocationDevice device);

    void OnReady(bool isReady);
  }
}
