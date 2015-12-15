using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioAccessDevice.Abstract;

namespace BioAccessDevice.Commands
{
  public class CommandReady : AccessDeviceCommand
  {
    public CommandReady() : base()
    {
      _command = new byte[] { (byte)AccessDeviceCommandID.WRITE_PORT_B, 0, 0, 0, 2, 8, 4 };
      _actualResponse = new byte[_command.Length];
    }
  }
}
