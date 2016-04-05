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
using AForge.Video.DirectShow;

namespace BioModule.ViewModels
{
  
  public interface IBioImageModel
  {
    void Activate();
    void Deactivate();
    void Reset();
    void UpdateController(IUserBioItemsController controller);
    void UploadPhoto(Photo photo);
    object GetInformation();   
    PhotoViewEnum EnumState                 { get; }
    IUserBioItemsController Controller      { get; }
  }

  #region facemodel
  public class FacesImageModel :  PropertyChangedBase, IBioImageModel, ICaptureDeviceObserver
  {
    public FacesImageModel(IProcessorLocator locator, IImageViewUpdate imageView)
    {
      PhotoInformation    = new PhotoInformationViewModel();
      ExpanderBarModel    = new FaceEnrollmentBarViewModel(locator);
      _marker             = new MarkerUtils();
      _faceFinder         = new FaceFinder();

      _imageView = imageView;
    }

    public void Activate()
    {
      ExpanderBarModel.Subscribe(this);
     // EnrollmentViewModel.SelectedDeviceChanged += EnrollmentViewModel_SelectedDeviceChanged;

      //if (EnrollmentViewModel.DeviceObserver.DeviceName != null)
      //  EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);
      // else
      _imageView.SetSingleImage(PhotoImageSource);           
    }

    public void Deactivate()
    {
      //EnrollmentViewModel.SelectedDeviceChanged -= EnrollmentViewModel_SelectedDeviceChanged;
      //EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
    }
    private void EnrollmentViewModel_SelectedDeviceChanged()
    {
    //  EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
    //  EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);
    }
    /*
    public void UpdateFromImage(ref Bitmap img)
    {
      if (img == null)
        return;

      Bitmap processedFrame = DrawFaces(ref img);

     // _imageView.UpdateOneImage(ref processedFrame); 
    }
    */
    public Bitmap DrawFaces(ref Bitmap img){
      return _marker.DrawRectangles(_faceFinder.GetFaces(ref img), ref img);
    }
    /*
    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      UpdateFromImage(ref bitmap);
    }
    */

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
    
    public object GetInformation()
    {
      return PhotoInformation;
    }    
    public void OnFrame(ref Bitmap frame)
    {
      if (frame == null)
      {
        _imageView.SetSingleImage(null);
        return;
      }

      Bitmap processedFrame = DrawFaces(ref frame);

      BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(processedFrame);
      newFrame.Freeze();

      _imageView.SetSingleImage(newFrame);
    }

    public void OnStop(bool stopped, string message) { _imageView.SetSingleImage(null);  }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) {}

    public PhotoViewEnum EnumState { get { return PhotoViewEnum.Faces; }
    }
    public BitmapSource SettingsToogleButtonBitmap
    {
      get { return ResourceLoader.UserFacesIconSource; }
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

    private FaceEnrollmentBarViewModel _expanderBarModel;
    public FaceEnrollmentBarViewModel ExpanderBarModel
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
    

    private MarkerUtils      _marker    ;
    private FaceFinder       _faceFinder;
    private IImageViewUpdate _imageView ;
  }

  #endregion


