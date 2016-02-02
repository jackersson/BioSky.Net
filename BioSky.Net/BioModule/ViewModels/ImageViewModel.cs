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
      CurrentImageUri = null;   
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
      CurrentImageUri = uriSource; 
    }

    public void CancelClick(double viewWidth, double viewHeight)
    {
      CurrentImageSource = ResourceLoader.UserDefaultImageIconSource;
      Zoom(viewWidth, viewHeight);
      CurrentImageUri = null;
    }

    private Uri _currentImageUri;
    public Uri CurrentImageUri
    {
      get { return _currentImageUri; }
      set
      {
        if (_currentImageUri != value)
        {
          _currentImageUri = value;
          NotifyOfPropertyChange(() => CurrentImageUri);
        }
      }
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


    //*********************************************ProgressRing**************************************************************


      //TODO refactor as soon as possible (To difficult to understand)
    public async void ShowProgress(int progress, bool status)
    {
      ProgressRingVisibility         = true ;
      ProgressRingTextVisibility     = true ;
      ProgressRingImageVisibility    = false ;
      ProgressRingStatus             = true ;
      ProgressRingProgressVisibility = true ;

      ProgressRingText = progress + "%";
      if (progress == 100)
      {
        ProgressRingTextVisibility = false;
        ProgressRingImageVisibility = true;
        ProgressRingStatus = false;
        ProgressRingProgressVisibility = false;

        ProgressRingIconSource = status ? ResourceLoader.OkIconSource : ResourceLoader.CancelIconSource;

        await Task.Delay(3000);
        ProgressRingVisibility = false;
      } 
      

    }

    private void Show(bool show = true)
    {
      ProgressRingVisibility         = false;
      ProgressRingImageVisibility    = false;
      ProgressRingTextVisibility     = true ;
      ProgressRingProgressVisibility = true ;
      ProgressRingText = "0%";
    }
   
    private bool _progressRingVisibility;
    public bool ProgressRingVisibility
    {
      get { return _progressRingVisibility; }
      set
      {
        if (_progressRingVisibility != value)
        {
          _progressRingVisibility = value;
          NotifyOfPropertyChange(() => ProgressRingVisibility);
        }
      }
    }

    private bool _progressRingStatus;
    public bool ProgressRingStatus
    {
      get { return _progressRingStatus; }
      set
      {
        if (_progressRingStatus != value)
        {
          _progressRingStatus = value;
          NotifyOfPropertyChange(() => ProgressRingStatus);
        }
      }
    }

    private string _progressRingText;
    public string ProgressRingText
    {
      get { return _progressRingText; }
      set
      {
        if (_progressRingText != value)
        {
          _progressRingText = value;
          NotifyOfPropertyChange(() => ProgressRingText);
        }
      }
    }

    private bool _progressRingTextVisibility;
    public bool ProgressRingTextVisibility
    {
      get { return _progressRingTextVisibility; }
      set
      {
        if (_progressRingTextVisibility != value)
        {
          _progressRingTextVisibility = value;
          NotifyOfPropertyChange(() => ProgressRingTextVisibility);
        }
      }
    }

    private bool _progressRingImageVisibility;
    public bool ProgressRingImageVisibility
    {
      get { return _progressRingImageVisibility; }
      set
      {
        if (_progressRingImageVisibility != value)
        {
          _progressRingImageVisibility = value;
          NotifyOfPropertyChange(() => ProgressRingImageVisibility);
        }
      }
    }

    private bool _progressRingProgressVisibility;
    public bool ProgressRingProgressVisibility
    {
      get { return _progressRingProgressVisibility; }
      set
      {
        if (_progressRingProgressVisibility != value)
        {
          _progressRingProgressVisibility = value;
          NotifyOfPropertyChange(() => ProgressRingProgressVisibility);
        }
      }
    }

    private BitmapSource _progressRingIconSource;
    public BitmapSource ProgressRingIconSource
    {
      get
      {
        return _progressRingIconSource;
      }
      set
      {
        if (_progressRingIconSource != value)
        {
          _progressRingIconSource = value;
          NotifyOfPropertyChange(() => ProgressRingIconSource);
        }
      }
    }
  }
}
