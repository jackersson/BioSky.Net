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
      Information         = new IrisInformationViewModel();
      _marker             = new MarkerUtils();
      _firstMarkerBitmapHolder = new MarkerBitmapSourceHolder();
      _secondMarkerBitmapHolder = new MarkerBitmapSourceHolder();


     _images = new Tuple<MarkerBitmapSourceHolder
                        , MarkerBitmapSourceHolder>(_firstMarkerBitmapHolder
                                                   , _secondMarkerBitmapHolder);
   

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
      if(_images.Item1.Unmarked == null && _images.Item2.Unmarked == null)
        _imageView.SetDoubleImage(ResourceLoader.IrisScanImageIconSource, ResourceLoader.IrisScanImageIconSource);
      else
        _imageView.SetDoubleImage(_images.Item1.Unmarked, _images.Item2.Unmarked);
    }

    public void Deactivate()
    {

    }

    public void ShowDetails(bool state)
    {
      if (state)
        DrawIrisCharacteristics();
      else
        _imageView.SetDoubleImage(_images.Item1.Unmarked, _images.Item2.Unmarked);
    }

    public void DrawIrisCharacteristics()
    {
      // if (CurrentPhoto == null)
      //  return;

      _images.Item1.Unmarked = _imageView.GetImageByIndex(0);
      _images.Item2.Unmarked = _imageView.GetImageByIndex(1);

      Bitmap detailedBitmap1 = _marker.DrawIrisCharacteristics(BitmapConversion.BitmapSourceToBitmap(_images.Item1.Unmarked));
      Bitmap detailedBitmap2 = _marker.DrawIrisCharacteristics(BitmapConversion.BitmapSourceToBitmap(_images.Item2.Unmarked));

      _images.Item1.Marked = BitmapConversion.BitmapToBitmapSource(detailedBitmap1);
      _images.Item2.Marked = BitmapConversion.BitmapToBitmapSource(detailedBitmap2);

      _imageView.SetDoubleImage(_images.Item1.Marked, _images.Item2.Marked);
    }

    #region UI

    public BioImageModelEnum EnumState
    {
      get { return BioImageModelEnum.Irises; }
    }
    public BitmapSource SettingsToogleButtonBitmap
    {
      get { return ResourceLoader.UserIricesIconSource; }
    }

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
    private IImageViewUpdate         _imageView         ;
    private MarkerUtils              _marker            ;
    private MarkerBitmapSourceHolder _firstMarkerBitmapHolder;
    private MarkerBitmapSourceHolder _secondMarkerBitmapHolder;
    private Tuple<MarkerBitmapSourceHolder, MarkerBitmapSourceHolder> _images;

    #endregion
  }
}
