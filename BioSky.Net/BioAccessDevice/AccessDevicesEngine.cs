using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BioContracts;
using BioAccessDevice.Interfaces;

namespace BioAccessDevice
{
  public class AccessDevicesEngine : IAccessDeviceEngine
  {
    public AccessDevicesEngine()
    {
      _accessDevices = new Dictionary<string, AccessDeviceListener>();     
    }

    public void Add( string portName)
    {
      AccessDeviceListener listener;
      if (  !_accessDevices.TryGetValue(portName, out listener) )
      {
        listener = new AccessDeviceListener(portName);
        listener.Start();       
        _accessDevices.Add(portName, listener);
      }
    }

    public void Remove(string portName)
    {
      AccessDeviceListener listener;
      if (_accessDevices.TryGetValue(portName, out listener))            
        _accessDevices.Remove(portName);      
    }

    public string[] GetPortNames()
    {
      return SerialPort.GetPortNames();
    }


    public void Execute(AccessDeviceCommands command, string portName)
    {
      AccessDeviceListener listener;
      if (_accessDevices.TryGetValue(portName, out listener))
      {        
        listener.Enqueque(command.ToString());
      }
    }

    public void Subscribe( IObserver<AccessDeviceActivity> observer, string portName)
    {
      AccessDeviceListener listener;
      if (_accessDevices.TryGetValue(portName, out listener))
      {
        listener.Subscribe(observer);
      }
    }

    private Dictionary<string, AccessDeviceListener> _accessDevices;    
  }
}
