using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BioAccessDevice
{

  public enum AccessDeviceCommands
  {
      CMD_Access = 0
    , CMD_Deny
    , CMD_Light
    , CMD_Reset
    , CMD_Ready
  }

  public class AccessDevicesEngine
  {
    public AccessDevicesEngine()
    {

    }

    public void Add( string comPortName)
    {
      AccessDeviceListener listener;
      if (  !_accessDevices.TryGetValue(comPortName, out listener) )
      {
        listener = new AccessDeviceListener();

        //ThreadPool.QueueUserWorkItem(new WaitCallback(), listener);
        _accessDevices.Add(comPortName, listener);
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

    }


    private Dictionary<string, AccessDeviceListener> _accessDevices;

  }
}
