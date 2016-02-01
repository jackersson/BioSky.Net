using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BioContracts
{
  public interface IAccessDeviceEngine
  {   
    void Add(string portName);

    void Remove(string portName);

    string[] GetPortNames();

    AsyncObservableCollection<string> GetAccessDevicesNames();

    bool AccessDeviceActive(string portName);

    bool HasObserver(IObserver<AccessDeviceActivity> observer, string portName);

    void Execute(AccessDeviceCommands command, string portName);

    void Subscribe(System.IObserver<AccessDeviceActivity> observer, string portName);

    void Unsubscribe(System.IObserver<AccessDeviceActivity> observer, string portName);
    void Stop();
  }
}
