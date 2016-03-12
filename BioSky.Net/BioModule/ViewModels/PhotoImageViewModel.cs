using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.IO;
using System.Drawing;
using BioService;
using BioModule.Utils;
using BioContracts;
using BioContracts.Common;

namespace BioModule.ViewModels
{
  public class PhotoImageViewModel : ImageViewModel, IPhotoUpdatable
  {
    public PhotoImageViewModel(IProcessorLocator locator) : base()    
    {      
      _notifier       = locator.GetProcessor<INotifier>();
     
      PhotoDetails        = new PhotoInfoExpanderViewModel();
      _enroller           = new Enroller(locator);
      _markerBitmapHolder = new MarkerBitmapSourceHolder();
      EnrollmentViewModel = new EnrollmentBarViewModel(locator);

      SetVisibility();
      UpdateFromPhoto(GetTestPhoto());
    }

    public Photo GetTestPhoto()
    {
      Photo ph = new Photo();
      ph.Width = 640;
      ph.Height = 480;

      ph.SizeType = PhotoSizeType.Croped;
      ph.OriginType = PhotoOriginType.Enrolled;

      ph.PortraitCharacteristic = new PortraitCharacteristic()
      {
        Age = 24
                                                               ,
        FacesCount = 1
      };

      BiometricLocation bl = new BiometricLocation() { Confidence = 1.0f, Xpos = 100.0f, Ypos = 100.0f };
      ph.PortraitCharacteristic.Faces.Add(new FaceCharacteristic() { Location = bl, Width = 100 });

      return ph;

    }

    #region Interface

    protected override void OnActivate()
    {
      PhotoDetails.ExpanderChanged += ShowPhotoDetails;
      EnrollmentViewModel.SelectedDeviceChanged += EnrollmentViewModel_SelectedDeviceChanged;
            
      if(EnrollmentViewModel.DeviceObserver.DeviceName != null)
        EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);

      base.OnActivate();
    }

