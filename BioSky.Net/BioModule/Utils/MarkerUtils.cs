using Accord.Imaging.Filters;
using BioModule.ResourcesLoader;
using BioModule.ViewModels;
using BioService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioModule.Utils
{
  public class MarkerUtils
  {
    public MarkerUtils()
    {
      _rectanglesMarker = new RectanglesMarker(System.Drawing.Color.DeepSkyBlue);  
      _pointsMarker     = new PointsMarker    (System.Drawing.Color.Red, 10);

      _rectangles = new List<Rectangle>();
      _points     = new List<AForge.IntPoint>();

    }

    public Bitmap DrawPortraitCharacteristics( PortraitCharacteristic pc, Bitmap data )
    {
      if (pc == null)
        return data;

      _rectangles.Clear();
      _points    .Clear();
      foreach ( FaceCharacteristic fc in pc.Faces )
      {
        BiometricLocation faceLocation = fc.Location;
        _rectangles.Add( new Rectangle() { X = (int)faceLocation.Xpos
                                         , Y = (int)faceLocation.Ypos
                                         , Width = (int)fc.Width
                                         , Height = (int)fc.Width });


        if (fc.Eyes != null && fc.Eyes.LeftEye != null)
        {
          if (fc.Eyes.LeftEye != null)
          {
            BiometricLocation leftEye = fc.Eyes.LeftEye;
            _points.Add(new AForge.IntPoint((int)leftEye.Xpos, (int)leftEye.Ypos));
          }

          if (fc.Eyes.RightEye != null)
          {
            BiometricLocation rightEye = fc.Eyes.RightEye;
            _points.Add(new AForge.IntPoint((int)rightEye.Xpos, (int)rightEye.Ypos));
          }
        }       
      }   

      Bitmap result = DrawPoints(_points, DrawRectangles(_rectangles, data, System.Drawing.Color.DeepSkyBlue));

      return result;
    }

    public Bitmap DrawFingerCharacteristics(Bitmap data)
    {
      _points.Clear();

      _rectangles.Add(new Rectangle() { X = 0, Y = 0, Width = 100, Height = 100 });

      Bitmap result = DrawRectangles(_rectangles, data, System.Drawing.Color.Green);

      return result;
    }

    public Bitmap DrawIrisCharacteristics(Bitmap data)
    {
      _points.Clear();

      _rectangles.Add(new Rectangle() { X = 0, Y = 0, Width = 100, Height = 100 });

      Bitmap result = DrawRectangles(_rectangles, data, System.Drawing.Color.Yellow);

      return result;
    }


    public Bitmap DrawRectangles(Rectangle[] rectangles, ref Bitmap data)
    {      
      _rectanglesMarker.Rectangles = rectangles;
      Bitmap rectanglesMarked = _rectanglesMarker.Apply(data);

      return rectanglesMarked;
    }    

    public Bitmap DrawRectangles( List<Rectangle> rectangles, Bitmap data, System.Drawing.Color color)
    {  
      _rectanglesMarker.Rectangles = rectangles;
      _rectanglesMarker.MarkerColor = color;

      Bitmap rectanglesMarked = _rectanglesMarker.Apply(data);
      
      return rectanglesMarked;
    }

    public Bitmap DrawPoints(List<AForge.IntPoint> points, Bitmap data)
    {
      _pointsMarker.Points = points;
      Bitmap pointsMarked = _pointsMarker.Apply(data);

      return pointsMarked;
    }

    private List<Rectangle>       _rectangles;
    private List<AForge.IntPoint> _points    ;

    private RectanglesMarker _rectanglesMarker;
    private PointsMarker     _pointsMarker    ;

    private RectanglesMarker _rectanglesMarkerFinger;
    private RectanglesMarker _rectanglesMarkerEye   ;

  }
}
