using BioContracts;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BioModule.ViewModels
{
  public class ImageItemViewModel : PropertyChangedBase
  {    
    public void Zoom(double newControlWidth, double newControlHeight )
    {
      if (ControlHeight == newControlHeight && newControlWidth == ControlWidth )
        return;

      ControlHeight = newControlHeight * ZOOM_RATIO;
      ControlWidth  = newControlWidth  * ZOOM_RATIO;

      float maxWidthScale  = (float)ControlWidth  / (float)ImageSource.Width ;      
      float maxHeightScale = (float)ControlHeight / (float)ImageSource.Height;

      ImageSourceScale = Math.Min(maxHeightScale, maxWidthScale);      
    }

    public void UpdateImageSource( BitmapSource source )
    {
      if (source == null)
      {
        //ImageSource = source;
        return;
      }

      try
      {        
        source.Freeze();
        ImageSource = source;
      }
      catch (Exception ex) {
        //_notifier.Notify(ex);
      }
    }


    private BitmapSource _imageSource;
    public BitmapSource ImageSource
    {
      get {
        if (_imageSource == null)
          return ResourcesLoader.ResourceLoader.UserDefaultImageIconSource;
        return _imageSource; }
      set
      {
        if (_imageSource != value)
        {
          _imageSource = value;
          NotifyOfPropertyChange(() => ImageSource);
        }
      }
    }

    private double _imageSourceScale;
    public double ImageSourceScale
    {
      get { return _imageSourceScale; }
      set
      {
        if (_imageSourceScale != value)
        {
          _imageSourceScale = value;
          NotifyOfPropertyChange(() => ImageSourceScale);
        }
      }
    }

    private double _controlWidth;
    public double ControlWidth
    {
      get { return _controlWidth; }
      set
      {
        if (_controlWidth != value)
        {
          _controlWidth = value;
          NotifyOfPropertyChange(() => ControlWidth);
        }
      }
    }

    private double _controlHeight;
    public double ControlHeight
    {
      get { return _controlHeight; }
      set
      {
        if (_controlHeight != value)
        {
          _controlHeight = value;
          NotifyOfPropertyChange(() => ControlHeight);
        }
      }
    }

    
    private const float ZOOM_RATIO = 0.9f;
  }
}
