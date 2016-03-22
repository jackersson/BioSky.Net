using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BioContracts;
using BioAccessDevice.Interfaces;
using System.Collections.ObjectModel;

namespace BioAccessDevice
{
  public class AccessDevicesEngine : IAccessDeviceEngine
  {
    public AccessDevicesEngine()
    {
      _accessDevices = new Dictionary<string, AccessDeviceListener>();

      _accessDeviceEnumerator = new AccessDevicesEnumerator();
      _accessDeviceEnumerator.Start();
    }

    public void Stop()
    {
      _accessDeviceEnumerator.Stop();
      foreach ( KeyValuePair<string, AccessDeviceListener> par in _accessDevices)      
        par.Value.Stop();       

      _accessDevices.Clear();
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
      {
        listener.Stop();
        _accessDevices.Remove(portName);
      }    
    }

    
    public string[] GetPortNames()
    {
      return SerialPort.GetPortNames();
    }
        

    public bool AccessDeviceActive(string portName)
    {
      AccessDeviceListener listener;
      if (_accessDevices.TryGetValue(portName, out listener))
        return listener.IsActive() ;
      return false;
    }
    
  


    public void Execute(AccessDeviceCommands command, string portName)
    {
      AccessDeviceListener listener;
      if (_accessDevices.TryGetValue(portName, out listener))
      {        
        listener.Enqueque(command);
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

    public void Unsubscribe(System.IObserver<AccessDeviceActivity> observer, string portName)
    {
      
      AccessDeviceListener listener;
      if (_accessDevices.TryGetValue(portName, out listener))
      {
        listener.Unsubscribe(observer);
      }
    }

    public bool HasObserver(IObserver<AccessDeviceActivity> observer, string portName)
    {
      AccessDeviceListener listener;
      if (_accessDevices.TryGetValue(portName, out listener))      
        return listener.HasObserver(observer);
      
      return false;
    }

    private readonly AccessDevicesEnumerator _accessDeviceEnumerator;


    public AsyncObservableCollection<string> GetAccessDevicesNames()
    {
      return _accessDeviceEnumerator.AccessDevicesNames;
    }

    private Dictionary<string, AccessDeviceListener> _accessDevices;    
  }
}
