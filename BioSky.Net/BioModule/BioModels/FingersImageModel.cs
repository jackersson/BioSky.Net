using BioModule.ResourcesLoader;
using BioModule.Utils;
using BioModule.ViewModels;
using BioService;
using Caliburn.Micro;
using System;
using System.Windows.Media.Imaging;
using System.Drawing;
using BioContracts.FingerprintDevices;
using BioContracts;

namespace BioModule.BioModels
{
  public class FingersImageModel : Conductor<IScreen>.Collection.AllActive, IBioImageModel, IFingerprintDeviceObserver
  {
    public FingersImageModel(IProcessorLocator locator, IImageViewUpdate imageView)
    {
      FingerInformation = new FingerInformationViewModel();
      EnrollmentBar     = new FingerprintEnrollmentBarViewModel(locator);

      _imageView = imageView;
    }

    public void Activate(bool isNewUser)
    {
      EnrollmentBar.Unsubscribe(this);
      EnrollmentBar.Subscribe(this);
      (EnrollmentBar as IScreen).Activate();

      _imageView.SetSingleImage(FingerImageSource);
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

    public void Reset()
    {

    }

    public void UploadPhoto(Photo photo)
    {
      
    }

    public object GetInformation()
    {
      return FingerInformation;
    }

    public void UpdateController(IUserBioItemsController controller)
    {
      if (controller != null)
      {
        Controller = controller;
        if (_controller is IFingerSelector)
          EnrollmentBar.UpdateSelector(_controller as IFingerSelector);
      }
    }
    
    public void UpdateFrame( Bitmap frame)
    {
      if (frame == null)
      {
        //_imageView.SetSingleImage(null);
        return;
      }
      
      BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(frame);
      _imageView.SetSingleImage(newFrame);
    }

    public void OnFrame(ref Bitmap frame)
    {
      UpdateFrame(frame);
    }

    public void OnError(Exception ex) { }

    public void OnMessage(string message) {}

    public void OnReady(bool isReady) { }

    public void ShowDetails(bool state)
    {
    
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

    private FingerprintEnrollmentBarViewModel _enrollmentBar;
    public FingerprintEnrollmentBarViewModel EnrollmentBar
    {
      get { return _enrollmentBar; }
      set
      {
        if (_enrollmentBar != value)
        {
          _enrollmentBar = value;
          NotifyOfPropertyChange(() => EnrollmentBar);
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

    BioImageModelType IBioImageModel.BioType
    {
      get { return BioImageModelType.Fingers; }
    }

    private bool _isActive;
    public bool IsActive
    {
      get
      {
        return _isActive;
      }
    }


    private IImageViewUpdate _imageView;
  }
}
