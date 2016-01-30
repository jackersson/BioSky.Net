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
using BioFaceService;
using System.Windows.Threading;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class ImageViewModel : Screen, IImageUpdatable
  {
    public ImageViewModel()
    {
      ZoomToFitState = true;
    }  
 
    public void UploadClick(double viewWidth, double viewHeight)
    {          

      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
      openFileDialog.InitialDirectory = Environment.CurrentDirectory;      
      
      if (openFileDialog.ShowDialog() == true)
      {        
        Zoom(viewWidth, viewHeight);
        SetImageFromFile(openFileDialog.FileName);
      }      
    }

    private void SetImageFromFile(string fileName)
    {
      if (File.Exists(fileName))
      {
        Bitmap bmp = (Bitmap)Image.FromFile(fileName);
        CurrentImageSource = BitmapConversion.BitmapToBitmapSource(bmp);
        //TODO User photo
        //User. = fileName;
        Zoom(_imageViewWidth, _imageViewHeight);
      }
    }


    public void Update(Person user )
    {
      User = user;
      //TODO User photo
      //SetImageFromFile(User.Photo);
    }

    public void UpdateImage(Uri uriSource)
    {
      SetImageFromFile(uriSource.OriginalString);
    }

    public void CancelClick(double viewWidth, double viewHeight)
    {
      CurrentImageSource = ResourceLoader.UserDefaultImageIconSource;
      Zoom(viewWidth, viewHeight);
    }

    public void Zoom(double viewWidth, double viewHeight)
    {
      _imageViewWidth  = viewWidth;
      _imageViewHeight = viewHeight;

      double zoomRateToFitInView = ZoomRate / ZOOM_RATIO;
            
      double imageWidth  = CurrentImageSource.Width;
      double imageHeight = CurrentImageSource.Height;
      
      double minImageSide = Math.Min(imageWidth, imageHeight);
      double maxImageSide = (minImageSide == imageWidth) ? imageHeight : imageWidth;      
      
      double proportionRate = minImageSide / maxImageSide;
      
      double calculatedZoomFactor = zoomRateToFitInView * proportionRate;
      
      CalculatedImageWidth  = calculatedZoomFactor * viewWidth ;
      CalculatedImageHeight = calculatedZoomFactor * viewHeight;
      
      CalculatedImageScale = CalculatedImageWidth / imageWidth;      
    }

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
    
    private double _imageViewWidth = 0;
    private double _imageViewHeight = 0;

    private const double ZOOM_TO_FIT_RATE = 90  ;
    private const double ZOOM_RATIO       = 100D;


    private Person _user;
    public Person User
    {
      get { return _user; }
      set
      {
        if (_user != value)
        {
          _user = value;
          NotifyOfPropertyChange(() => User);
        }
      }
    }

    

    public void UpdateImage(ref Bitmap img)
    {
      if (img == null)
        return;
            
      BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(img);
      newFrame.Freeze();
      CurrentImageSource = newFrame;        
    }

    public void Clear()
    {
      CurrentImageSource = ResourceLoader.UserDefaultImageIconSource;
    }


    private BitmapSource _currentImageSource;
    public BitmapSource CurrentImageSource
    {
      get {
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
  }
}
