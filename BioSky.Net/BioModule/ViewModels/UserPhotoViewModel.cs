using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using BioData;
using BioContracts;
using BioModule.Model;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.IO;

namespace BioModule.ViewModels
{
  public static class RobotImageLoader
  {
    public static List<BitmapImage> LoadImages()
    {
      List<BitmapImage> robotImages = new List<BitmapImage>();
      DirectoryInfo robotImageDir = new DirectoryInfo(@"C:\Users\Spark\Downloads\CustomItemsPanel\CustomItemsPanel\CustomItemsPanel\Robots");
      foreach (FileInfo robotImageFile in robotImageDir.GetFiles("*.jpg"))
      {
        Uri uri = new Uri(robotImageFile.FullName);
        robotImages.Add(new BitmapImage(uri));
      }
      return robotImages;
    }
  }
  public class UserPhotoViewModel : Screen, IObserver<AccessDeviceActivity>
  {
    public UserPhotoViewModel(IBioEngine bioEngine)
    {
      _bioEngine = bioEngine;

      DisplayName = "Photo";

      AccessDevicesNames = _bioEngine.AccessDeviceEngine().GetAccessDevicesNames();

      AccessDevicesNames.CollectionChanged += AccessDevicesNames_CollectionChanged;

      AccessDeviceConnected = false;


      List<BitmapImage> _robotImages = new List<BitmapImage>();
      DirectoryInfo robotImageDir = new DirectoryInfo(@"C:\Users\Spark\Downloads\CustomItemsPanel\CustomItemsPanel\CustomItemsPanel\Robots");
      foreach (FileInfo robotImageFile in robotImageDir.GetFiles("*.jpg"))
      {
        Uri uri = new Uri(robotImageFile.FullName);
        _robotImages.Add(new BitmapImage(uri));
      }
    }
    public static List<BitmapImage> LoadImages()
    {
      List<BitmapImage> robotImages = new List<BitmapImage>();
      DirectoryInfo robotImageDir = new DirectoryInfo(@"C:\Users\Spark\Downloads\CustomItemsPanel\CustomItemsPanel\CustomItemsPanel\Robots");
      foreach (FileInfo robotImageFile in robotImageDir.GetFiles("*.jpg"))
      {
        Uri uri = new Uri(robotImageFile.FullName);
        robotImages.Add(new BitmapImage(uri));
      }
      return robotImages;
    }

    private List<BitmapImage> _robotImages;
    public List<BitmapImage> RobotImages
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

    private BitmapImage _selectedItem;
    public BitmapImage SelectedItem
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

    override protected void OnActivate()
    {
      Console.WriteLine("Activated " + DisplayName);
    }

    override protected void OnDeactivate(bool canClose)
    {
      Console.WriteLine("Deactivated " + DisplayName);
    }

    private void AccessDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }

    private AsyncObservableCollection<string> _accessDevicesNames;
    public AsyncObservableCollection<string> AccessDevicesNames
    {
      get { return _accessDevicesNames; }
      set
      {
        if (_accessDevicesNames != value)
        {
          _accessDevicesNames = value;

          NotifyOfPropertyChange(() => AccessDevicesNames);
        }
      }
    }

    public string AvaliableDevicesCount
    {
      get { return String.Format("Available Devices ({0})", _accessDevicesNames.Count); }
    }

    private string _selectedAccessDevice;
    public string SelectedAccessDevice
    {
      get { return _selectedAccessDevice; }
      set
      {
        if (_selectedAccessDevice != value)
        {
          _selectedAccessDevice = value;
          NotifyOfPropertyChange(() => SelectedAccessDevice);

          Subscribe();
        }
      }
    }

    public void Subscribe()
    {
      if (_selectedAccessDevice == null)
      {
        AccessDeviceConnected = false;
        return;
      }

      IAccessDeviceEngine accessDeviceEngine = _bioEngine.AccessDeviceEngine();

      accessDeviceEngine.Add(_selectedAccessDevice);

      if (!accessDeviceEngine.HasObserver(this, _selectedAccessDevice))
        accessDeviceEngine.Subscribe(this, _selectedAccessDevice);

      AccessDeviceConnected = accessDeviceEngine.AccessDeviceActive(_selectedAccessDevice);
    }

    private bool _accessDeviceConnected;
    private bool AccessDeviceConnected
    {
      get { return _accessDeviceConnected; }
      set
      {
        if (_accessDeviceConnected != value)
        {
          _accessDeviceConnected = value;
          NotifyOfPropertyChange(() => AccessDeviceConnectedIcon);
        }
      }
    }

    public BitmapSource AccessDeviceConnectedIcon
    {
      get { return AccessDeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }

        public void OnNext(AccessDeviceActivity value)
    {
      AccessDeviceConnected = true;      
             
    }

    public void OnError(Exception error)
    {
      AccessDeviceConnected = false;
    }

    public void OnCompleted()
    {
      throw new NotImplementedException();
    }

    private readonly IBioEngine _bioEngine;
  }
}
