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
    List<AccessDevice>  AccessDevices { get; }
    List<CaptureDevice> CaptureDevices { get; }

    Location GetValue(Location location);
  }
}
