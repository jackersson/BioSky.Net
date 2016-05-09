using AForge.Video.DirectShow;
using BioContracts;
using BioContracts.BioTasks.Utils;
using BioContracts.CaptureDevices;
using BioContracts.Common;
using BioContracts.Locations;
using BioModule.ResourcesLoader;
using BioModule.Utils;
using BioModule.ViewModels;
using BioService;
using Caliburn.Micro;
using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace BioModule.BioModels
{
  public class FacesImageModel : PropertyChangedBase, IBioImageModel, ICaptureDeviceObserver
  {
    public FacesImageModel(IProcessorLocator locator, IImageViewUpdate imageView)
    {
      _utils              = new BioImageUtils();
      Information         = new FacialInformationViewModel();
      EnrollmentBar       = new FaceEnrollmentBarViewModel(locator);
      _marker             = new MarkerUtils();
      _faceFinder         = new FaceFinder();
      _markerBitmapHolder = new MarkerBitmapSourceHolder();
      _notifier           = locator.GetProcessor<INotifier>();

      _imageView    = imageView;

      //CurrentPhoto = GetTestPhoto();
    }

    #region test
    public Photo GetTestPhoto()
    {
      Photo ph = new Photo();
      ph.Width = 640;
      ph.Height = 480;

      //ph.SizeType = PhotoSizeType.Croped;
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
    #endregion

    public void ShowEnrollment(bool state)
    {
      EnrollmentBar.Subscribe(this);
    }

    public void Activate(bool isNewUser)
    {
      (EnrollmentBar as IScreen).Activate();
      EnrollmentBar.SetStyle(isNewUser);
      EnrollmentBar.Unsubscribe(this);
      EnrollmentBar.Subscribe(this);

      if (_markerBitmapHolder.Unmarked == null)
        SetDefaultImage();
      else
        _imageView.SetSingleImage(_markerBitmapHolder.Unmarked);

      _isActive = true;
      NotifyOfPropertyChange(() => IsActive);
    }

    public void Deactivate()
    {
      (EnrollmentBar as IScreen).Deactivate(false);
      EnrollmentBar.Unsubscribe(this);

      _isActive = false;
      NotifyOfPropertyChange(() => IsActive);
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
        SetDefaultImage();
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
        SetDefaultImage();
        return;
      }

      BitmapSource newFrame;
      if (EnrollmentBar._isSnapshootActive)
      {
        GetSnapshoot(frame);
        newFrame = BitmapConversion.BitmapToBitmapSource(frame);
        _imageView.SetSingleImage(newFrame);
        _markerBitmapHolder.Unmarked = newFrame;        
      }
      else
      {
        Bitmap processedFrame = DrawFaces(ref frame);
        //Bitmap processedFrame = frame;

        newFrame = BitmapConversion.BitmapToBitmapSource(processedFrame);
      }
      _imageView.SetSingleImage(newFrame);
    }

    private void GetSnapshoot(Bitmap frame)
    {
      EnrollmentBar._isSnapshootActive = false;
      EnrollmentBar.SelectedDevice = null;

      SetSnapshootPhoto(frame);

      _notifier.ShowInformation("Snapshoot done");  
      _notifier.Hide(SNAPSHOOT_TIMER);
    }

    private void SetSnapshootPhoto(Bitmap frame)
    {
      Google.Protobuf.ByteString description = _utils.ImageToByteString(frame);
      Photo photo = new Photo()
      {
          Bytestring = description
        , Datetime   = DateTime.Now.Ticks
        , OriginType = PhotoOriginType.Thumbnail
      };

      CurrentPhoto = photo;
    }

    public void SetDefaultImage()
    {
      BitmapSource source = ResourceLoader.UserDefaultImageIconSource;
      _imageView.SetSingleImage(source);
      _markerBitmapHolder.Unmarked = source;
    }

    public void OnStop(bool stopped, ErrorMessage error, LocationDevice device)
    {
      if (_markerBitmapHolder.Unmarked != null)
        _imageView.SetSingleImage(_markerBitmapHolder.Unmarked);
      else
        SetDefaultImage();
    }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) { }

    public void OnMessage(string message){ }

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

    private FacialInformationViewModel _information;
    public FacialInformationViewModel Information
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

    public const int SNAPSHOOT_TIMER = 2000;

    private MarkerUtils              _marker            ;
    private FaceFinder               _faceFinder        ;
    private IImageViewUpdate         _imageView         ;
    private MarkerBitmapSourceHolder _markerBitmapHolder;
    private BioImageUtils            _utils             ;
    private INotifier _notifier;
    #endregion
  }

  public class MarkerBitmapSourceHolder
  {
    public BitmapSource Unmarked;
    public BitmapSource Marked;
  }
}
