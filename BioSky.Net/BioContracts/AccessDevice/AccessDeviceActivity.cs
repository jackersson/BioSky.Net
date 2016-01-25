using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public enum AccessDeviceCommands
  {
     CommandAccess = 0
   , CommandDallasKey
   , CommandLight
   , CommandReset
   , CommandReady
  }

  public class AccessDeviceActivity
  {
    public AccessDeviceCommands CommandID
    {
      get { return _commandID; }
      set
      {
        _commandID = value;
      }
    }

    public byte[] Data
    {
      get { return _data; }
      set
      {
        _data = value;
      }

    }

    private AccessDeviceCommands _commandID;
    private byte[] _data;
  }
}
