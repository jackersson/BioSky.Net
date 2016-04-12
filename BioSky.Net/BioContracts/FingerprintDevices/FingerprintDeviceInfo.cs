using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.FingerprintDevices
{
  public class FingerprintDeviceInfo
  {
    public static int BaseInterface { get; set; }
    public string Name { get; set; }
    public bool LiveCheck { get; set; }
    public int InterfaceNumber { get; set; }
    public override bool Equals(object obj) { return Name.GetHashCode() == obj.GetHashCode(); }
    public override int GetHashCode() { return Name.GetHashCode(); }
  }
}
