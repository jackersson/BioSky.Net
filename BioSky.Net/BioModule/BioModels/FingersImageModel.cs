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

namespace BioModule.BioModels
{
  public class FingersImageModel : PropertyChangedBase, IBioImageModel
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

    public void UpdateFrame( Bitmap frame)
    {
      throw new NotImplementedException();
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
}
