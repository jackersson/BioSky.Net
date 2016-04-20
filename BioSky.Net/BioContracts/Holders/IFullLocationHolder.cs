using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Holders
{
  public interface IFullLocationHolder : IFullHolder<Location>
  {
    HashSet<string> AccessDevicesSet { get; }
    HashSet<string> CaptureDevicesSet { get; }
    HashSet<string> FingerDevicesSet { get; }

    Location GetValue(Location location);
  }
}
