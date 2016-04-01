using BioContracts.Common;
using System;

namespace BioContracts
{
  public interface ICaptureDeviceEngine
  {
    void Stop();
    void Add(string deviceName);
    void Remove(string deviceName);
    bool IsDeviceActive(string deviceName);    
    void Subscribe  (ICaptureDeviceObserver observer, string deviceName);
    void Unsubscribe(ICaptureDeviceObserver observer);
    bool HasObserver(ICaptureDeviceObserver observer, string deviceName);
    void ApplyResolution(string deviceName, int resolutionIndex);

    void ApplyProperties(string deviceName, IntPtr parentWindow );
    AsyncObservableCollection<string> GetDevicesNames();
    
  }
}
