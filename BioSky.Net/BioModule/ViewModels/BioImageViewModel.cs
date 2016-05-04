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
using BioModule.BioModels;

namespace BioModule.ViewModels
{   
  public enum BioImageStyle : long
  {  
      Zoom               = 1 << 0
    , BioSelector        = 1 << 1
    , LiveEnrollment     = 1 << 2
    , EnrollmentFromFile = 1 << 3
    , Information        = 1 << 4
    , Arrows             = 1 << 5
    , CancelBtn          = 1 << 6
    , Btn3D              = 1 << 7
    , FacialBtn          = 1 << 8
    , FingerBtn          = 1 << 9
    , IrisBtn            = 1 << 10
  }  
  public class BioImageViewModel : ImageViewModel, IUserBioItemsUpdatable
  {
    public BioImageViewModel(IProcessorLocator locator, long style = MAX_BIO_IMAGE_STYLE)
    {
      _notifier = locator.GetProcessor<INotifier>();
      _database = locator.GetProcessor<IBioSkyNetRepository>();

      Progress = _notifier.LoadingViewModel as ProgressRingViewModel;

      BioImageModels = new ObservableCollection<IBioImageModel>();

      BioImageModels.Add(new FacesImageModel  (locator, this));
      BioImageModels.Add(new FingersImageModel(locator, this));
      BioImageModels.Add(new IrisesImageModel (locator, this));

      SetStyle(style);
      // UpdateFromPhoto(GetTestPhoto());

      SetBioImageModel(BioImageModelType.Faces);
    }   

    public void SetBioImageModel(BioImageModelType state)
    {
      if (CurrentBioImage != null && CurrentBioImage.BioType == state)
        return;

      foreach (IBioImageModel view in BioImageModels)
      {
        if (view.BioType == state)
        {
          CurrentBioImage = view;         
          CurrentBioImage.Activate(_isNewUser);
          CurrentBioImage.ShowDetails(_isDetailsExpanded);
        }
        else
          view.Deactivate();
      }
    }  

    public void OnLoadFromFile()
    {
      Photo photo = UploadPhotoFromFile();

      if(photo == null)
      {
        Progress.ShowWaiting("Cannot upload Photo");
        Progress.Hide(CANNOT_UPLOAD_PHOTO_TIMER);
      }

      if (CurrentBioImage != null)
        CurrentBioImage.UploadPhoto(photo);            
    }

    public void OnClearClick()
    {
      if (CanUsePhotoController)
        UserController.Remove(CurrentPhoto);

      base.Clear();
      CurrentBioImage.UploadPhoto(null);
    }

    #region testPhoto
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
    */
    #endregion

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
      if (CurrentBioImage != null)
        CurrentBioImage.Activate(_isNewUser);

      base.OnActivate();
    }   

    protected override void OnDeactivate(bool close)
    {    
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

    public void UpdateFrame(Bitmap frame)
    { 
      if (CurrentBioImage != null) 
        CurrentBioImage.UpdateFrame( frame); 
    }


  public void UpdatePhotoFromFile(string filename = "")
    {
      BitmapImage bmp = base.UpdateFromFile(filename);

      if (bmp == null)
      {
        base.Clear();
        return;
      }

      Google.Protobuf.ByteString bytes = Google.Protobuf.ByteString.CopyFrom(File.ReadAllBytes(filename));
      Photo newphoto = new Photo() { Bytestring = bytes};

      CurrentPhoto = newphoto;

      CurrentPhoto.Width    = (long)bmp.Width   ;
      CurrentPhoto.Height   = (long)bmp.Height  ;
     // CurrentPhoto.SizeType = PhotoSizeType.Full;     
    }

    public void UpdateFromPhoto(Photo photo)
    {
      if (photo == null)
      {
        SetDefaultImage();
        return;
      }
      
      string filename = _database.LocalStorage.GetParametr(ConfigurationParametrs.MediaPathway) + photo.PhotoUrl;
      BitmapImage image = base.UpdateFromFile(filename);

      if(image == null)
      {
        SetDefaultImage();
        return;
      }

      CurrentPhoto = photo;
      
      CurrentBioImage.UploadPhoto(CurrentPhoto);
    }

    private void SetDefaultImage()
    {
      base.Clear();
      if (CurrentBioImage != null)
        CurrentBioImage.UploadPhoto(null);
    }

    private void OnBioImageChanged(BioImageModelType bioImageModel)
    {
      if (BioImageModelChanged != null)
        BioImageModelChanged(bioImageModel);
    }

    private void OnPhotoChanged()
    {
      if (PhotoChanged != null)
        PhotoChanged();
    }
    #endregion

    #region BioService

    public void UpdateBioItemsController(Utils.IUserBioItemsController controller)
    {
      if (controller == null)
        return;

      foreach (IBioImageModel view in BioImageModels)
      {
        if (view.BioType == controller.BioImageModelType)
        {
          view.UpdateController(controller);
          NotifyOfPropertyChange(() => UserController);
          return;
        }
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

          NotifyOfPropertyChange(() => CurrentBioImage);          
          NotifyOfPropertyChange(() => UserController);
          NotifyOfPropertyChange(() => CanUsePhotoController);

          OnBioImageChanged(CurrentBioImage.BioType);
        }
      }
    }
   