    private void EnrollmentViewModel_SelectedDeviceChanged()
    {
      EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
      EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);
    }

    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      base.UpdateFromImage(ref bitmap);
    }    

    protected override void OnDeactivate(bool close)
    {
      PhotoDetails.ExpanderChanged -= ShowPhotoDetails;
      EnrollmentViewModel.SelectedDeviceChanged -= EnrollmentViewModel_SelectedDeviceChanged;
      EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
      base.OnDeactivate(close);
    }

    public Photo UploadPhotoFromFile()
    {
      string filename = base.Upload();
      UpdatePhotoFromFile(filename);
      return CurrentPhoto;
    }

    public void UpdatePhotoFromFile(string filename = "")
    {
      BitmapImage bmp = base.UpdateFromFile(filename);

      if (bmp == null)
      {
        Clear();
        return;
      }

      Google.Protobuf.ByteString bytes = Google.Protobuf.ByteString.CopyFrom(File.ReadAllBytes(filename));
      Photo newphoto = new Photo() { Bytestring = bytes};
      CurrentPhoto = newphoto;
    }

    public void UpdateFromPhoto(Photo photo, string prefixPath = "")
    {
      if (photo == null)
      {
        Clear();
        return;
      }
      
      string filename = prefixPath + "\\" + photo.PhotoUrl;
      base.UpdateFromFile(filename);

      CurrentPhoto = photo;
      PhotoDetails.Update(CurrentPhoto);
    }    

    public void EnrollFromLoadedImage()
    {
      if (_enroller.Busy)
      {
        _notifier.Notify("Wait for finnishing previous operation", WarningLevel.Warning);
        return;
      }
      
      Photo photo = UploadPhotoFromFile();
      if (photo == null || photo.Bytestring.Length <= 0)
      {
        _notifier.Notify("Please load correct image", WarningLevel.Warning);
        return;
      }

      _enroller.EnrollmentDone -= OnEnrollmentDone;
      _enroller.EnrollmentDone += OnEnrollmentDone;
      _enroller.Start(photo, CurrentPerson);
    }    

    private void OnEnrollmentDone(Photo photo, Person person)
    {      
      _enroller.EnrollmentDone -= OnEnrollmentDone;

      photo.Bytestring = CurrentPhoto.Bytestring;
      CurrentPhoto = photo;   
    }

    public void EnrollFromCamera()
    {
      if (_enroller.Busy)
      {
        _notifier.Notify("Wait for finnishing previous operation", WarningLevel.Warning);
        return;
      }      
    }

    public void ShowPhotoDetails(bool isExpanded)
    {
      if (isExpanded)      
        DrawPortraitCharacteristics();
      else
        CurrentImageSource = MarkerBitmapHolder.Unmarked;
    }

    public void DrawPortraitCharacteristics()
    {
      if (CurrentPhoto == null)
        return;

      MarkerBitmapHolder.Unmarked = CurrentImageSource;

      Bitmap detailedBitmap = _marker.DrawPortraitCharacteristics( CurrentPhoto.PortraitCharacteristic
                                     , BitmapConversion.BitmapSourceToBitmap(CurrentImageSource));
      
      CurrentImageSource = BitmapConversion.BitmapToBitmapSource(detailedBitmap);
      MarkerBitmapHolder.Marked = CurrentImageSource;
    }

    public void SetVisibility(bool arrows = true         , bool cancelButton = true
                             , bool enrollExpander = true, bool controllPanel = true, bool enrollFromPhoto = true)
    {
      ArrowsVisibility          = arrows         ;
      CancelButtonVisibility    = cancelButton   ;
      EnrollExpanderVisibility  = enrollExpander ;
      ControllPanelVisibility   = controllPanel  ;
      EnrollFromPhotoVisibility = enrollFromPhoto;
    }
   
    #endregion

    #region BioService

    public override void Clear()
    {
      CurrentPhoto = null;
      base.Clear();     
    }    
    #endregion

    #region UI
    private MarkerBitmapSourceHolder _markerBitmapHolder;
    public MarkerBitmapSourceHolder MarkerBitmapHolder
    {
      get { return _markerBitmapHolder; }    
    }

    private EnrollmentBarViewModel _enrollmentViewModel;
    public EnrollmentBarViewModel EnrollmentViewModel
    {
      get { return _enrollmentViewModel; }
      set
      {
        if (_enrollmentViewModel != value)
        {
          _enrollmentViewModel = value;
          NotifyOfPropertyChange(() => EnrollmentViewModel);
        }
      }
    }

    private bool _enrollFromPhotoVisibility;
    public bool EnrollFromPhotoVisibility
    {
      get { return _enrollFromPhotoVisibility; }
      set
      {
        if (_enrollFromPhotoVisibility != value)
        {
          _enrollFromPhotoVisibility = value;
          NotifyOfPropertyChange(() => EnrollFromPhotoVisibility);
        }
      }
    }

    private bool _arrowsVisibility;
    public bool ArrowsVisibility
    {
      get { return _arrowsVisibility; }
      set
      {
        if (_arrowsVisibility != value)
        {
          _arrowsVisibility = value;
          NotifyOfPropertyChange(() => ArrowsVisibility);
        }
      }
    }

    private bool _cancelButtonVisibility;
    public bool CancelButtonVisibility
    {
      get { return _cancelButtonVisibility; }
      set
      {
        if (_cancelButtonVisibility != value)
        {
          _cancelButtonVisibility = value;
          NotifyOfPropertyChange(() => CancelButtonVisibility);
        }
      }
    }

    private bool _enrollExpanderVisibility;
    public bool EnrollExpanderVisibility
    {
      get { return _enrollExpanderVisibility; }
      set
      {
        if (_enrollExpanderVisibility != value)
        {
          _enrollExpanderVisibility = value;
          NotifyOfPropertyChange(() => EnrollExpanderVisibility);
        }
      }
    }

    private bool _controllPanelVisibility;
    public bool ControllPanelVisibility
    {
      get { return _controllPanelVisibility; }
      set
      {
        if (_controllPanelVisibility != value)
        {
          _controllPanelVisibility = value;
          NotifyOfPropertyChange(() => ControllPanelVisibility);
        }
      }
    }

    private Photo _currentPhoto;
    public Photo CurrentPhoto
    {
      get { return _currentPhoto; }
      set
      {
        if (_currentPhoto != value)
        {
          _currentPhoto = value;
          NotifyOfPropertyChange(() => CurrentPhoto);
        }
      }
    }

    private PhotoInfoExpanderViewModel _photoDetails;
    public PhotoInfoExpanderViewModel PhotoDetails
    {
      get { return _photoDetails; }
      set
      {
        if (_photoDetails != value)
        {
          _photoDetails = value;
          NotifyOfPropertyChange(() => PhotoDetails);
        }
      }
    }

    private Person _currentPerson;
    public Person CurrentPerson
    {
      get { return _currentPerson; }
      set { _currentPerson = value; }
    }
    #endregion

    #region Global Variables    
    private readonly Enroller  _enroller;    
    private readonly INotifier _notifier;
    //private MarkerUtils _marker;
    #endregion
  }

  public class MarkerBitmapSourceHolder
  {
    public BitmapSource Unmarked;
    public BitmapSource Marked  ;
  }
}
