using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.CaptureDevices
{
  public class CaptureDeviceCapabilities
  {
    public CaptureDeviceCapabilities()
    {
      FrameSize = new Size();
    }
    public override int GetHashCode()
    {
      unchecked
      {
        int hash = 13;
        hash = hash * 23 + FrameSize.Height.GetHashCode();
        hash = hash * 23 + FrameSize.Width .GetHashCode();
        hash = hash * 23 + AverageFrameRate.GetHashCode();
        return hash;
      }
    }
    public override bool Equals(object obj)
    {
      return (this.GetHashCode() == obj.GetHashCode());
    }

    private Size _frameSize;
    public Size FrameSize
    {
      get { return _frameSize; }
      set
      {
        if (_frameSize != value)        
          _frameSize = value;        
      }
    }

    private int _averageFrameRate;
    public int AverageFrameRate
    {
      get { return _averageFrameRate; }
      set
      {
        if (_averageFrameRate != value)        
          _averageFrameRate = value;        
      }
    }
  }
}