  #region fingersmodel
  public class FingersImageModel :  PropertyChangedBase, IBioImageModel
  {
    public FingersImageModel(IImageViewUpdate imageView)
    {
      FingerInformation = new FingerInformationViewModel();
      ExpanderBarModel = new FingerBarViewModel();

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

    public void Activate()
    {
      _imageView.SetSingleImage(FingerImageSource);
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
      get { return ResourceLoader.UserFingerprintIconSource; }
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

    private FingerBarViewModel _expanderBarModel;
    public FingerBarViewModel ExpanderBarModel
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
  #endregion

  #region irises
  public class IrisesImageModel :  PropertyChangedBase, IBioImageModel
  {
    public IrisesImageModel(IImageViewUpdate imageView)
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
    public void Activate()
    {
      _imageView.SetDoubleImage(LeftEyeImageSource, RightEyeImageSource);
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
      get { return ResourceLoader.UserIricesIconSource; }
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
  #endregion


  public enum PhotoViewEnum
  {
     Faces
   , Irises
   , Fingers
  }

  public class BioImageViewModel : ImageViewModel, IUserBioItemsUpdatable
  {
    public BioImageViewModel(IProcessorLocator locator) 
    {      
      _notifier       = locator.GetProcessor<INotifier>();
      _database       = locator.GetProcessor<IBioSkyNetRepository>();      

      BioImageModels = new ObservableCollection<IBioImageModel>();

      BioImageModels.Add(new FacesImageModel  (locator, this));
      BioImageModels.Add(new FingersImageModel(this)         );
      BioImageModels.Add(new IrisesImageModel (this)         );

      BioImageDetails        = new PhotoInfoExpanderViewModel();

      //_enroller           = new Enroller(locator);
      //_markerBitmapHolder = new MarkerBitmapSourceHolder();

      SetVisibility();

     // UpdateFromPhoto(GetTestPhoto());

      ChangeBioImageModel(PhotoViewEnum.Faces);     

    }

    public void ChangeBioImageModel(PhotoViewEnum state)
    {  
      foreach (IBioImageModel view in BioImageModels)
      {
        if (view.EnumState == state)
        {
          if (CurrentBioImage == view)
            return;
                   
          CurrentBioImage = view;           
        }
        else
          view.Deactivate();
      }
    }

    public void OnLoadFromFile()
    {
      Photo photo = UploadPhotoFromFile();

      if(CurrentBioImage != null)
        CurrentBioImage.UploadPhoto(photo);            
    }
    public override void Clear()
    {
      if (CanUsePhotoController)
        UserController.Remove(CurrentPhoto);

      base.Clear();
    }

    /*
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
    */

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
      //PhotoDetails.ExpanderChanged += ShowPhotoDetails;

      if (CurrentBioImage != null)
        CurrentBioImage.Activate();

      base.OnActivate();
    }   

    protected override void OnDeactivate(bool close)
    {
     // PhotoDetails.ExpanderChanged -= ShowPhotoDetails;

      if (CurrentBioImage != null)
        CurrentBioImage.Deactivate();

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
      BioImageDetails.Update(CurrentPhoto);
    }        

    /*
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
    */

    //Make as style
    public void SetVisibility( bool arrows = true         , bool cancelButton = true
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

    public void UpdateBioItemsController(Utils.IUserBioItemsController controller)
    {
      if (controller == null)
        return;

      foreach (IBioImageModel view in BioImageModels)
      {
        if (view.EnumState == controller.PageEnum)        
          view.UpdateController(controller);

        NotifyOfPropertyChange(() => UserController);
      }       
    }
    #endregion

    #region UI


    private IBioImageModel _currentBioImage;
    public IBioImageModel CurrentBioImage
    {
      get { return _currentBioImage; }
      set
      {
        if (_currentBioImage != value)
        {
          _currentBioImage = value;

          if (_currentBioImage != null)
          {
            BioImageDetails.Update(_currentBioImage.GetInformation());
            OnBioImageChanged(_currentBioImage.EnumState);
            _currentBioImage.Activate();
          }

          NotifyOfPropertyChange(() => CurrentBioImage);
          NotifyOfPropertyChange(() => UserController);
          NotifyOfPropertyChange(() => CanUsePhotoController);
        }
      }
    }

    private ObservableCollection<IBioImageModel> _bioImageModels;
    public ObservableCollection<IBioImageModel> BioImageModels
    {
      get { return _bioImageModels; }
      set
      {
        if (_bioImageModels != value)
        {
          _bioImageModels = value;
          NotifyOfPropertyChange(() => BioImageModels);
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
      get { return CanUsePhotoController && UserController.CanNext; }
    }

    public bool CanMovePrevious
    {
      get { return CanUsePhotoController && UserController.CanPrevious; }
    }

    public bool CanUsePhotoController
    {
      get { return CurrentBioImage.Controller != null; }
    }
    public IUserBioItemsController UserController
    {
      get { return (CurrentBioImage != null) ? CurrentBioImage.Controller : null; }
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

    private PhotoInfoExpanderViewModel _bioImageDetails;
    public PhotoInfoExpanderViewModel BioImageDetails
    {
      get { return _bioImageDetails; }
      set
      {
        if (_bioImageDetails != value)
        {
          _bioImageDetails = value;
          NotifyOfPropertyChange(() => BioImageDetails);
        }
      }
    }

    public delegate void BioImageChangedEventHandler(PhotoViewEnum bioImageModel);

    public event BioImageChangedEventHandler BioImageModelChanged;

    private void OnBioImageChanged(PhotoViewEnum bioImageModel)
    {
      if (BioImageModelChanged != null)
        BioImageModelChanged(bioImageModel);
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
