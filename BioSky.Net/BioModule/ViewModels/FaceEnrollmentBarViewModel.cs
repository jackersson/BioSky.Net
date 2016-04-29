using AForge.Video.DirectShow;
using BioContracts;
using BioContracts.Common;
using BioModule.ResourcesLoader;
using BioModule.Utils;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Drawing;
using BioContracts.CaptureDevices;
using BioContracts.Locations;
using BioContracts.BioTasks.Utils;
using BioService;
using System.Threading.Tasks;

namespace BioModule.ViewModels
{
  //public delegate void SelectedDeviceChangedEventHandler();
  public class FaceEnrollmentBarViewModel : Screen, ICaptureDeviceObserver, IBioObservable<ICaptureDeviceObserver>
  {    
    public FaceEnrollmentBarViewModel(IProcessorLocator locator)
    {      
      _captureDeviceEngine  = locator.GetProcessor<ICaptureDeviceEngine>();
      _dialogsHolder        = locator.GetProcessor<DialogsHolder>();
      _observer             = new BioObserver<ICaptureDeviceObserver>();
      Resolution            = new AsyncObservableCollection<string>();

      //_progressRing = progressRing;
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

    public void OnFrame(ref Bitmap frame) {
      foreach (KeyValuePair<int, ICaptureDeviceObserver> observer in _observer.Observers)
        observer.Value.OnFrame(ref frame);
    }

    public void OnStop(bool stopped, Exception ex, LocationDevice device) {

      if (ex.Message != "Stoped by user")
        _progressRing.ShowWaiting(ex.Message);
      NotifyOfPropertyChange(() => DeviceConnectedIcon);
    }    

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all)
    {
      NotifyOfPropertyChange(() => DeviceConnectedIcon);
      UpdateResolution(active, all);
      _progressRing.Hide(2000);
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

    public void OnMessage(string message)
    {
      _progressRing.ShowWaiting(message);
    }

    public void OnSnapshoot()
    {
      if (SelectedDevice == null)
        return;
      _isSnapshootActive = true;
    }

    public async void OnStopDevice()
    {
      SelectedDevice = null;
      await Task.Delay(500);
      OnDeviceStopedByUser();
    }

    #region observer
    public void Subscribe(ICaptureDeviceObserver observer)
    {
      _observer.Subscribe(observer);
    }

    public void Unsubscribe(ICaptureDeviceObserver observer)
    {
      _observer.Unsubscribe(observer);
    }

    public void UnsubscribeAll()
    {
      _observer.UnsubscribeAll();
    }

    public bool HasObserver(ICaptureDeviceObserver observer)
    {
      return _observer.HasObserver(observer);
    }
    #endregion

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

    private bool _enrollButtonVisibility;
    public bool EnrollButtonVisibility
    {
      get { return _enrollButtonVisibility; }
      set
      {
        if (_enrollButtonVisibility != value)
        {
          _enrollButtonVisibility = value;
          NotifyOfPropertyChange(() => EnrollButtonVisibility);
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

    public delegate void DeviceStopedByUserEventHandler();

    public event DeviceStopedByUserEventHandler DeviceStopedByUser;

    private void OnDeviceStopedByUser()
    {
      if (DeviceStopedByUser != null)
        DeviceStopedByUser();
    }
    #endregion

    #region Global Variables
    public bool _isSnapshootActive;
    private BioObserver<ICaptureDeviceObserver> _observer           ;
    private readonly DialogsHolder              _dialogsHolder      ;
    private readonly ICaptureDeviceEngine       _captureDeviceEngine;
    private          ProgressRingViewModel      _progressRing       ;
    #endregion

  }
}
