using AForge.Video.DirectShow;
using BioContracts;
using BioContracts.Common;
using BioModule.ResourcesLoader;
using BioModule.Utils;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioModule.ViewModels
{
  public delegate void SelectedDeviceChangedEventHandler();
  public class EnrollmentBarViewModel : Screen
  {

    public event SelectedDeviceChangedEventHandler SelectedDeviceChanged;

    public EnrollmentBarViewModel(IProcessorLocator locator)
    {
      _deviceEngine  = locator.GetProcessor<ICaptureDeviceEngine>();
      _dialogsHolder = locator.GetProcessor<DialogsHolder>();

      DevicesNames = _deviceEngine.GetCaptureDevicesNames();

      DeviceObserver = new TrackLocationCaptureDeviceObserver(locator);
      Resolution     = new AsyncObservableCollection<string>();

      DeviceObserver.DeviceChanged += UpdateVideoCapabilities;

      DeviceConnected = false;
    }

    private void OnSelectedDeviceChanged()
    {
      if (SelectedDeviceChanged != null)
        SelectedDeviceChanged();
    }

    private void DevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }

    public string AvaliableDevicesCount
    {
      get { return String.Format("Available Devices ({0})", _devicesNames.Count); }
    }

    public bool CaptureDevicePropertyPageVisibility
    {
      get { return true; }
    }

    protected override void OnActivate()
    {
      DevicesNames = _deviceEngine.GetCaptureDevicesNames();

      DevicesNames.CollectionChanged += DevicesNames_CollectionChanged;  

      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      DevicesNames.CollectionChanged -= DevicesNames_CollectionChanged;
    
      base.OnDeactivate(close);
    }

    private void DeviceObserver_AccessDeviceState(bool status)
    {
      DeviceConnected = status;
    }

    private void UpdateVideoCapabilities()
    {
      if (DeviceObserver.DeviceName == null)
        return;

      VideoCapabilities[] videoCapabilities =  DeviceObserver.GetVideoCapabilities();

      if (videoCapabilities == null)
        return;

      VideoCapabilities currentVideoResolution = DeviceObserver.GetVideoResolution();      

      Resolution.Clear();
      int i = 0;
      foreach (VideoCapabilities vc in videoCapabilities)
      {
        string item = string.Format("{0}x{1}, {2} fps"
                                    , vc.FrameSize.Width
                                    , vc.FrameSize.Height
                                    , vc.AverageFrameRate);

        Resolution.Add(item);

        if (vc == currentVideoResolution)
          SelectedResolution = i;

        i++;
      }

      NotifyOfPropertyChange(() => Resolution);
    }

    private void ApplyVideoDeviceCapability()
    {
      DeviceObserver.SetVideoCapabilities(SelectedResolution);
    }

    #region UI

    private int _selectedResolution;
    public int SelectedResolution
    {
      get { return _selectedResolution; }
      set
      {
        if (_selectedResolution != value)
        {
          _selectedResolution = value;
          NotifyOfPropertyChange(() => SelectedResolution);

          ApplyVideoDeviceCapability();
        }
      }
    }

    private AsyncObservableCollection<string> _resolution;
    public AsyncObservableCollection<string> Resolution
    {
      get { return _resolution; }
      set
      {
        if (_resolution != value)
        {
          _resolution = value;
          NotifyOfPropertyChange(() => Resolution);
        }
      }
    }

    private TrackLocationCaptureDeviceObserver _deviceObserver;
    public TrackLocationCaptureDeviceObserver DeviceObserver
    {
      get { return _deviceObserver; }
      private set
      {
        if (_deviceObserver != value)
          _deviceObserver = value;
      }
    }

    private AsyncObservableCollection<string> _devicesNames;
    public AsyncObservableCollection<string> DevicesNames
    {
      get { return _devicesNames; }
      set
      {
        if (_devicesNames != value)
        {
          _devicesNames = value;
          NotifyOfPropertyChange(() => DevicesNames);
        }
      }
    }

    public BitmapSource DeviceConnectedIcon
    {
      get { return DeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }

    private string _selectedDevice;
    public string SelectedDevice
    {
      get { return _selectedDevice; }
      set
      {
        if (_selectedDevice != value)
        {
          _selectedDevice = value;

          DeviceObserver.Update(_selectedDevice);

          OnSelectedDeviceChanged();

          NotifyOfPropertyChange(() => SelectedDevice);
          //Subscribe();
        }
      }
    }

    private bool _deviceConnected;
    public bool DeviceConnected
    {
      get { return _deviceConnected; }
      set
      {
        if (_deviceConnected != value)
        {
          _deviceConnected = value;
          NotifyOfPropertyChange(() => DeviceConnectedIcon);
        }
      }
    }

    #endregion

    #region Global Variables

    private readonly DialogsHolder        _dialogsHolder;
    private readonly ICaptureDeviceEngine _deviceEngine;

    #endregion

  }
}
