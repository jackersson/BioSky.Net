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
using System.Drawing;

namespace BioModule.ViewModels
{
  public delegate void SelectedDeviceChangedEventHandler();
  public class FaceEnrollmentBarViewModel : Screen, ICaptureDeviceObserver
  {

    public event SelectedDeviceChangedEventHandler SelectedDeviceChanged;

    public FaceEnrollmentBarViewModel(IProcessorLocator locator)
    {
      _captureDeviceEngine  = locator.GetProcessor<ICaptureDeviceEngine>();
      _dialogsHolder        = locator.GetProcessor<DialogsHolder>();
      Resolution            = new AsyncObservableCollection<string>();      
    }
    
    private void DevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)  {
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }

    public string AvaliableDevicesCount {
      get {
        if (DevicesNames == null)
          DevicesNames = _captureDeviceEngine.GetDevicesNames();

        return String.Format("Available Devices ({0})", _devicesNames.Count); }
    }
    
    protected override void OnActivate()
    {
      DevicesNames                    = _captureDeviceEngine.GetDevicesNames();
      DevicesNames.CollectionChanged += DevicesNames_CollectionChanged;

      StartCaptureDevice();

      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      DevicesNames.CollectionChanged -= DevicesNames_CollectionChanged;
      StopCaptureDevice();
      base.OnDeactivate(close);
    }

    private void StartCaptureDevice()
    {
      if (string.IsNullOrEmpty(SelectedDevice))
        return;

      Resolution.Clear();
      _captureDeviceEngine.Add(SelectedDevice);
      _captureDeviceEngine.Subscribe(this, SelectedDevice);
    }

    private void StopCaptureDevice()
    {

      Resolution.Clear();
      _captureDeviceEngine.Unsubscribe(this);
      _captureDeviceEngine.Remove(SelectedDevice);     
    }

    private void ApplyVideoDeviceCapability() {
      _captureDeviceEngine.ApplyResolution(SelectedDevice, SelectedResolution);     
    }

    public void OnFrame(ref Bitmap frame) {}

    public void OnStop(bool stopped, string message) { NotifyOfPropertyChange(() => DeviceConnectedIcon);   }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all)
    {
      NotifyOfPropertyChange(() => DeviceConnectedIcon);
      UpdateResolution(active, all);
    }

    private void UpdateResolution(VideoCapabilities active, VideoCapabilities[] all)
    {
      if (all == null)
        return;

      Resolution.Clear();
      int i = 0;
      foreach (VideoCapabilities vc in all)
      {
        string item = string.Format("{0}x{1}, {2} fps"
                                    , vc.FrameSize.Width
                                    , vc.FrameSize.Height
                                    , vc.AverageFrameRate);

        Resolution.Add(item);

        if (vc == active)
          SelectedResolution = i;

        i++;
      }

      NotifyOfPropertyChange(() => Resolution);
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
      get { return _captureDeviceEngine.IsDeviceActive(SelectedDevice) ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }

    private string _selectedDevice;
    public string SelectedDevice
    {
      get { return _selectedDevice; }
      set
      {
        if (_selectedDevice != value)
        {
          StopCaptureDevice();
          _selectedDevice = value;
          StartCaptureDevice();
      
          NotifyOfPropertyChange(() => SelectedDevice);
         
        }
      }
    }  
    #endregion

    #region Global Variables
    private readonly DialogsHolder        _dialogsHolder;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    #endregion

  }
}
