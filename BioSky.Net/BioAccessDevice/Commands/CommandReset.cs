using BioAccessDevice.Abstract;

namespace BioAccessDevice.Commands
{
  public class CommandReset : AccessDeviceCommand
  {
    public CommandReset() : base()
    {
      _command = new byte[] { (byte)AccessDeviceCommandID.WRITE_PORT_B, 0, 0, 0, 0, 8, 2 };
      _actualResponse = new byte[_command.Length];
    }

  }  
}
