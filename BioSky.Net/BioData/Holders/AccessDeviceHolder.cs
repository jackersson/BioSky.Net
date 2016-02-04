using BioData.Holders.Base;
using BioService;
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

    public override void Update(IList<AccessDevice> list, Result result)
    {
      foreach (ResultPair currentResult in result.Status)
      {
        AccessDevice accessDevice = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
            accessDevice = currentResult.AccessDevice;
          else
            accessDevice = list.Where(x => x.Id == currentResult.Id).FirstOrDefault();

          if (accessDevice != null)
          {
            accessDevice.Dbstate = DbState.None;
            UpdateItem(accessDevice, accessDevice.Id, currentResult.State);
          }
        }
      }
      base.Update(list, result);
    }

  }
}
