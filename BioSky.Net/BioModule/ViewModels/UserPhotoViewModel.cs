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

namespace BioModule.ViewModels
{
  public class UserPhotoViewModel : Screen
  {
    public UserPhotoViewModel(IBioEngine bioEngine, IScreen screen)
    {
      _bioEngine = bioEngine;
      _screen = screen;

      DisplayName = "Photo";

      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;

      CaptureDeviceConnected = false;

      _robotImages = new ObservableCollection<Uri>();
      DirectoryInfo robotImageDir = new DirectoryInfo(@"C:\Users\Spark\Downloads\CustomItemsPanel\CustomItemsPanel\CustomItemsPanel\Robots");
      foreach (FileInfo robotImageFile in robotImageDir.GetFiles("*.jpg"))
      {
        Uri uri = new Uri(robotImageFile.FullName);

        RobotImages.Add(uri);
      }
      // BitmapSource _logoListIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.tracking);
      //BitmapSource _logoListIconSource2 = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.add_user);

      //RobotImages.Add(_logoListIconSource);
      //RobotImages.Add(_logoListIconSource2);
    }

    public void OnSelectionChange()
    {
      if (SelectedItem != null)
      {
        MethodInfo method = _screen.GetType().GetMethod("UpdatePhoto");
        if (method != null)
          method.Invoke(_screen, new object[] { SelectedItem });
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
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }

    private AsyncObservableCollection<FilterInfo> _captureDevicesNames;
    public AsyncObservableCollection<FilterInfo> CaptureDevicesNames
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

    private string _selectedAccessDevice;
    public string SelectedCaptureDevice
    {
      get { return _selectedAccessDevice; }
      set
      {
        if (_selectedAccessDevice != value)
        {
          _selectedAccessDevice = value;
          NotifyOfPropertyChange(() => SelectedCaptureDevice);          
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

    private readonly IBioEngine _bioEngine;
    private readonly IScreen _screen;
  }
}
