using System;
using BioAccessDevice.Interfaces;
using System.IO.Ports;
using System.Threading;

namespace BioAccessDevice.Abstract
{
  public enum AccessDeviceCommandID : int
  {
      READ_PORT_B     = 129
    , WRITE_PORT_B    = 130
    , READ_PORT_C     = 131
    , READ_DALLAS_KEY = 132
  }

  public class AccessDeviceCommand : ICommand
  {   
    public AccessDeviceCommand()   {     
      _utils = new SerialPortUtils();
    }

    public bool Execute(ref SerialPort serialPort)
    {
      _exception = null;

      if (!serialPort.IsOpen)
      {
        _exception = new System.IO.IOException("COM port not Opened");
        return false;
      }

      _response = null;

      try
      {               
        serialPort.Write(_command, 0, _command.Length);

        Thread.Sleep(WRITE_READ_DELAY);
        
        int  value   = 0;
        int  timeout = 0;      
        bool commandDetected = false;

        while ( !commandDetected || timeout < ACCESS_DEVICE_READ_TIMEOUT)
        {
          value = serialPort.ReadByte();
         
          commandDetected = Enum.IsDefined(typeof(AccessDeviceCommandID), value);
          if (commandDetected)
          {
            _actualResponse[0] = (byte)value;
            serialPort.Read(_actualResponse, 1, _actualResponse.Length - 1);           
            break;
          }
           
          timeout++;
        }      
        return Validate();
      }
      catch (Exception exception)
      {
        _exception = exception;
        return false;
      }
    }

    public Exception ErrorMessage() { return _exception; }

    public byte[] Message() { return _response;    }

    public virtual bool Validate()
    {
      bool checkResponseSumValid = _utils.CheckResponseSum(_actualResponse);
      bool commandEquality = _utils.Compare(_command, _actualResponse);

      return commandEquality && checkResponseSumValid;
    }

    private const short ACCESS_DEVICE_READ_TIMEOUT = 100;
    private const short WRITE_READ_DELAY           = 100;

    protected Exception       _exception     ;
    protected SerialPortUtils _utils         ;
    protected byte[]          _command       ;
    protected byte[]          _targetResponse;
    protected byte[]          _actualResponse;
    protected byte[]          _response      ;

  }
}
