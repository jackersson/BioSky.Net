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
    public ImageViewModel(IProcessorLocator locator, IWindowManager  windowManager)
    {
      _bioUtils = new BioContracts.Common.BioImageUtils();
      _locator       = locator      ;
      _windowManager = windowManager;

      if(_locator != null)
        _bioEngine = _locator.GetProcessor<IBioEngine>();

      PhotoInfoExpanderView = new PhotoInfoExpanderViewModel();
      ProgressRingView      = new ProgressRingViewModel();

      ZoomToFitState = true;
      CurrentImagePhoto = null; 
    }
    #region Update

    public void ShowProgress(int progress, bool status)
    {
      ProgressRingView.ShowProgress(progress, status);
    }


    public void UploadClick(double viewWidth, double viewHeight)
    {
      var dialog = OpenFileDialog();
      if (dialog.ShowDialog() == true)
      {
        Zoom(viewWidth, viewHeight);
        UpdateImage(null, dialog.FileName);        
      }
    }
    public Photo UploadPhoto()
    {
      var dialog = OpenFileDialog();      
      if (dialog.ShowDialog() == true)
      {
        string filename = dialog.FileName;
        if (File.Exists(filename))
        {
          Zoom(_imageViewWidth, _imageViewHeight);
          UpdateImage(null, filename);
          return CurrentImagePhoto;
        }
      }
      return null;
    }

   
    public OpenFileDialog OpenFileDialog()
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "All files (*.*)|*.*";
      openFileDialog.InitialDirectory = Environment.CurrentDirectory;

      return openFileDialog;
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

    public void SavePhoto(string path)
    {
      Directory.CreateDirectory(Path.GetDirectoryName(path));

      if (CurrentImagePhoto.Description == null)
        return;

      byte[] data = CurrentImagePhoto.Description.ToByteArray();
      var fs = new BinaryWriter(new FileStream(path, FileMode.CreateNew, FileAccess.Write));
      fs.Write(data);
      fs.Close();
    }
    public void MovePhoto(string pathFrom, string pathTo)
    {
      Directory.CreateDirectory(Path.GetDirectoryName(pathTo));

      try
      {
        File.Move(pathFrom, pathTo); // Try to move
        Console.WriteLine("Moved"); // Success
      }
      catch (IOException ex)
      {
        Console.WriteLine(ex); // Write error
      }
      
    }
    public BitmapImage SetImageFromFile(string fileName)
    {
      if (File.Exists(fileName))
      {
        //Bitmap bmp = (Bitmap)Image.FromFile(fileName);
        BitmapImage bmp = GetImageSource(fileName);
        CurrentImageSource = bmp;//BitmapConversion.BitmapToBitmapSource(bmp);

        Zoom(_imageViewWidth, _imageViewHeight);
        return bmp;
      }

      //CurrentImageSource = null;

      return null;
    }

    public BitmapImage GetImageSource(string fileName)
    {
      BitmapImage image = new BitmapImage();
            
      image.BeginInit();
      image.UriSource = new Uri(fileName);
      image.CacheOption = BitmapCacheOption.OnLoad;
      image.EndInit();

      return image;
    }

    public void Update(Person user)
    {
      if (user == null)
        CurrentImageSource = null;

      User = user;
    }

    public void UpdateImage(Photo photo, string path)
    {
      if (photo != null)
      {
        string photoLocation = path + "\\" + photo.FileLocation;

        var result = SetImageFromFile(photoLocation);        
        if(result != null)
          CurrentImagePhoto = photo;
      }
      else if(photo == null && path != null)
      {
        BitmapImage bmp = SetImageFromFile(path);
        if (bmp == null)
        {
          CurrentImageSource = null;
        }
        
        Google.Protobuf.ByteString description = Google.Protobuf.ByteString.CopyFrom(File.ReadAllBytes(path ));
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
      else if (photo == null && path == null)
      {
        CurrentImagePhoto = null;
        CurrentImageSource = null;
        Zoom(_imageViewWidth, _imageViewHeight);       
      }
    }
    #endregion

    #region Interface
    public void Clear()
    {
      CurrentImageSource = ResourceLoader.UserDefaultImageIconSource;
    }

    public void CancelClick(double viewWidth, double viewHeight)
    {
      CurrentImageSource = ResourceLoader.UserDefaultImageIconSource;
      Zoom(viewWidth, viewHeight);
      CurrentImagePhoto = null;
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

    public void EnrollFromPhoto()
    {      
      OnEnrollFromPhoto();
    }

    public void EnrollFromCamera()
    {
      _windowManager.ShowDialog(new CameraDialogViewModel(_bioEngine, _locator));
      OnEnrollFromCamera();
    }
    #endregion

    #region UI

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

    private Bitmap _currentImageBitmap;
    public Bitmap CurrentImageBitmap
    {
      get { return _currentImageBitmap; }
      set
      {
        if (_currentImageBitmap != value)
        {
          _currentImageBitmap = value;
          NotifyOfPropertyChange(() => CurrentImageBitmap);
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
    #endregion    

    #region Global Variables

    private double _imageViewWidth  = 0;
    private double _imageViewHeight = 0;
    private double _width           = 0;
    private double _height          = 0;

    private const double ZOOM_TO_FIT_RATE = 90;
    private const double ZOOM_RATIO = 100D;

    private BioContracts.Common.BioImageUtils _bioUtils     ;
    private readonly IBioEngine               _bioEngine    ;
    private readonly IProcessorLocator        _locator      ;
    private          IWindowManager           _windowManager;


    #endregion
  }
}
