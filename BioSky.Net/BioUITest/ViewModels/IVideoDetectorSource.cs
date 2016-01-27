using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioUITest.ViewModels
{
  public interface IVideoDetectorSource
  {
    void Detect(ref Bitmap image);
  }
}
