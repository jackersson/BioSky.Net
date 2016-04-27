using BioModule.ResourcesLoader;
using BioModule.Utils;
using BioModule.ViewModels;

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using BioService;
using System.Drawing;
using BioContracts.IrisDevices;
using BioContracts;

namespace BioModule.BioModels
{
  public interface IEyeSelector
  {
    EyeType SelectedEye { get; set; }
  }

  public class IrisesImageModel : PropertyChangedBase, IBioImageModel, IIrisDeviceObserver, IEyeSelector
  {
    public IrisesImageModel(IProcessorLocator locator, IImageViewUpdate imageView)
    {
      Information         = new IrisInformationViewModel();
      EnrollmentBar       = new IrisEnrollmentBarViewModel(locator, this);

      _marker             = new MarkerUtils();

      _leftEyeHolder   = new MarkerBitmapSourceHolder();
      _rightEyeHolder  = new MarkerBitmapSourceHolder();

      _imageView = imageView;
    }
    public void UploadPhoto(Photo photo)
    {
      throw new NotImplementedException();
    }
    public void UpdateController(IUserBioItemsController controller)
    {
      if (controller != null)
        Controller = controller;
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

      SelectEye(SelectedEye);

      _isActive = true;
      NotifyOfPropertyChange(() => IsActive);
    }

    public void Deactivate()
    {
      EnrollmentBar.Unsubscribe(this);
      if (EnrollmentBar is IScreen)
      {
        IScreen screen = EnrollmentBar as IScreen;
        screen.Deactivate(false);
      }
      _isActive = false;
      NotifyOfPropertyChange(() => IsActive);
    }

    public void ShowDetails(bool state)
    {
      //_isShowDetails = state;
      if (state)
        DrawIrisCharacteristics();
      else
        SelectEye(SelectedEye);
    }


    public void DrawIrisCharacteristics()
    {
      if(SelectedEye != EyeType.Right)
      {
        _leftEyeHolder.Unmarked = _imageView.GetImageByIndex(0);
        Bitmap detailedLeftEye = _marker.DrawIrisCharacteristics(BitmapConversion.BitmapSourceToBitmap(_leftEyeHolder.Unmarked));
        _leftEyeHolder.Marked = BitmapConversion.BitmapToBitmapSource(detailedLeftEye);
      }

      if (SelectedEye != EyeType.Left)
      {
        _rightEyeHolder.Unmarked = _imageView.GetImageByIndex(1);
        Bitmap detailedRightEye = _marker.DrawIrisCharacteristics(BitmapConversion.BitmapSourceToBitmap(_rightEyeHolder.Unmarked));
        _rightEyeHolder.Marked = BitmapConversion.BitmapToBitmapSource(detailedRightEye);
      }

      SelectEye(SelectedEye, true);
    }

    public void UpdateFrame(Bitmap frame)
    {
      if (frame == null)
      {
       // _imageView.SetSingleImage(null);
        return;
      }

      BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(frame);
      _imageView.SetSingleImage(newFrame);
      //throw new NotImplementedException();
    }

    public void OnFrame( Bitmap left,  Bitmap right)
    {      
      BitmapSource targetLeft = null;
      if (left != null)
        targetLeft = BitmapConversion.BitmapToBitmapSource(left);

      BitmapSource targetRight = null;
      if (right != null)
        targetRight = BitmapConversion.BitmapToBitmapSource(right);

      _imageView.SetDoubleImage(targetLeft, targetRight); 

      //UpdateFrame(left);
    }

    public void SelectEye(EyeType eye, bool isShowDetails = false)
    {
     // _isShowDetails = isShowDetails;
      SelectedEye = eye;

      BitmapSource targetLeftEyeImage  = _leftEyeHolder .Unmarked == null ? ResourceLoader.IrisScanImageIconSource : _leftEyeHolder .Unmarked;
      BitmapSource targetRightEyeImage = _rightEyeHolder.Unmarked == null ? ResourceLoader.IrisScanImageIconSource : _rightEyeHolder.Unmarked;

      //if (!_isShowDetails)
    //  {
        targetLeftEyeImage  = _leftEyeHolder .Marked  == null ? ResourceLoader.IrisScanImageIconSource : _leftEyeHolder .Marked;
        targetRightEyeImage = _rightEyeHolder.Marked  == null ? ResourceLoader.IrisScanImageIconSource : _rightEyeHolder.Marked;
   //   }

      switch (eye)
      {
        case EyeType.Both:
          _imageView.SetDoubleImage(targetLeftEyeImage, targetRightEyeImage);
          break;
        case EyeType.Left:
          _imageView.SetSingleImage(targetLeftEyeImage);
          break;
        case EyeType.Right:
          _imageView.SetSingleImage(targetRightEyeImage);
          break;
      }
    }

    public void OnIrisQualities(IList<EyeScore> scores)
    {
      Console.WriteLine("OnIrisQualities");
    }

    public void OnEyesDetected(bool detected)
    {
      Console.WriteLine("OnEyesDetected");
    }

    public void OnState(CaptureState captureState)
    {
      Console.WriteLine("OnState");
    }

    public void OnError(Exception ex)
    {
      Console.WriteLine("OnError");
    }

    public void OnMessage(string message)
    {
      Console.WriteLine("OnMesage");
    }

    public void OnReady(bool isReady)
    {
      Console.WriteLine("OnReady");
    }

    #region UI

    public BioImageModelType BioType { get { return BioImageModelType.Irises; } }


    public BitmapSource SettingsToogleButtonBitmap {  get { return ResourceLoader.UserIricesIconSource; }  }

    private IrisInformationViewModel _information;
    public IrisInformationViewModel Information
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

    private IrisEnrollmentBarViewModel _enrollmentBar;
    public IrisEnrollmentBarViewModel EnrollmentBar
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

    private EyeType _selectedEye;
    public EyeType SelectedEye
    {
      get { return _selectedEye; }
      set
      {
        if (_selectedEye != value)
        {
          _selectedEye = value;
          SelectEye(_selectedEye);
        }
      }
    }

    private bool _isActive;
    public bool IsActive
    {
      get
      {
        return _isActive;
      }
    }
    #endregion

    #region Global Variables

   // private bool _isShowDetails;

    private IImageViewUpdate         _imageView     ;
    private MarkerUtils              _marker        ;
    private MarkerBitmapSourceHolder _leftEyeHolder ;
    private MarkerBitmapSourceHolder _rightEyeHolder;

    #endregion
  }
}
