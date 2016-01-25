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
using System.Collections.ObjectModel;
using System.Reflection;


namespace BioModule.ViewModels
{
  public class UserPhotoViewModel : Screen, IObserver<AccessDeviceActivity>
  {
    public UserPhotoViewModel(IBioEngine bioEngine, IScreen screen)
    {
      _bioEngine = bioEngine;
      _screen = screen;
      DisplayName = "Photo";

      AccessDevicesNames = _bioEngine.AccessDeviceEngine().GetAccessDevicesNames();

      AccessDevicesNames.CollectionChanged += AccessDevicesNames_CollectionChanged;

      AccessDeviceConnected = false;


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
      if(SelectedItem != null)
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
    private readonly IScreen _screen;
  }

  
}
