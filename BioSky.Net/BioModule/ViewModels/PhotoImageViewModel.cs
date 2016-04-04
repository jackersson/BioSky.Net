using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.IO;
using System.Drawing;
using BioService;
using BioModule.Utils;
using BioContracts;
using BioContracts.Common;
using System;
using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace BioModule.ViewModels
{
  
  public interface IPhotoView
  {
    void Activate();
    void Deactivate();
    void Reset();
    void UpdateController(IUserBioItemsController controller);
    void UploadPhoto(Photo photo);
    object GetInformation();
    object GetBarViewModel();    
    PhotoViewEnum EnumState                 { get; }
    BitmapSource SettingsToogleButtonBitmap { get; }
    BitmapSource LoadFromFileButtonBitmap   { get; }
    IUserBioItemsController Controller      { get; }
  }

  public interface IFacesModel
  {

  }

  public class FacesModel :  PropertyChangedBase, IPhotoView
  {
    public FacesModel(IProcessorLocator locator, IImageViewUpdate imageView)
    {
      PhotoInformation    = new PhotoInformationViewModel();
      EnrollmentViewModel = new EnrollmentBarViewModel(locator);
      _marker             = new MarkerUtils();
      _faceFinder         = new FaceFinder();

      _imageView = imageView;
    }

    public void Activate()
    {
      EnrollmentViewModel.SelectedDeviceChanged += EnrollmentViewModel_SelectedDeviceChanged;

      if (EnrollmentViewModel.DeviceObserver.DeviceName != null)
        EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);
      else
        _imageView.SetOneImage(PhotoImageSource);   
    }

    public void Deactivate()
    {
      EnrollmentViewModel.SelectedDeviceChanged -= EnrollmentViewModel_SelectedDeviceChanged;
      EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
    }
    private void EnrollmentViewModel_SelectedDeviceChanged()
    {
      EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
      EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);
    }
    public void UpdateFromImage(ref Bitmap img)
    {
      if (img == null)
        return;

      Bitmap processedFrame = DrawFaces(ref img);

      _imageView.UpdateOneImage(ref processedFrame); 
    }
    public Bitmap DrawFaces(ref Bitmap img)
    {
      return _marker.DrawRectangles(_faceFinder.GetFaces(ref img), ref img);
    }
    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      UpdateFromImage(ref bitmap);
    }

    public void Reset()
    {
      
    }

    public void UploadPhoto(Photo photo)
    {
      
    }

    public void UpdateController(IUserBioItemsController controller)
    {
      if(controller != null)
        Controller = controller;
    }

    #region UI

    public object GetInformation()
    {
      return PhotoInformation;
    }

    public object GetBarViewModel()
    {
      return EnrollmentViewModel;
    }

    public PhotoViewEnum EnumState
    {
      get { return PhotoViewEnum.Faces; }
    }
    public BitmapSource SettingsToogleButtonBitmap
    {
      get { return ResourceLoader.EnrollFromCaptureDeviceIconSource; }
    }
    public BitmapSource LoadFromFileButtonBitmap
    {
      get { return ResourceLoader.EnrollFromPhotoIconSource; }
    }

    private BitmapSource _photoImageSource;
    public BitmapSource PhotoImageSource
    {
      get
      {
        if (_photoImageSource == null)
          _photoImageSource = ResourceLoader.UserDefaultImageIconSource;
        return _photoImageSource;
      }
      set
      {
        if (_photoImageSource != value)
        {
          _photoImageSource = value;
          NotifyOfPropertyChange(() => PhotoImageSource);
        }
      }  
    }

    private PhotoInformationViewModel _photoInformation;
    public PhotoInformationViewModel PhotoInformation
    {
      get { return _photoInformation; }
      set
      {
        if (_photoInformation != value)
        {
          _photoInformation = value;
          NotifyOfPropertyChange(() => PhotoInformation);
        }
      }
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

    #endregion

    private MarkerUtils      _marker    ;
    private FaceFinder       _faceFinder;
    private IImageViewUpdate _imageView ;
  }

  public class FingersModel :  PropertyChangedBase, IPhotoView
  {
    public FingersModel(IImageViewUpdate imageView)
    {
      FingerInformation = new FingerInformationViewModel();
      FingerBar         = new FingerBarViewModel();

      _imageView = imageView;
    }
    public void Reset()
    {
      
    }

    public void UploadPhoto(Photo photo)
    {
      throw new NotImplementedException();
    }

    public object GetInformation()
    {
      return FingerInformation;
    }

    public void UpdateController(IUserBioItemsController controller)
    {
      if (controller != null)
        Controller = controller;
    }

    public object GetBarViewModel()
    {
      return FingerBar;
    }

    public void Activate()
    {
      _imageView.SetOneImage(FingerImageSource);
    }

    public void Deactivate()
    {
      
    }

    public PhotoViewEnum EnumState
    {
      get { return PhotoViewEnum.Fingers; }
    }
    public BitmapSource SettingsToogleButtonBitmap
    {
      get { return ResourceLoader.PlusIconSource; }
    }
    public BitmapSource LoadFromFileButtonBitmap
    {
      get { return ResourceLoader.RefreshIconSource; }
    }

    private BitmapSource _fingerImageSource;
    public BitmapSource FingerImageSource
    {
      get
      {
        if (_fingerImageSource == null)
          _fingerImageSource = ResourceLoader.FingerScanIconSource;
        return _fingerImageSource;
      }
    }

    private FingerBarViewModel _fingerBar;
    public FingerBarViewModel FingerBar
    {
      get { return _fingerBar; }
      set
      {
        if (_fingerBar != value)
        {
          _fingerBar = value;
          NotifyOfPropertyChange(() => FingerBar);
        }
      }
    }

    private FingerInformationViewModel _fingerInformation;
    public FingerInformationViewModel FingerInformation
    {
      get { return _fingerInformation; }
      set
      {
        if (_fingerInformation != value)
        {
          _fingerInformation = value;
          NotifyOfPropertyChange(() => FingerInformation);
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

    private IImageViewUpdate _imageView;
  }

  public class IrisesModel :  PropertyChangedBase, IPhotoView
  {
    public IrisesModel(IImageViewUpdate imageView)
    {
      IrisInformation = new IrisInformationViewModel();

      _imageView = imageView;
    }
    public void Reset()
    {
      
    }
    public void UploadPhoto(Photo photo)
    {
      throw new NotImplementedException();
    }

    public object GetInformation()
    {
      return IrisInformation;
    }

    public void UpdateController(IUserBioItemsController controller)
    {
      if (controller != null)
        Controller = controller;
    }

    public object GetBarViewModel()
    {
      return null;
    }

    public void Activate()
    {
      _imageView.SetTwoImages(LeftEyeImageSource, RightEyeImageSource);
    }

    public void Deactivate()
    {
      
    }

    public PhotoViewEnum EnumState
    {
      get { return PhotoViewEnum.Irises; }
    }
    public BitmapSource SettingsToogleButtonBitmap
    {
      get { return ResourceLoader.EnrollFromCaptureDeviceIconSource; }
    }
    public BitmapSource LoadFromFileButtonBitmap
    {
      get { return ResourceLoader.EnrollFromPhotoIconSource; }
    }

    private IrisInformationViewModel _irisInformation;
    public IrisInformationViewModel IrisInformation
    {
      get { return _irisInformation; }
      set
      {
        if (_irisInformation != value)
        {
          _irisInformation = value;
          NotifyOfPropertyChange(() => IrisInformation);
        }
      }
    }

    private BitmapSource _leftEyeImageSource;
    public BitmapSource LeftEyeImageSource
    {
      get
      {
        if (_leftEyeImageSource == null)
          _leftEyeImageSource = ResourceLoader.IrisScanImageIconSource;
        return _leftEyeImageSource;
      }
    }

    private BitmapSource _rightEyeImageSource;
    public BitmapSource RightEyeImageSource
    {
      get
      {
        if (_rightEyeImageSource == null)
          _rightEyeImageSource = ResourceLoader.IrisScanImageIconSource;
        return _rightEyeImageSource;
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

    private IImageViewUpdate _imageView;
  }

  public enum PhotoViewEnum
  {
     Faces
   , Irises
   , Fingers
  }

  public class PhotoImageViewModel : ImageViewModel, IUserBioItemsUpdatable
  {
    public PhotoImageViewModel(IProcessorLocator locator) : base(locator)    
    {      
      _notifier       = locator.GetProcessor<INotifier>();
      _database       = locator.GetProcessor<IBioSkyNetRepository>();


      PhotoViews = new ObservableCollection<IPhotoView>();

      PhotoViews.Add(new FacesModel  (locator, this));
      PhotoViews.Add(new FingersModel(this)         );
      PhotoViews.Add(new IrisesModel (this)         );


      PhotoDetails        = new PhotoInfoExpanderViewModel();
      _enroller           = new Enroller(locator);
      _markerBitmapHolder = new MarkerBitmapSourceHolder();

      SetVisibility();

      UpdateFromPhoto(GetTestPhoto());

      ChangeView(PhotoViewEnum.Faces);
      ChangeView(PhotoViewEnum.Faces);

    }

    //*************************************************************************************************************

    private IPhotoView _currentPhotoView;
    public IPhotoView CurrentPhotoView
    {
      get { return _currentPhotoView; }
      set
      {
        if (_currentPhotoView != value)
        {
          _currentPhotoView = value;
          NotifyOfPropertyChange(() => CurrentPhotoView);
        }
      }
    }

    private ObservableCollection<IPhotoView> _photoViews;
    public ObservableCollection<IPhotoView> PhotoViews
    {
      get { return _photoViews; }
      set
      {
        if (_photoViews != value)
        {
          _photoViews = value;
          NotifyOfPropertyChange(() => PhotoViews);
        }
      }
    }

    public void OnChangeViewClick(PhotoViewEnum state, double scrollWidth, double scrollHeight
                                 , double imageControllerWidth, double imageControllerHeight)
    {
      ChangeView(state);      
    }

    private void ChangeView(PhotoViewEnum state)
    {  
      foreach (IPhotoView view in PhotoViews)
      {
        if (view.EnumState == state)
        {
          view.Activate();

          CurrentPhotoView = view;
          ExpanderBarModel = view.GetBarViewModel();
          UserController = view.Controller;

          PhotoDetails.Update(view.GetInformation());

          _settingsToogleButtonBitmap = view.SettingsToogleButtonBitmap;
          _loadFromFileButtonBitmap   = view.LoadFromFileButtonBitmap;         

          NotifyOfPropertyChange(() => SettingsToogleButtonBitmap);
          NotifyOfPropertyChange(() => LoadFromFileButtonBitmap);
        }
        else
          view.Deactivate();
      }
    }
    public void OnLoadFromFile()
    {
      Photo photo = UploadPhotoFromFile();

      if(CurrentPhotoView != null)
        CurrentPhotoView.UploadPhoto(photo);      

      /*if (_enroller.Busy)
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
      _enroller.Start(photo, UserPhotoController.User);*/
    }
    public override void OnClear( double viewWidth, double viewHeight
                                , double imageControlWidth, double imageControlHeight)
    {
      if (CanUsePhotoController)
        UserController.Remove(CurrentPhoto);

      base.OnClear(viewWidth, viewHeight, imageControlWidth, imageControlHeight);
    }

    //for test
    public Photo GetTestPhoto()
    {
      Photo ph = new Photo();
      ph.Width = 640;
      ph.Height = 480;

      ph.SizeType   = PhotoSizeType.Croped;
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
    // for test
    #region Interface
    public void Add()
    {
      if (UserController != null)
        UserController.Add(CurrentPhoto);
    }

    public void Next()
    {
      if (UserController != null)
        UserController.Next();
    }

    public void Previous()
    {
      if (UserController != null)
        UserController.Previous();
    }

    public void Remove()
    {
      if (UserController != null)
        UserController.Remove(CurrentPhoto);
    }
    protected override void OnActivate()
    {
      PhotoDetails.ExpanderChanged += ShowPhotoDetails;

      if (CurrentPhotoView != null)
        CurrentPhotoView.Activate();

      base.OnActivate();
    }   

    protected override void OnDeactivate(bool close)
    {
      PhotoDetails.ExpanderChanged -= ShowPhotoDetails;

      if (CurrentPhotoView != null)
        CurrentPhotoView.Deactivate();

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
      /*if (photo == null)
      {
        Clear();
        return;
      }
      
      string filename = _database.LocalStorage.LocalStoragePath + "\\" + photo.PhotoUrl;
      base.UpdateFromFile(filename);*/

      CurrentPhoto = photo;
      PhotoDetails.Update(CurrentPhoto);
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
      /*if (isExpanded)      
        DrawPortraitCharacteristics();
      else
        CurrentImageSource = MarkerBitmapHolder.Unmarked;*/
    }

    public void DrawPortraitCharacteristics()
    {
      /*if (CurrentPhoto == null)
        return;

      MarkerBitmapHolder.Unmarked = CurrentImageSource;

      Bitmap detailedBitmap = _marker.DrawPortraitCharacteristics( CurrentPhoto.PortraitCharacteristic
                                     , BitmapConversion.BitmapSourceToBitmap(CurrentImageSource));
      
      CurrentImageSource = BitmapConversion.BitmapToBitmapSource(detailedBitmap);
      MarkerBitmapHolder.Marked = CurrentImageSource;*/
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

    public void UpdateBioItemsController(Utils.IUserBioItemsController controller)
    {
      if (controller == null)
        return;

      foreach (IPhotoView view in PhotoViews)
      {
        if (view.EnumState == controller.PageEnum)
        {
          view.UpdateController(controller);
          if (UserController == null)
            UserController = CurrentPhotoView.Controller;
        }
      }    
    }
    #endregion

    #region UI

    private BitmapSource _settingsToogleButtonBitmap;
    public BitmapSource SettingsToogleButtonBitmap
    {
      get { return _settingsToogleButtonBitmap; }
    }

    private BitmapSource _loadFromFileButtonBitmap;
    public BitmapSource LoadFromFileButtonBitmap
    {
      get { return _loadFromFileButtonBitmap; }
    }

    private object _expanderBarModel;
    public object ExpanderBarModel
    {
      get { return _expanderBarModel; }
      set
      {
        if (_expanderBarModel != value)
        {
          _expanderBarModel = value;
          NotifyOfPropertyChange(() => ExpanderBarModel);
        }
      }
    }

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

    public bool CanAddPhoto
    {
      get { return CanUsePhotoController
                && IsValid
                && UserController.User != null
                && UserController.User.Id > 0
                /*&& CurrentPhoto.PortraitCharacteristic != null
                && CurrentPhoto.PortraitCharacteristic.FirBytestring.Length > 0*/; }
    }

    public bool IsValid { get { return CurrentPhoto != null && CurrentPhoto.Bytestring.Length > 0; } }

    public bool CanMoveNext
    {
      get { return true; }//CanUsePhotoController && UserController.CanNext; }
    }

    public bool CanMovePrevious
    {
      get { return true; }//CanUsePhotoController && UserController.CanPrevious; }
    }

    public bool CanUsePhotoController
    {
      get { return _userPhotoController != null; }
    }

    private IUserBioItemsController _userPhotoController;
    public IUserBioItemsController UserController
    {
      get { return _userPhotoController; }
      set
      {
        if (_userPhotoController != value)
        {
          _userPhotoController = value;
          NotifyOfPropertyChange(() => UserController  );
          NotifyOfPropertyChange(() => CanUsePhotoController);
        }
      }
    }

    private MarkerBitmapSourceHolder _markerBitmapHolder;
    public MarkerBitmapSourceHolder MarkerBitmapHolder
    {
      get { return _markerBitmapHolder; }    
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
