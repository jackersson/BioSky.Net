using BioContracts.CaptureDevices;
using BioContracts.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BioContracts.CaptureDevices
{
  public interface ICaptureDeviceEngine
  {
    void RemoveAll();
    void Add(string deviceName);
    void Remove(string deviceName);
    bool IsDeviceActive(string deviceName);    
    void Subscribe  (ICaptureDeviceObserver observer, string deviceName);
    void Unsubscribe(ICaptureDeviceObserver observer);
    bool HasObserver(ICaptureDeviceObserver observer, string deviceName);
    void ApplyResolution(string deviceName, int resolutionIndex);

    void ApplyProperties(string deviceName, IntPtr parentWindow );
    
    void UpdateFromSet( HashSet<string> devices );
    
    AsyncObservableCollection<string> GetDevicesNames();
    
  }
}
