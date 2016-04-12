using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.FingerprintDevices
{
  public interface IFingerprintDeviceEngine
  {

    void Stop();
    void Add(string deviceName);
    void Remove(string deviceName);
    bool IsDeviceActive(string deviceName);
    void Subscribe  (IFingerprintDeviceObserver observer, string deviceName);
    void Unsubscribe(IFingerprintDeviceObserver observer);
    bool HasObserver(IFingerprintDeviceObserver observer, string deviceName);
 
    AsyncObservableCollection<FingerprintDeviceInfo> GetDevicesNames();

    
  }
}
