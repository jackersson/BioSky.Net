using AForge.Video.DirectShow;
using BioContracts;
using BioContracts.CaptureDevices;
using BioContracts.Common;
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
using System.Windows.Threading;

namespace BioModule.BioModels
{
  public class FacesImageModel : PropertyChangedBase, IBioImageModel, ICaptureDeviceObserver
  {
    public FacesImageModel(IProcessorLocator locator, IImageViewUpdate imageView)
    {
      PhotoInformation = new PhotoInformationViewModel();
      ExpanderBarModel = new FaceEnrollmentBarViewModel(locator);
      _marker = new MarkerUtils();
      _faceFinder = new FaceFinder();
      _uiDispatcher = locator.GetProcessor<Dispatcher>();
      _imageView = imageView;
    }

    public void Activate()
    {
      ExpanderBarModel.Subscribe(this);
      // EnrollmentViewModel.SelectedDeviceChanged += EnrollmentViewModel_SelectedDeviceChanged;

      //if (EnrollmentViewModel.DeviceObserver.DeviceName != null)
      //  EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);
      // else
      _imageView.SetSingleImage(PhotoImageSource);
    }

    public void Deactivate()
    {
     
      //EnrollmentViewModel.SelectedDeviceChanged -= EnrollmentViewModel_SelectedDeviceChanged;
      //EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
    }
    private void EnrollmentViewModel_SelectedDeviceChanged()
    {
      //  EnrollmentViewModel.DeviceObserver.Unsubscribe(OnNewFrame);
      //  EnrollmentViewModel.DeviceObserver.Subscribe(OnNewFrame);
    }
    /*
    public void UpdateFromImage(ref Bitmap img)
    {
      if (img == null)
        return;

      Bitmap processedFrame = DrawFaces(ref img);

     // _imageView.UpdateOneImage(ref processedFrame); 
    }
    */
    public Bitmap DrawFaces(ref Bitmap img)
    {
      return _marker.DrawRectangles(_faceFinder.GetFaces(ref img), ref img);
    }
    /*
    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      UpdateFromImage(ref bitmap);
    }
    */

    public void Reset()
    {

    }

    public void UploadPhoto(Photo photo)
    {

    }

    public void UpdateController(IUserBioItemsController controller)
    {
      if (controller != null)
        Controller = controller;
    }

    public object GetInformation()
    {
      return PhotoInformation;
    }
    
    public void OnFrame(ref Bitmap frame)
    {
     /// Bitmap temp = frame;
     UpdateFrame(frame);
       //  _uiDispatcher.Invoke(() => UpdateFrame(temp));
    }
    
    public void OnStop(bool stopped, string message) { _imageView.SetSingleImage(null); }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) { }

    public void UpdateFrame( Bitmap frame)
    {
      if (frame == null)
      {
        _imageView.SetSingleImage(null);
        return;
      }

      Bitmap processedFrame = DrawFaces(ref frame);
      BitmapSource newFrame = BitmapConversion.BitmapToBitmapSource(processedFrame);   

     // Console.WriteLine(_uiDispatcher.GetHashCode());
      //Console.WriteLine(Dispatcher.CurrentDispatcher.GetHashCode());

      _imageView.SetSingleImage(newFrame);
    }

    public PhotoViewEnum EnumState
    {
      get { return PhotoViewEnum.Faces; }
    }
    public BitmapSource SettingsToogleButtonBitmap
    {
      get { return ResourceLoader.UserFacesIconSource; }
    }

    private BitmapSource _photoImageSource;
    public BitmapSource PhotoImageSource
    {
      get
      {
        if (_photoImageSource == null)
          _photoImageSource = ResourceLoader.UserDefaultImageIconSource;
        return _photoImageSource;
      }
      set
      {
        if (_photoImageSource != value)
        {
          _photoImageSource = value;
          NotifyOfPropertyChange(() => PhotoImageSource);
        }
      }
    }

    private PhotoInformationViewModel _photoInformation;
    public PhotoInformationViewModel PhotoInformation
    {
      get { return _photoInformation; }
      set
      {
        if (_photoInformation != value)
        {
          _photoInformation = value;
          NotifyOfPropertyChange(() => PhotoInformation);
        }
      }
    }

    private FaceEnrollmentBarViewModel _expanderBarModel;
    public FaceEnrollmentBarViewModel ExpanderBarModel
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
    private readonly Dispatcher _uiDispatcher;

    private MarkerUtils _marker;
    private FaceFinder _faceFinder;
    private IImageViewUpdate _imageView;
  }

}
