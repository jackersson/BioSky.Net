using BioContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AForge.Video.DirectShow;

namespace BioEngine.CaptureDevices
{
  public class CaptureDeviceEngine : ICaptureDeviceEngine
  {
    public CaptureDeviceEngine()
    {
      //_captureDevicesNames = new FilterInfoCollection(FilterCategory.VideoInputDevice);    
      _captureDeviceEnumerator = new CaptureDeviceEnumerator();
      _captureDeviceEnumerator.Start();
    }

    public AsyncObservableCollection<FilterInfo> GetCaptureDevicesNames()
    {
      return _captureDeviceEnumerator.CaptureDevicesNames;
    }

    public void Stop()
    {
      _captureDeviceEnumerator.Stop();
    }

    private readonly CaptureDeviceEnumerator _captureDeviceEnumerator;
    //private FilterInfoCollection _captureDevicesNames;
  }
}
