using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using Accord.Vision.Tracking;
using AForge.Controls;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioUITest.ViewModels
{
  public class VideoDetector 
  {
        
    HaarObjectDetector detector;
    
    Camshift tracker = null;
    
    private bool detecting;

    private bool tracking;

    public VideoDetector()
    {
      HaarCascade cascade = new FaceHaarCascade();
      detector = new HaarObjectDetector(cascade, 25
                                        , ObjectDetectorSearchMode.NoOverlap, 1.2f
                                        , ObjectDetectorScalingMode.GreaterToSmaller);


      detecting = true;

      tracker = new Camshift();
      tracker.Mode = CamshiftMode.RGB;
      tracker.Conservative = true;
      tracker.AspectRatio = 1.5f;
    }
    public Rectangle[] Detect( ref Bitmap image)
    {            
      UnmanagedImage im = UnmanagedImage.FromManagedImage(image);

      float xscale = image.Width / 160f;
      float yscale = image.Height / 120f;

      ResizeNearestNeighbor resize = new ResizeNearestNeighbor(160, 120);
      UnmanagedImage downsample = resize.Apply(im);
      
      Rectangle[] regions = detector.ProcessFrame(downsample);

      if (regions.Length > 0)
      {
        tracker.Reset();

        foreach (Rectangle face in regions)
        {
          Rectangle window = new Rectangle(
           (int)((regions[0].X + regions[0].Width / 2f) * xscale),
           (int)((regions[0].Y + regions[0].Height / 2f) * yscale),
           1, 1);

          window.Inflate(
              (int)(0.2f * regions[0].Width * xscale),
              (int)(0.4f * regions[0].Height * yscale));

          tracker.SearchWindow = window;
          tracker.ProcessFrame(im);

          //marker = new RectanglesMarker(window);
          //marker.ApplyInPlace(im);
        }
        tracking = true;
      }

      return regions;
    }

    public void Tracking(ref Bitmap image)
    {
      UnmanagedImage im = UnmanagedImage.FromManagedImage(image);

      // Track the object
      tracker.ProcessFrame(im);

      // Get the object position
      var obj = tracker.TrackingObject;
      var wnd = tracker.SearchWindow;
    }

    public void Detection(ref Bitmap image)
    {
      detecting = false;
      tracking = false;

      UnmanagedImage im = UnmanagedImage.FromManagedImage(image);

      float xscale = image.Width / 160f;
      float yscale = image.Height / 120f;

      ResizeNearestNeighbor resize = new ResizeNearestNeighbor(160, 120);
      UnmanagedImage downsample = resize.Apply(im);

      Rectangle[] regions = detector.ProcessFrame(downsample);

      /*
      if (regions.Length > 0)
      {
        tracker.Reset();

        foreach (Rectangle face in regions)
        {
          Rectangle window = new Rectangle(
           (int)((regions[0].X + regions[0].Width / 2f) * xscale),
           (int)((regions[0].Y + regions[0].Height / 2f) * yscale),
           1, 1);

          window.Inflate(
              (int)(0.2f * regions[0].Width * xscale),
              (int)(0.4f * regions[0].Height * yscale));
          
          tracker.SearchWindow = window;
          tracker.ProcessFrame(im);
        }       
        tracking = true;       
      }
      else      
        detecting = true;  
        */    
    }

  }
}
