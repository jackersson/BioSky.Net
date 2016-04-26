using BioModule.ResourcesLoader;
using BioModule.Utils;
using BioModule.ViewModels;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using BioContracts.FingerprintDevices;
using BioContracts;
using BioContracts.Common;

namespace BioModule.BioModels
{
  public class FingersImageModel : PropertyChangedBase, IBioImageModel, IFingerprintDeviceObserver
  {
    public FingersImageModel(IProcessorLocator locator, IImageViewUpdate imageView)
    {
      FingerInformation = new FingerInformationViewModel();
      EnrollmentBar     = new FingerprintEnrollmentBarViewModel(locator);

      _imageView = imageView;
    }

    public void Activate()
    {
      EnrollmentBar.Subscribe(this);
      _imageView.SetSingleImage(FingerImageSource);
    }

    public void Deactivate()
    {
      //EnrollmentViewModel.SelectedDeviceChanged -= EnrollmentViewModel_SelectedDeviceChanged;
      //EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
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
    
    public void UpdateFrame( Bitmap frame)
    {
      if (frame == null)
      {
        _imageView.SetSingleImage(null);
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
          EnrollmentBar.UpdateSelector((IFingerSelector)_controller);
          NotifyOfPropertyChange(() => Controller);
        }
      }
    }

    BioImageModelType IBioImageModel.BioType
    {
      get { return BioImageModelType.Fingers; }
    }
        

    private IImageViewUpdate _imageView;
  }
}
