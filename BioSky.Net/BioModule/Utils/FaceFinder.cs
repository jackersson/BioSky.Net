using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.Utils
{
  public class FaceFinder
  {
    public FaceFinder()
    {
      HaarCascade cascade = new FaceHaarCascade();
      _detector = new HaarObjectDetector(cascade, 30);

      _resizer = new ResizeNearestNeighbor(160, 120);
    }

    public Rectangle[] GetFaces( ref Bitmap bitmap )
    {
      UnmanagedImage im = UnmanagedImage.FromManagedImage(bitmap);

      float xscale = bitmap.Width / 160f;
      float yscale = bitmap.Height / 120f;
      
      UnmanagedImage downsample = _resizer.Apply(im);
      Rectangle[] rects = _detector.ProcessFrame(downsample);      
    
      for( int i = 0; i < rects.Length; ++i)
      {
        rects[i].X = (int)((rects[i].X + rects[i].Width  / 2.5f) * xscale);
        rects[i].Y = (int)((rects[i].Y + rects[i].Height / 2.5f) * yscale);        

        rects[i].Inflate( (int)(0.3f * rects[i].Width * xscale)
                        , (int)(0.5f * rects[i].Height * yscale));
      }
      

      return rects;
    }
    private ResizeNearestNeighbor _resizer;
    private HaarObjectDetector   _detector;
  }
}
