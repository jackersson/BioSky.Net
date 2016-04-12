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
  
  public interface IBioImageModel
  {
    void Activate();
    void Deactivate();
    void Reset();
    void UpdateController(IUserBioItemsController controller);

    void UpdateFrame( Bitmap frame);
    void UploadPhoto(Photo photo);
    object GetInformation();   
    PhotoViewEnum EnumState                 { get; }
    IUserBioItemsController Controller      { get; }
  }

  public enum BioImageStyle : long
  {  
    Zoom               = 1 << 0
  , BioSelector        = 1 << 1
  , LiveEnrollment     = 1 << 2
  , EnrollmentFromFile = 1 << 3
  , Information        = 1 << 4
  , Arrows             = 1 << 5
  , CancelBtn          = 1 << 6
  }

  public enum PhotoViewEnum
  {
     Faces
   , Irises
   , Fingers
  }

  public class BioImageViewModel : ImageViewModel, IUserBioItemsUpdatable
  {

    public BioImageViewModel(IProcessorLocator locator, long style = MAX_BIO_IMAGE_STYLE) 
    {
      _notifier = locator.GetProcessor<INotifier>();
      _database = locator.GetProcessor<IBioSkyNetRepository>();

      BioImageModels = new ObservableCollection<IBioImageModel>();

      BioImageModels.Add(new FacesImageModel  (locator, this));
      BioImageModels.Add(new FingersImageModel(locator, this));
      BioImageModels.Add(new IrisesImageModel(this));

      BioImageDetails = new PhotoInfoExpanderViewModel();

      SetStyle(style);

      // UpdateFromPhoto(GetTestPhoto());

      ChangeBioImageModel(PhotoViewEnum.Faces);

    }

    public void ChangeBioImageModel(PhotoViewEnum state)
    {
      if (CurrentBioImage != null && CurrentBioImage.EnumState == state)
        return;

      foreach (IBioImageModel view in BioImageModels)
      {
        if (view.EnumState == state)
        {
          CurrentBioImage = view;
          BioImageDetails.Update(CurrentBioImage.GetInformation());          
          CurrentBioImage.Activate();          
        }
        else
          view.Deactivate();
      }
    }

    public void UpdateFrame ( Bitmap frame )
    {
      if (CurrentBioImage != null)
        CurrentBioImage.UpdateFrame( frame);
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
        CurrentBioImage.Activate();

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

    private void OnBioImageChanged(PhotoViewEnum bioImageModel)
    {
      if (BioImageModelChanged != null)
        BioImageModelChanged(bioImageModel);
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

          OnBioImageChanged(CurrentBioImage.EnumState);
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
          NotifyOfPropertyChange(() => ControlStyle);
          NotifyOfPropertyChange(() => EnrollFromPhotoVisibility);
          NotifyOfPropertyChange(() => LiveEnrollmentVisibility);
          NotifyOfPropertyChange(() => CancelButtonVisibility);
          NotifyOfPropertyChange(() => EnrollExpanderVisibility);
          NotifyOfPropertyChange(() => BioImageDetailsVisibility);
          NotifyOfPropertyChange(() => BioImageSelectorVisibility);
          NotifyOfPropertyChange(() => ArrowsVisibility);
          NotifyOfPropertyChange(() => ZoomVisibility);
        }
      }
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

         // if (_currentPhoto != null && _database.Persons.PhotosIndexesWithoutExistingFile.Contains(_currentPhoto.Id))          
           // Message = "Can't upload photo";

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
    #endregion

    #region Global Variables    

    public const long MIN_BIO_IMAGE_STYLE = (long)(BioImageStyle.Zoom | BioImageStyle.CancelBtn);
    public const long MAX_BIO_IMAGE_STYLE = (long)(BioImageStyle.Zoom | BioImageStyle.CancelBtn
                                                   | BioImageStyle.Arrows | BioImageStyle.BioSelector
                                                   | BioImageStyle.EnrollmentFromFile | BioImageStyle.Information
                                                   | BioImageStyle.LiveEnrollment);

    private readonly INotifier _notifier;
    private readonly IBioSkyNetRepository _database;

    public delegate void BioImageChangedEventHandler(PhotoViewEnum bioImageModel);

    public event BioImageChangedEventHandler BioImageModelChanged;
    #endregion
  }

}
