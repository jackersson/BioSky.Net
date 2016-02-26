using BioData.Holders.Base;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders
{
  public class CaptureDeviceHolder : HolderBase<CaptureDevice, long>
  {
    public CaptureDeviceHolder() : base() { }

    protected override void UpdateDataSet(IList<CaptureDevice> list)
    {
      foreach (CaptureDevice captureDevice in list)
        Update(captureDevice, captureDevice.Id);
    }

    public override void Remove( long key)
    {
      base.Remove( key);
      var item = Data.Where(x => x.Id == key).FirstOrDefault();
      if (item != null)
      {
        Data.Remove(item);
      }
    }

    protected override void CopyFrom(CaptureDevice from, CaptureDevice to)
    {
      to.MergeFrom(from);
    }
       
  }
}
