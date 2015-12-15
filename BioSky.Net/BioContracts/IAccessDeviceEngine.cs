using System;
using System.Collections.Generic;
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

    void Execute(AccessDeviceCommands command, string portName);

    void Subscribe(System.IObserver<AccessDeviceActivity> observer, string portName);    
  }
}
