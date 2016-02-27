using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;

using Microsoft.Win32;
using System.IO;
using System.Drawing;
using MahApps.Metro.Controls;
using BioData;
using BioService;
using System.Windows.Threading;
using BioModule.Utils;
using BioContracts;

namespace BioModule.ViewModels
{
  public class ImageViewModel : Screen, IImageUpdatable
  {
    public ImageViewModel(IProcessorLocator locator)
    {     
      _locator       = locator      ;
      //_windowManager = windowManager;

    
      _bioUtils     = new BioContracts.Common.BioImageUtils();
      _bioFileUtils = new BioFileUtils();

      ZoomToFitState = true;
      CurrentImagePhoto = null; 
    }
    #region Update
    public void UpdateImage(Photo photo, string prefixPath = "")
    {
      if (photo != null)
      {
        string photoLocation = prefixPath + "\\" + photo.FileLocation;

        var result = SetImageFromFile(photoLocation);
        if (result != null)
          CurrentImagePhoto = photo;
      }
      else
        Clear();
    }
    public void UpdateImage(ref Bitmap img)
    {
      if (img == null)
        return;

      BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(img);
      newFrame.Freeze();

      CurrentImageSource = newFrame;

      if (_width != _imageViewWidth || _height != _imageViewHeight)
      {
        _width = _imageViewWidth;
        _height = _imageViewHeight;
        Zoom(_imageViewWidth, _imageViewHeight);
      }
    }
    public Photo UploadPhotoFromFile()
    {
      var dialog = _bioFileUtils.OpenFileDialog();      
      if (dialog.ShowDialog() == true)
      {
        string filename = dialog.FileName;
        if (File.Exists(filename))
        {
          Zoom(_imageViewWidth, _imageViewHeight);
          UpdateImageFromPath(filename);
          return CurrentImagePhoto;
        }
      }
      return null;
    }
    public void UpdateImageFromPath(string path)
    {
      BitmapImage bmp = SetImageFromFile(path);
      if (bmp == null)
        CurrentImageSource = null;

      Google.Protobuf.ByteString description = Google.Protobuf.ByteString.CopyFrom(File.ReadAllBytes(path));
      Photo newphoto = new Photo()
      {
        EntityState = EntityState.Added
        , Description = description
        , FileLocation = ""
        , FirLocation = ""
        , SizeType = PhotoSizeType.Full
        , OriginType = PhotoOriginType.Loaded
      };
      CurrentImagePhoto = newphoto;
    } 
    
    private BitmapImage SetImageFromFile(string fileName = "")
    {
      if (!File.Exists(fileName))
        return null;
       
      BitmapImage bmp    = GetImageSource(fileName);
      CurrentImageSource = bmp;
      Zoom(_imageViewWidth, _imageViewHeight);

      return bmp;      
    }

    public BitmapImage GetImageSource(string fileName)
    {
      if (!File.Exists(fileName))
        return null;

      BitmapImage image = new BitmapImage();
            
      image.BeginInit();
      image.UriSource   = new Uri(fileName);
      image.CacheOption = BitmapCacheOption.OnLoad;
      image.EndInit();

      return image;
    }


    #endregion

    #region Interface
    public void Clear()
    {
      CurrentImagePhoto = null;
      CurrentImageSource = null;
      Zoom(_imageViewWidth, _imageViewHeight);
    }

    public void CancelClick(double viewWidth, double viewHeight)
    {
      Clear();
      Zoom(viewWidth, viewHeight);      
    }


    public void Zoom(double viewWidth, double viewHeight)
    {
      _imageViewWidth = viewWidth;
      _imageViewHeight = viewHeight;

      double zoomRateToFitInView = ZoomRate / ZOOM_RATIO;

      double imageWidth = CurrentImageSource.Width;
      double imageHeight = CurrentImageSource.Height;

      double minImageSide = Math.Min(imageWidth, imageHeight);
      double maxImageSide = (minImageSide == imageWidth) ? imageHeight : imageWidth;

      double proportionRate = minImageSide / maxImageSide;

      double calculatedZoomFactor = zoomRateToFitInView * proportionRate;

      CalculatedImageWidth = calculatedZoomFactor * viewWidth;
      CalculatedImageHeight = calculatedZoomFactor * viewHeight;

      CalculatedImageScale = CalculatedImageWidth / imageWidth;
      CalculatedImageScaleY = CalculatedImageHeight / imageHeight;

      if (CalculatedImageScale > CalculatedImageScaleY)
        CalculatedImageScale = CalculatedImageScaleY;     
    }
    #endregion