    private bool _isDetailsExpanded;
    public bool IsDetailsExpanded
    {
      get { return _isDetailsExpanded; }
      set
      {
        if (_isDetailsExpanded != value)
        {
          _isDetailsExpanded = value;
          CurrentBioImage.ShowDetails(_isDetailsExpanded);
          NotifyOfPropertyChange(() => IsDetailsExpanded);
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

    public bool CanMoveNext{
      get { return CanUsePhotoController && UserController.CanNext; }
    }

    public bool CanMovePrevious {
      get { return CanUsePhotoController && UserController.CanPrevious; }
    }

    public bool CanUsePhotoController{
      get { return CurrentBioImage.Controller != null; }
    }

    public IUserBioItemsController UserController {
      get { return (CurrentBioImage != null) ? CurrentBioImage.Controller : null; }
    }
        
    #region style
    private long SetFlag(long currentStyle, BioImageStyle style)
    {
      return currentStyle | (long)style;
    }
    private bool HasFlag(long currentStyle, BioImageStyle style)
    {
      long activityL = (long)style;
      return (currentStyle & activityL) == activityL;
    }
    public void SetStyle(long style) { ControlStyle = style; }

    private long _controlStyle;
    public long ControlStyle
    {
      get { return _controlStyle; }
      set
      {
        if (_controlStyle != value)
        {
          _controlStyle = value;
          //for test
          _controlStyle = MAX_BIO_IMAGE_STYLE;
          //for test
          _isNewUser = (_controlStyle == NEW_USER_BIO_IMAGE_STYLE);
          if(CurrentBioImage != null)
          {
            CurrentBioImage.Deactivate();
            CurrentBioImage.Activate(_isNewUser);
          }          
          NotifyOfPropertyChange(() => ControlStyle);
          NotifyOfPropertyChange(() => EnrollFromPhotoVisibility);
          NotifyOfPropertyChange(() => LiveEnrollmentVisibility);
          NotifyOfPropertyChange(() => CancelButtonVisibility);
          NotifyOfPropertyChange(() => EnrollExpanderVisibility);
          NotifyOfPropertyChange(() => BioImageDetailsVisibility);
          NotifyOfPropertyChange(() => BioImageSelectorVisibility);
          NotifyOfPropertyChange(() => ArrowsVisibility);
          NotifyOfPropertyChange(() => ZoomVisibility);
          NotifyOfPropertyChange(() => Button3DVisibility);
          NotifyOfPropertyChange(() => FacialButtonVisibility);
          NotifyOfPropertyChange(() => FingerButtonVisibility);
          NotifyOfPropertyChange(() => IrisButtonVisibility);
        }
      }
    }
    public bool IrisButtonVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.IrisBtn); }
    }
    public bool FingerButtonVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.FingerBtn); }
    }
    public bool FacialButtonVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.FacialBtn); }
    }
    public bool EnrollFromPhotoVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.EnrollmentFromFile); }
    }

    public bool LiveEnrollmentVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.LiveEnrollment); }
    }

    public bool CancelButtonVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.CancelBtn); }
    }
    public bool Button3DVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.Btn3D); }
    }

    public bool EnrollExpanderVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.EnrollmentFromFile) || HasFlag(ControlStyle, BioImageStyle.LiveEnrollment); }
    }

    public bool BioImageDetailsVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.Information); }
    }

    public bool BioImageSelectorVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.BioSelector); }
    }

    public bool ArrowsVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.Arrows); }
    }

    public bool ZoomVisibility
    {
      get { return HasFlag(ControlStyle, BioImageStyle.Zoom); }
    }
    #endregion
    
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

          //if (_currentPhoto != null && _database.Persons.PhotosIndexesWithoutExistingFile.Contains(_currentPhoto.Id))          
           // Message = "Can't upload photo";

          OnPhotoChanged();

          NotifyOfPropertyChange(() => CurrentPhoto);
          NotifyOfPropertyChange(() => CanAddPhoto );
          
        }
      }
    }

    private  ProgressRingViewModel _progress;
    public ProgressRingViewModel Progress
    {
      get { return _progress; }
      set
      {
        if (_progress != value)
        {
          _progress = value;
          NotifyOfPropertyChange(() => Progress);
        }
      }
    }

    private Controller3DViewModel _controller3D;
    public Controller3DViewModel Controller3D
    {
      get {
        if (_controller3D == null)
          _controller3D = new Controller3DViewModel();

        return _controller3D; }
      set
      {
        if (_controller3D != value)
        {
          _controller3D = value;
          NotifyOfPropertyChange(() => Controller3D);
        }
      }
    }


    private object _contentView;
    public object ContentView
    {
      get
      {
        return _contentView;
      }
      set
      {
        if (_contentView != value)
        {
          _contentView = value;
          NotifyOfPropertyChange(() => ContentView);
        }
      }
    }

    private bool _isExpanded;
    public bool IsExpanded
    {
      get { return _isExpanded; }
      set
      {
        if (_isExpanded != value)
        {
          _isExpanded = value;
          NotifyOfPropertyChange(() => IsExpanded);
        }
      }
    }


    #endregion

    #region Global Variables  

    private bool _isNewUser;

    public const int CANNOT_UPLOAD_PHOTO_TIMER = 5000;

    public const long MY_BIO_IMAGE_STYLE = (long)(BioImageStyle.Zoom);
    public const long MIN_BIO_IMAGE_STYLE = (long)(BioImageStyle.Zoom | BioImageStyle.CancelBtn);
    public const long MAX_BIO_IMAGE_STYLE = (long)(  BioImageStyle.Zoom               | BioImageStyle.CancelBtn
                                                   | BioImageStyle.Arrows             | BioImageStyle.BioSelector
                                                   | BioImageStyle.EnrollmentFromFile | BioImageStyle.Information
                                                   | BioImageStyle.LiveEnrollment     | BioImageStyle.Btn3D
                                                   | BioImageStyle.FingerBtn          | BioImageStyle.FacialBtn | BioImageStyle.IrisBtn);

    public const long NEW_USER_BIO_IMAGE_STYLE = (long)( BioImageStyle.Zoom               | BioImageStyle.CancelBtn
                                                       | BioImageStyle.Arrows             | BioImageStyle.BioSelector
                                                       | BioImageStyle.EnrollmentFromFile 
                                                       | BioImageStyle.LiveEnrollment     | BioImageStyle.FacialBtn );

    private readonly INotifier _notifier;
    private readonly IBioSkyNetRepository _database;


    public delegate void BioImageChangedEventHandler(BioImageModelType bioImageModel);

    public event BioImageChangedEventHandler BioImageModelChanged;

    public delegate void PhotoChangedEventHandler();

    public event PhotoChangedEventHandler PhotoChanged;
    #endregion
  }

}
