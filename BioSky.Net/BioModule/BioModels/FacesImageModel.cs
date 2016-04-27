using AForge.Video.DirectShow;
using BioContracts;
using BioContracts.CaptureDevices;
using BioContracts.Common;
using BioContracts.Locations;
using BioModule.ResourcesLoader;
using BioModule.Utils;
using BioModule.ViewModels;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioModule.BioModels
{
  public class FacesImageModel : PropertyChangedBase, IBioImageModel, ICaptureDeviceObserver
  {
    public FacesImageModel(IProcessorLocator locator, IImageViewUpdate imageView)
    {
      Information         = new PhotoInformationViewModel();
      EnrollmentBar       = new FaceEnrollmentBarViewModel(locator);
      _marker             = new MarkerUtils();
      _faceFinder         = new FaceFinder();
      _markerBitmapHolder = new MarkerBitmapSourceHolder();

      _imageView = imageView;
            
      //CurrentPhoto = GetTestPhoto();
    }

    //test
    public Photo GetTestPhoto()
    {
      Photo ph = new Photo();
      ph.Width = 640;
      ph.Height = 480;

      ph.SizeType = PhotoSizeType.Croped;
      ph.OriginType = PhotoOriginType.Enrolled;

      /*
      ph.PortraitCharacteristic = new PortraitCharacteristic()
      {
        Age = 24
                                                               ,
        FacesCount = 1
      };

      BiometricLocation bl = new BiometricLocation() { Confidence = 1.0f, Xpos = 100.0f, Ypos = 100.0f };
      ph.PortraitCharacteristic.Faces.Add(new FaceCharacteristic() { Location = bl, Width = 100 });
      */
      return ph;

    }

    public void ShowEnrollment(bool state)
    {
      EnrollmentBar.Subscribe(this);
    }

    public void Activate()
    {
      if (EnrollmentBar is IScreen)
      {
        IScreen screen = EnrollmentBar as IScreen;
        screen.Activate();
      }
      EnrollmentBar.Unsubscribe(this);
      EnrollmentBar.Subscribe(this);
      // EnrollmentViewModel.SelectedDeviceChanged += EnrollmentViewModel_SelectedDeviceChanged;

      //if (EnrollmentViewModel.DeviceObserver.DeviceName != null)
      //  EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);
      // else
      if(_markerBitmapHolder.Unmarked == null)
        _imageView.SetSingleImage(ResourceLoader.UserDefaultImageIconSource);
      else
        _imageView.SetSingleImage(_markerBitmapHolder.Unmarked);

      _isActive = true;
      NotifyOfPropertyChange(() => IsActive);
    }

    public void Deactivate()
    {
      if (EnrollmentBar is IScreen)
      {
        IScreen screen = EnrollmentBar as IScreen;
        screen.Deactivate(false);
      }
      EnrollmentBar.Unsubscribe(this);
      _isActive = false;
      NotifyOfPropertyChange(() => IsActive);

      //EnrollmentViewModel.SelectedDeviceChanged -= EnrollmentViewModel_SelectedDeviceChanged;
      //EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
    }
    private void EnrollmentViewModel_SelectedDeviceChanged()
    {
      //  EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
      //  EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);
    }
    public Bitmap DrawFaces(ref Bitmap img)
    {
      return _marker.DrawRectangles(_faceFinder.GetFaces(ref img), ref img);
    }

    public void ShowDetails(bool state)
    {
      if (state)      
        DrawPortraitCharacteristics();      
      else   
        _imageView.SetSingleImage(_markerBitmapHolder.Unmarked);
    }

    public void DrawPortraitCharacteristics()
    {
      if (CurrentPhoto == null)
        return;

      _markerBitmapHolder.Unmarked = _imageView.GetImageByIndex(0);
      /*
      Bitmap detailedBitmap = _marker.DrawPortraitCharacteristics(CurrentPhoto.PortraitCharacteristic
                                     , BitmapConversion.BitmapSourceToBitmap(_markerBitmapHolder.Unmarked));

      _markerBitmapHolder.Marked = BitmapConversion.BitmapToBitmapSource(detailedBitmap);

      _imageView.SetSingleImage(_markerBitmapHolder.Marked);
      */
    }

    public void UploadPhoto(Photo photo)
    {
      if(photo == null)
      {
        _imageView.SetSingleImage(ResourceLoader.UserDefaultImageIconSource);
        return;
      }
      CurrentPhoto = photo;
      _markerBitmapHolder.Unmarked = _imageView.GetImageByIndex(0);
    }

    public void UpdateController(IUserBioItemsController controller)
    {
      if (controller != null)
        Controller = controller;
    }

    public void OnFrame(ref Bitmap frame)
    {   
      UpdateFrame(frame);
    }

    public void UpdateFrame(Bitmap frame)
    {
      if (frame == null)
      {
        _imageView.SetSingleImage(ResourceLoader.UserDefaultImageIconSource);
        return;
      }

      Bitmap processedFrame = DrawFaces(ref frame);
      BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(processedFrame);

      // Console.WriteLine(_uiDispatcher.GetHashCode());
      //Console.WriteLine(Dispatcher.CurrentDispatcher.GetHashCode());

      _imageView.SetSingleImage(newFrame);
    }


    public void OnStop(bool stopped, string message, LocationDevice device)
    {
      _imageView.SetSingleImage(null);
    }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) { }
    
    #region UI
    public BioImageModelType BioType
    {
      get { return BioImageModelType.Faces; }
    }
    public BitmapSource SettingsToogleButtonBitmap
    {
      get { return ResourceLoader.UserFacesIconSource; }
    }

    private Photo _currentPhoto;
    public Photo CurrentPhoto
    {
      get
      {
        return _currentPhoto;
      }
      set
      {
        if (_currentPhoto != value)
        {
          _currentPhoto = value;
          Information.Update(_currentPhoto);
        }        
                 
      }
    }

    private PhotoInformationViewModel _information;
    public PhotoInformationViewModel Information
    {
      get { return _information; }
      set
      {
        if (_information != value)
        {
          _information = value;
          NotifyOfPropertyChange(() => Information);
        }
      }
    }

    private FaceEnrollmentBarViewModel _expanderBarModel;
    public FaceEnrollmentBarViewModel EnrollmentBar
    {
      get { return _expanderBarModel; }
      set
      {
        if (_expanderBarModel != value)
        {
          _expanderBarModel = value;
          NotifyOfPropertyChange(() => EnrollmentBar);
        }
      }
    }

    private IUserBioItemsController _controller;
    public IUserBioItemsController Controller
    {
      get { return _controller; }
      set
      {
        if (_controller != value)
        {
          _controller = value;
          NotifyOfPropertyChange(() => Controller);
        }
      }
    }

    private bool _isActive;
    public bool IsActive{
      get{
        return _isActive;
      }
    }

    #endregion

    #region GlobalVariables

    private MarkerUtils              _marker            ;
    private FaceFinder               _faceFinder        ;
    private IImageViewUpdate         _imageView         ;
    private MarkerBitmapSourceHolder _markerBitmapHolder;


    #endregion
  }

  public class MarkerBitmapSourceHolder
  {
    public BitmapSource Unmarked;
    public BitmapSource Marked;
  }

}
