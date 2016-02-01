using BioData.Holders.Base;
using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders
{
  public class AccessDeviceHolder : HolderBase<AccessDevice, long>
  {
    public AccessDeviceHolder() : base() { }

    protected override void UpdateDataSet(IList<AccessDevice> list)
    {
      foreach (AccessDevice accessDevice in list)
        AddToDataSet(accessDevice, accessDevice.Id);
    }

  }
}
