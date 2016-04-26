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
  public class IrisesImageModel : PropertyChangedBase, IBioImageModel, IIrisDeviceObserver
  {
    public IrisesImageModel(IProcessorLocator locator, IImageViewUpdate imageView)
    {
      Information         = new IrisInformationViewModel();
      EnrollmentBar       = new IrisEnrollmentBarViewModel(locator);

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
    public void Activate()
    {
      EnrollmentBar.Unsubscribe(this);
      EnrollmentBar.Subscribe(this);

      BitmapSource targetLeftEyeImage  = _leftEyeHolder .Unmarked == null ? ResourceLoader.IrisScanImageIconSource : _leftEyeHolder.Unmarked ;
      BitmapSource targetRightEyeImage = _rightEyeHolder.Unmarked == null ? ResourceLoader.IrisScanImageIconSource : _rightEyeHolder.Unmarked;

      _imageView.SetDoubleImage(targetLeftEyeImage, targetRightEyeImage);
    }

    public void Deactivate()
    {
      EnrollmentBar.Unsubscribe(this);
    }

    public void ShowDetails(bool state)
    {
      if (state)
        DrawIrisCharacteristics();
      else
        _imageView.SetDoubleImage(_leftEyeHolder.Unmarked, _rightEyeHolder.Unmarked);
    }

    public void DrawIrisCharacteristics()
    {
      _leftEyeHolder .Unmarked = _imageView.GetImageByIndex(0);
      _rightEyeHolder.Unmarked = _imageView.GetImageByIndex(1);

      Bitmap detailedLeftEye  = _marker.DrawIrisCharacteristics(BitmapConversion.BitmapSourceToBitmap(_leftEyeHolder.Unmarked ));
      Bitmap detailedRightEye = _marker.DrawIrisCharacteristics(BitmapConversion.BitmapSourceToBitmap(_rightEyeHolder.Unmarked));

      _leftEyeHolder .Marked = BitmapConversion.BitmapToBitmapSource(detailedLeftEye);
      _rightEyeHolder.Marked = BitmapConversion.BitmapToBitmapSource(detailedRightEye);

      _imageView.SetDoubleImage(_leftEyeHolder.Marked, _rightEyeHolder.Marked);
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
    #endregion

    #region Global Variables
    private IImageViewUpdate         _imageView     ;
    private MarkerUtils              _marker        ;
    private MarkerBitmapSourceHolder _leftEyeHolder ;
    private MarkerBitmapSourceHolder _rightEyeHolder;

    #endregion
  }
}
