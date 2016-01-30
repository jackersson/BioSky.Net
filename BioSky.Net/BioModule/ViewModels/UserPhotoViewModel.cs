using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using BioData;
using BioContracts;

using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.IO;
using AForge.Video.DirectShow;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Drawing;
using BioModule.Utils;

using BioFaceService;
using BioContracts.Services;
using Microsoft.Win32;

namespace BioModule.ViewModels
{

  public class Enroller
  {
    private BioImagesList _bioImagesList = new BioImagesList();
    public Enroller( IProcessorLocator locator)
    {
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
    }

    private void Reset()
    {
      _busy = false;
      _bioImagesList.Images.Clear();
    }


    public void Start( Bitmap bitmap)
    {
      if (_busy)
        return;

      Reset();

      _busy = true;
      BioImage image = ImageToBioImage(bitmap);
      _bioImagesList.Images.Add(image);

      EnrollRequest();
    }

    public void Start( string captureDeviceName )
    {
      if (_busy)
        return;

      _captureDeviceName = captureDeviceName;

      Reset();
      _busy = true;

      _captureDeviceEngine.Subscribe(CollectCapturedFrame, captureDeviceName);
    }

    public byte[] ImageToByte(Image img)
    {
      byte[] byteArray = new byte[0];
      using (MemoryStream stream = new MemoryStream())
      {
        img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
        stream.Close();

        byteArray = stream.ToArray();
      }
      return byteArray;
    }

    private Google.Protobuf.ByteString ImageToByteString(Image img)
    {
      byte[] bytes = ImageToByte(img);
      return Google.Protobuf.ByteString.CopyFrom(bytes);
    }

    private BioImage ImageToBioImage(Image img)
    {
      Google.Protobuf.ByteString description = ImageToByteString(img);
      return new BioImage() { Description = description };
    }

    private void CollectCapturedFrame(object sender, ref Bitmap bitmap)
    {      
      BioImage image = ImageToBioImage(bitmap);
      _bioImagesList.Images.Add(image);

      if (_bioImagesList.Images.Count > MAX_ENROLL_IMAGES_COUNT)
      {        
        _captureDeviceEngine.Unsubscribe(CollectCapturedFrame, _captureDeviceName);
        EnrollRequest();
      }
    }

    private async void EnrollRequest()
    {
      IServiceManager bioService = _locator.GetProcessor<IServiceManager>();
      await bioService.FaceService.Enroll(_bioImagesList);

      _busy = false;
    }

    private string    _captureDeviceName;
    private const int MAX_ENROLL_IMAGES_COUNT = 1;

    private bool _busy = false;

    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IProcessorLocator    _locator            ;
  }

  public class UserPhotoViewModel : Screen
  {
    public UserPhotoViewModel(IBioEngine bioEngine, IImageUpdatable imageViewer, IProcessorLocator locator)
    {
      _locator             = locator;
      _bioEngine           = bioEngine;
      _imageViewer         = imageViewer;
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      
      DisplayName = "Photo";

      _enroller = new Enroller(locator);

      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();    
    }

    protected override void OnActivate()
    {
      CaptureDeviceConnected = false;
   
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      CaptureDeviceConnected = false;

      if (ActiveCaptureDevice != null)
        _captureDeviceEngine.Unsubscribe(OnNewFrame, ActiveCaptureDevice);

      SelectedCaptureDevice = null;
      _imageViewer.Clear();

      CaptureDevicesNames.CollectionChanged -= CaptureDevicesNames_CollectionChanged;

      base.OnDeactivate(close);
    }

    public void OnSelectionChange()
    {
      if (SelectedItem != null)      
        _imageViewer.UpdateImage(SelectedItem);       
    }

    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      if (bitmap == null)
        return;

      CaptureDeviceConnected = true;

      _imageViewer.UpdateImage(ref bitmap);     
    }

    public void Subscribe()
    {
      CaptureDeviceConnected = false;
      if (SelectedCaptureDevice == null)       
        return;      

      if (ActiveCaptureDevice != null)      
        _captureDeviceEngine.Unsubscribe(OnNewFrame, ActiveCaptureDevice);           

      ActiveCaptureDevice = SelectedCaptureDevice;

      if (!_captureDeviceEngine.CaptureDeviceActive(ActiveCaptureDevice))      
        _captureDeviceEngine.Add(ActiveCaptureDevice);      
      
      _captureDeviceEngine.Subscribe(OnNewFrame, ActiveCaptureDevice);
    }

    public void EnrollFromCamera()
    {
      // Console.Write("Enroll Camera");
      if (CaptureDeviceConnected)
        _enroller.Start(ActiveCaptureDevice);
    }

    public void UploadClick()
    {

      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
      openFileDialog.InitialDirectory = Environment.CurrentDirectory;

      if (openFileDialog.ShowDialog() == true)
      {
        string filename = openFileDialog.FileName;
        if (File.Exists(filename))
        {
          Bitmap bmp = (Bitmap)Image.FromFile(filename);        
          _enroller.Start(bmp);
        }        
      }
    }

    public void EnrollFromPhoto()
    {
      UploadClick();
    }

    public void DeletePhoto()
    {
      Console.Write("Delete Photos");
    }    

    private string _activeCaptureDevice;
    public string ActiveCaptureDevice
    {
      get { return _activeCaptureDevice; }
      set
      {
        if (_activeCaptureDevice != value)
        {
          _activeCaptureDevice = value;

          NotifyOfPropertyChange(() => ActiveCaptureDevice);
        }
      }
    }

    private ObservableCollection<Uri> _robotImages;
    public ObservableCollection<Uri> RobotImages
    {
      get { return _robotImages; }
      set
      {
        if (_robotImages != value)
        {
          _robotImages = value;

          NotifyOfPropertyChange(() => RobotImages);
        }
      }
    }

    private Uri _selectedItem;
    public Uri SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem != value)
        {
          _selectedItem = value;

          NotifyOfPropertyChange(() => SelectedItem);
        }
      }
    }

    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CaptureDevicesNames);
      if(ActiveCaptureDevice == null)
        NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }

    private AsyncObservableCollection<string> _captureDevicesNames;
    public AsyncObservableCollection<string> CaptureDevicesNames
    {
      get { return _captureDevicesNames; }
      set
      {
        if (_captureDevicesNames != value)
        {
          _captureDevicesNames = value;

          NotifyOfPropertyChange(() => CaptureDevicesNames  );          
        }
      }
    }

    public string AvaliableDevicesCount
    {
      get { return String.Format("Available Devices ({0})", _captureDevicesNames.Count); }
    }

    private string _selectedCaptureDevice;
    public string SelectedCaptureDevice
    {
      get { return _selectedCaptureDevice; }
      set
      {
        if (_selectedCaptureDevice != value)
        {
          _selectedCaptureDevice = value;
          NotifyOfPropertyChange(() => SelectedCaptureDevice);

          Subscribe();
        }
      }
    }

    private bool _captureDeviceConnected;
    private bool CaptureDeviceConnected
    {
      get { return _captureDeviceConnected; }
      set
      {
        if (_captureDeviceConnected != value)
        {
          _captureDeviceConnected = value;
          NotifyOfPropertyChange(() => CaptureDeviceConnectedIcon);
        }
      }
    }

    public BitmapSource CaptureDeviceConnectedIcon
    {
      get { return CaptureDeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }

    private readonly Enroller             _enroller           ;
    private readonly IProcessorLocator    _locator            ;
    private readonly IBioEngine           _bioEngine          ;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IImageUpdatable      _imageViewer        ;
  }
}
