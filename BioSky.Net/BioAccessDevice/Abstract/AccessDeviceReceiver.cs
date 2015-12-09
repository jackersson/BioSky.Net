using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioAccessDevice.Interfaces;
using System.IO.Ports;
using System.Threading;

namespace BioAccessDevice.Abstract
{
  public abstract class AccessDeviceReceiver : IReceiver
  {

    public AccessDeviceReceiver( ref SerialPort serialPort )
    {
      this._serialPort = serialPort;
    }

    public void Action( )
    {
      if (!_serialPort.IsOpen)
        return;

      byte[] command = {1};
      _serialPort.Write(command, 0, 0);

      Thread.Sleep(50);

      byte[] response = {};
      _serialPort.Read(response, 0, 0);
    }

    protected SerialPort _serialPort;
  }
}
