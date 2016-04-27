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
    ICollection<string> AccessDevices      { get; }
    ICollection<string> CaptureDevices     { get; }
    ICollection<string> FingerprintDevices { get; }
    ICollection<string> IrisDevices        { get; }


    Location GetValue(Location location);
  }
}
