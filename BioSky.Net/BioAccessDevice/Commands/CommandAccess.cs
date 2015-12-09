using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioAccessDevice.Abstract;
using BioAccessDevice.Interfaces;
using System.IO.Ports;
using System.Threading;

namespace BioAccessDevice.Commands
{
  public class CommandAccess : AccessDeviceCommand
  {
    

    public CommandAccess(ref SerialPort serialPort)
                        : base(ref serialPort)
    {
      _command  = new byte[] { (byte)AccessDeviceCommandID.WRITE_PORT_B, 0, 0, 8, 0, 8, 10 };
      _response = _command;
    }

    public override bool Execute()
    {
      if (!_serialPort.IsOpen)
        return false;

      try
      {        
        _serialPort.Write(_command, _command.Length, 0);

        Thread.Sleep(50);

        byte[] response = { };
        _serialPort.Read(_response, _command.Length, 0);

        return true;

      }
      catch
      {
        return false;
      }
    
    }

  }
}
