using System.Collections.Generic;
using BioAccessDevice.Interfaces;
using BioAccessDevice.Commands;
using BioContracts;

namespace BioAccessDevice
{
  public class AccessDeviceCommandFactory : ICommandFactory
  {
    public AccessDeviceCommandFactory()
    {
      _commands = new Dictionary<AccessDeviceCommands, ICommand>();
      _commands.Add(AccessDeviceCommands.CommandAccess   , new CommandAccess());
      _commands.Add(AccessDeviceCommands.CommandReset    , new CommandReset());
      _commands.Add(AccessDeviceCommands.CommandReady    , new CommandReady());
      _commands.Add(AccessDeviceCommands.CommandDallasKey, new CommandDallasKey());
     // _commands.Add(AccessDeviceCommands.CommandLight    , new CommandLight());
    }
    /*
    T ICommandFactory.GetCommand<T>() 
    {
      return new T();
    }
    */
    public object GetCommand(AccessDeviceCommands commandID)
    {
      ICommand command;
      _commands.TryGetValue(commandID, out command);     
      return command;
    }

    private Dictionary<AccessDeviceCommands, ICommand> _commands;
  }
}
