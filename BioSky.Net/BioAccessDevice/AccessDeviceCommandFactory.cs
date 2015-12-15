using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioAccessDevice.Interfaces;

namespace BioAccessDevice
{
  public class AccessDeviceCommandFactory : ICommandFactory
  {
    T ICommandFactory.GetCommand<T>() 
    {
      return new T();
    }

    public object GetCommand(string commandName)
    {
      object command = Activator.CreateInstance(Type.GetType(commandName));
      return command;
    }
  }
}
