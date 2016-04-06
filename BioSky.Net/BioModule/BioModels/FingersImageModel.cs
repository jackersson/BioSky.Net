using BioModule.ResourcesLoader;
using BioModule.Utils;
using BioModule.ViewModels;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioModule.BioModels
{
  public class FingersImageModel : PropertyChangedBase, IBioImageModel
  {
    public FingersImageModel(IImageViewUpdate imageView)
    {
      Information         = new FingerInformationViewModel();
      ExpanderBarModel    = new FingerBarViewModel();
      _marker             = new MarkerUtils();
      _markerBitmapHolder = new MarkerBitmapSourceHolder();

      _imageView = imageView;
    }

    public void Activate()
    {
      if(_markerBitmapHolder.Unmarked == null)
        _imageView.SetSingleImage(ResourceLoader.FingerScanIconSource);
      else
        _imageView.SetSingleImage(_markerBitmapHolder.Unmarked);
    }

    public void Deactivate()
    {

    }

    public void ShowDetails(bool state)
    {
      if (state)
        DrawFingerCharacteristics();
      else
        _imageView.SetSingleImage(_markerBitmapHolder.Unmarked);
    }

    public void DrawFingerCharacteristics()
    {
     // if (CurrentPhoto == null)
      //  return;

      _markerBitmapHolder.Unmarked = _imageView.GetImageByIndex(0);

      Bitmap detailedBitmap = _marker.DrawFingerCharacteristics(BitmapConversion.BitmapSourceToBitmap(_markerBitmapHolder.Unmarked));

      _markerBitmapHolder.Marked = BitmapConversion.BitmapToBitmapSource(detailedBitmap);

      _imageView.SetSingleImage(_markerBitmapHolder.Marked);
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

    #region UI

    public BioImageModelEnum EnumState
    {
      get { return BioImageModelEnum.Fingers; }
    }
    public BitmapSource SettingsToogleButtonBitmap
    {
      get { return ResourceLoader.UserFingerprintIconSource; }
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

    private FingerInformationViewModel _information;
    public FingerInformationViewModel Information
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

    private Photo _currentPhoto;
    public Photo CurrentPhoto
    {
      get
      {
        return _currentPhoto;
      }
      set
      {
        if (_currentPhoto != value)
        {
          _currentPhoto = value;
          Information.Update(_currentPhoto);
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


    #region Global Variables
    private IImageViewUpdate _imageView;
    private MarkerBitmapSourceHolder _markerBitmapHolder;
    private MarkerUtils _marker;
    #endregion
  }
}
