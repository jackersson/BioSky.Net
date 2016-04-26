using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.IrisDevices
{
  public interface IIrisDeviceEngine
  {
    void RemoveAll();
    void Add(string deviceName);
    void Capture(string deviceName);
    void Remove(string deviceName);
    bool IsDeviceActive(string deviceName);
    void Subscribe(IIrisDeviceObserver observer, string deviceName);
    void Unsubscribe(IIrisDeviceObserver observer);
    bool HasObserver(IIrisDeviceObserver observer, string deviceName);
    void UpdateFromSet(ICollection<string> devices);
    AsyncObservableCollection<string> GetDevicesNames();
  }
}
