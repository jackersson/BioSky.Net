using BioContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioAccessDevice.Interfaces
{
  public interface ICommandFactory
  {
    T GetCommand<T>() where T : ICommand, new();

    object GetCommand(AccessDeviceCommands commandID);

  }
}
