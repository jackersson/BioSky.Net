using BioContracts;

namespace BioAccessDevice.Interfaces
{
  public interface ICommandFactory
  {
    //T GetCommand<T>() where T : ICommand, new();

    object GetCommand(AccessDeviceCommands commandID);

  }
}
