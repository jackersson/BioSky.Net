using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.IO;
using System.Drawing;
using BioService;
using BioModule.Utils;
using BioContracts;
using BioContracts.Common;
using System;

namespace BioModule.ViewModels
{
  public class PhotoImageViewModel : ImageViewModel, IUserPhotoUpdatable
  {
    public PhotoImageViewModel(IProcessorLocator locator) : base()    
    {      
      _notifier       = locator.GetProcessor<INotifier>();
      _database       = locator.GetProcessor<IBioSkyNetRepository>();

      PhotoDetails        = new PhotoInfoExpanderViewModel();
      _enroller           = new Enroller(locator);
      _markerBitmapHolder = new MarkerBitmapSourceHolder();
      EnrollmentViewModel = new FaceEnrollmentBarViewModel(locator);

      SetVisibility();
      //UpdateFromPhoto(GetTestPhoto());
    }


    public override void OnClear(double viewWidth, double viewHeight)
    {
      if (CanUsePhotoController)
        UserPhotoController.Remove(CurrentPhoto);

      base.OnClear(viewWidth, viewHeight);
    }

    /*
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
    */
    #region Interface

    private string _message;
    public string Message
    {
      get { return _message; }
      set
      {
        if (_message != value)
        {
          _message = value;
          NotifyOfPropertyChange(() => Message);
        }
      }
    }

    protected override void OnActivate()
    {
      PhotoDetails.ExpanderChanged += ShowPhotoDetails;
      EnrollmentViewModel.SelectedDeviceChanged += EnrollmentViewModel_SelectedDeviceChanged;
            
     // if(EnrollmentViewModel.DeviceObserver.DeviceName != null)
      //  EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);

      base.OnActivate();
    }

    private void EnrollmentViewModel_SelectedDeviceChanged()
    {
      //EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
     // EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);
    }

    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      base.UpdateFromImage(ref bitmap);
    }    

    protected override void OnDeactivate(bool close)
    {
      PhotoDetails.ExpanderChanged -= ShowPhotoDetails;
      EnrollmentViewModel.SelectedDeviceChanged -= EnrollmentViewModel_SelectedDeviceChanged;
     // EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
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

      CurrentPhoto.Width    = (long)bmp.Width   ;
      CurrentPhoto.Height   = (long)bmp.Height  ;
      CurrentPhoto.SizeType = PhotoSizeType.Full;
    }

    public void UpdateFromPhoto(Photo photo)
    {
      if (photo == null)
      {
        Clear();
        return;
      }
      
      string filename = _database.LocalStorage.LocalStoragePath + "\\" + photo.PhotoUrl;
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
      _enroller.Start(photo, UserPhotoController.User);
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
      //ArrowsVisibility          = arrows         ;
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

    public void UpdatePhotoController(Utils.IUserPhotoController controller)
    {
      UserPhotoController = controller;
    }
    #endregion

    #region UI

    public void Next()
    {
      if ( UserPhotoController != null )
        UserPhotoController.Next();
    }

    public void Previous()
    {
      if (UserPhotoController != null)
        UserPhotoController.Previous();
    }

    public void Add()
    {
      
      if (UserPhotoController != null)
        UserPhotoController.Add(CurrentPhoto);
    }

    public void Remove()
    {
      if (UserPhotoController != null)
        UserPhotoController.Remove(CurrentPhoto);
    }

    public bool CanAddPhoto
    {
      get { return CanUsePhotoController
                && IsValid
                && UserPhotoController.User != null
                && UserPhotoController.User.Id > 0
                /*&& CurrentPhoto.PortraitCharacteristic != null
                && CurrentPhoto.PortraitCharacteristic.FirBytestring.Length > 0*/; }
    }

    public bool IsValid { get { return CurrentPhoto != null && CurrentPhoto.Bytestring.Length > 0; } }

    public bool CanMoveNext
    {
      get { return CanUsePhotoController && UserPhotoController.CanNext; }
    }

    public bool CanMovePrevious
    {
      get { return CanUsePhotoController && UserPhotoController.CanPrevious; }
    }

    public bool CanUsePhotoController
    {
      get { return _userPhotoController != null; }
    }

    private IUserPhotoController _userPhotoController;
    public IUserPhotoController UserPhotoController
    {
      get { return _userPhotoController; }
      set
      {
        if (_userPhotoController != value)
        {
          _userPhotoController = value;
          NotifyOfPropertyChange(() => UserPhotoController  );
          NotifyOfPropertyChange(() => CanUsePhotoController);
        }
      }
    }

    private MarkerBitmapSourceHolder _markerBitmapHolder;
    public MarkerBitmapSourceHolder MarkerBitmapHolder
    {
      get { return _markerBitmapHolder; }    
    }

    private FaceEnrollmentBarViewModel _enrollmentViewModel;
    public FaceEnrollmentBarViewModel EnrollmentViewModel
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
          Message = "";

          if (_currentPhoto != null && _database.Persons.PhotosIndexesWithoutExistingFile.Contains(_currentPhoto.Id))          
            Message = "Can't upload photo";

          NotifyOfPropertyChange(() => CurrentPhoto);
          NotifyOfPropertyChange(() => CanAddPhoto );
          
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

    #endregion

    #region Global Variables    
    private readonly Enroller  _enroller;    
    private readonly INotifier _notifier;
    private readonly IBioSkyNetRepository _database; 
    #endregion
  }

  public class MarkerBitmapSourceHolder
  {
    public BitmapSource Unmarked;
    public BitmapSource Marked  ;
  }
}
