using System;
using System.IO.Ports;

namespace BioAccessDevice.Interfaces
{
  public interface ICommand
  { 
    bool   Execute( ref SerialPort serialPort );
    byte[] Message();

    Exception ErrorMessage();
  }
}