    #region UI
    /*
    //??????
    public delegate void OnEnrollFromPhotoHandler();
    public event OnEnrollFromPhotoHandler EnrollFromPhotoChanged;

    public void OnEnrollFromPhoto()
    {
      if (EnrollFromPhotoChanged != null)
        EnrollFromPhotoChanged();
    }

    public delegate void OnEnrollFromCameraHandler();
    public event OnEnrollFromCameraHandler EnrollFromCameraChanged;

    public void OnEnrollFromCamera()
    {
      if (EnrollFromCameraChanged != null)
        EnrollFromCameraChanged();
    }

      */

    double _calculatedImageScale;
    public double CalculatedImageScale
    {
      get { return _calculatedImageScale; }
      set
      {
        if (_calculatedImageScale != value)
        {
          _calculatedImageScale = value;
          NotifyOfPropertyChange(() => CalculatedImageScale);
        }
      }
    }

    double _calculatedImageScaleY;
    public double CalculatedImageScaleY
    {
      get { return _calculatedImageScaleY; }
      set
      {
        if (_calculatedImageScaleY != value)
        {
          _calculatedImageScaleY = value;
          NotifyOfPropertyChange(() => CalculatedImageScaleY);
        }
      }
    }

    /*
    private PhotoInfoExpanderViewModel _photoInfoExpanderView;
    public PhotoInfoExpanderViewModel PhotoInfoExpanderView
    {
      get { return _photoInfoExpanderView; }
      set
      {
        if (_photoInfoExpanderView != value)
        {
          _photoInfoExpanderView = value;
          NotifyOfPropertyChange(() => PhotoInfoExpanderView);
        }
      }
    }

    private ProgressRingViewModel _progressRingView;
    public ProgressRingViewModel ProgressRingView
    {
      get { return _progressRingView; }
      set
      {
        if (_progressRingView != value)
        {
          _progressRingView = value;
          NotifyOfPropertyChange(() => ProgressRingView);
        }
      }
    }
    */

    double _calculatedImageWidth;
    private double CalculatedImageWidth
    {
      get { return _calculatedImageWidth; }
      set
      {
        if (_calculatedImageWidth != value)
        {
          _calculatedImageWidth = value;
          NotifyOfPropertyChange(() => CalculatedImageWidth);
        }
      }
    }

    double _calculatedImageHeight;
    private double CalculatedImageHeight
    {
      get { return _calculatedImageHeight; }
      set
      {
        if (_calculatedImageHeight != value)
        {
          _calculatedImageHeight = value;
          NotifyOfPropertyChange(() => CalculatedImageHeight);
        }
      }
    }

    private double _zoomRate;
    public double ZoomRate
    {
      get { return _zoomRate; }
      set
      {
        if (_zoomRate != value)
        {
          _zoomRate = value;

          ZoomToFitState = (_zoomRate == ZOOM_TO_FIT_RATE);
          NotifyOfPropertyChange(() => ZoomRate);
        }
      }
    }

    private bool _zoomToFitState;
    public bool ZoomToFitState
    {
      get { return _zoomToFitState; }
      set
      {
        if (_zoomToFitState != value)
        {
          _zoomToFitState = value;
          ZoomRate = _zoomToFitState ? ZOOM_TO_FIT_RATE : ZoomRate;
          NotifyOfPropertyChange(() => ZoomToFitState);
        }
      }
    }

    private BitmapSource _currentImageSource;
    public BitmapSource CurrentImageSource
    {
      get
      {
        if (_currentImageSource == null)
          _currentImageSource = ResourceLoader.UserDefaultImageIconSource;
        return _currentImageSource;

      }
      private set
      {
        try
        {
          if (_currentImageSource != value)
          {
            _currentImageSource = value;
            NotifyOfPropertyChange(() => CurrentImageSource);
          }
        }
        catch (TaskCanceledException ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
    }
     

    private Photo _currentImagePhoto;
    public Photo CurrentImagePhoto
    {
      get { return _currentImagePhoto; }
      set
      {
        if (_currentImagePhoto != value)
        {
          _currentImagePhoto = value;
          NotifyOfPropertyChange(() => CurrentImagePhoto);
        }
      }
    }
      
    #endregion    

    #region Global Variables

    private double _imageViewWidth  = 0;
    private double _imageViewHeight = 0;
    private double _width           = 0;
    private double _height          = 0;

    private const double ZOOM_TO_FIT_RATE = 90;
    private const double ZOOM_RATIO = 100D;

    private BioContracts.Common.BioImageUtils _bioUtils     ;
    private BioFileUtils                      _bioFileUtils ; 
    private readonly IProcessorLocator        _locator      ;
    private          IWindowManager           _windowManager;


    #endregion
  }
}
