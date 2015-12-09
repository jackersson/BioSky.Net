using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioAccessDevice.Interfaces;
using System.IO.Ports;

namespace BioAccessDevice.Abstract
{

  public enum AccessDeviceCommandID : byte
  {
      READ_PORT_B     = 129
    , WRITE_PORT_B    = 130
    , READ_PORT_C     = 131
    , READ_DALLAS_KEY = 132
  }

  public abstract class AccessDeviceCommand : ICommand
  {
    // protected IReceiver _receiver;
    protected SerialPortUtils _utils     ;
    protected SerialPort      _serialPort;
    protected byte[]          _command   ;
    protected byte[]          _response  ;

    // Constructor
    public AccessDeviceCommand( ref SerialPort serialPort )
    {
      this._serialPort = serialPort;
      _utils = new SerialPortUtils();
    }    

    public abstract bool Execute();
  }
}
