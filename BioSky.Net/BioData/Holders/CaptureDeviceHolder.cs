using BioData.Holders.Base;
using BioFaceService;
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
        AddToDataSet(captureDevice, captureDevice.Id);
    }

  }
}
