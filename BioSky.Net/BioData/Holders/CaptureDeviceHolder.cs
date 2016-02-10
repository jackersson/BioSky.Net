﻿using BioData.Holders.Base;
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

    public override void Remove(CaptureDevice obj, long key)
    {
      base.Remove(obj, key);
      var item = Data.Where(x => x.Id == obj.Id).FirstOrDefault();
      if (item != null)
      {
        Data.Remove(item);
      }
    }

    protected override void CopyFrom(CaptureDevice from, CaptureDevice to)
    {
      to.MergeFrom(from);
    }

    /*
    public override void Update(IList<CaptureDevice> list,  result)
    {
      foreach (ResultPair currentResult in result.Status)
      {
        CaptureDevice captureDevice = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
            captureDevice = currentResult.CaptureDevice;
          else
            captureDevice = list.Where(x => x.Id == currentResult.Id).FirstOrDefault();

          if (captureDevice != null)
          {
            captureDevice.Dbstate = DbState.None;
            UpdateItem(captureDevice, captureDevice.Id, currentResult.State);
          }
        }
      }
      base.Update(list, result);
    }
    */
  }
}
