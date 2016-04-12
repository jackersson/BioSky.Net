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

namespace BioModule.BioModels
{
  public class IrisesImageModel : PropertyChangedBase, IBioImageModel
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

    public void UpdateFrame( Bitmap frame)
    {
      throw new NotImplementedException();
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
}
