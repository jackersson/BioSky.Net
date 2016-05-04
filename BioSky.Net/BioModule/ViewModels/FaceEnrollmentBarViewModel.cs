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
    public FaceEnrollmentBarViewModel(IProcessorLocator locator, long style = MAX_FACE_ENROLLMENT_BAR_STYLE)
    {      
      _captureDeviceEngine  = locator.GetProcessor<ICaptureDeviceEngine>();
      _dialogsHolder        = locator.GetProcessor<DialogsHolder>();
      _observer             = new BioObserver<ICaptureDeviceObserver>();
      Resolution            = new AsyncObservableCollection<string>();
      _notifier             = locator.GetProcessor<INotifier>();

      SetStyle(style);
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
      foreach (KeyValuePair<int, ICaptureDeviceObserver> observer in _observer.Observers)
        _captureDeviceEngine.Subscribe(observer.Value, SelectedDevice);
    }

    private void StopCaptureDevice()
    {
      Resolution.Clear();
      _captureDeviceEngine.Unsubscribe(this);
      _captureDeviceEngine.Remove(SelectedDevice);
      foreach (KeyValuePair<int, ICaptureDeviceObserver> observer in _observer.Observers)
        _captureDeviceEngine.Unsubscribe(observer.Value);
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
        _notifier.ShowInformation(ex.Message);
      NotifyOfPropertyChange(() => DeviceConnectedIcon);
    }    

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all)
    {
      NotifyOfPropertyChange(() => DeviceConnectedIcon);
      UpdateResolution(active, all);
      _notifier.Hide(ON_START_TIMER);
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
      _notifier.ShowInformation(message);
    }

    public void OnSnapshoot()
    {
      if (SelectedDevice == null)
        return;
      _isSnapshootActive = true;
    }

    public void OnStopDevice()
    {
      SelectedDevice = null;
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

    #region style
    private long SetFlag(long currentStyle, FaceEnrollmentBarStyle style)
    {
      return currentStyle | (long)style;
    }
    private bool HasFlag(long currentStyle, FaceEnrollmentBarStyle style)
    {
      long activityL = (long)style;
      return (currentStyle & activityL) == activityL;
    }
    public void SetStyle(long style) { ControlStyle = style; }

    public void SetStyle(bool isNewUser) { ControlStyle = (isNewUser) ? NEW_USER_FACE_ENROLLMENT_BAR_STYLE : MAX_FACE_ENROLLMENT_BAR_STYLE; }

    private long _controlStyle;
    public long ControlStyle
    {
      get { return _controlStyle; }
      set
      {
        if (_controlStyle != value)
        {
          _controlStyle = value;
          NotifyOfPropertyChange(() => ControlStyle);
          NotifyOfPropertyChange(() => EnrollButtonVisibility);
        }
      }
    }
    public bool EnrollButtonVisibility
    {
      get { return HasFlag(ControlStyle, FaceEnrollmentBarStyle.EnrollButton); }
    }

    #endregion

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

    public const long NEW_USER_FACE_ENROLLMENT_BAR_STYLE = 0;
    public const long MAX_FACE_ENROLLMENT_BAR_STYLE = (long)(FaceEnrollmentBarStyle.EnrollButton);

    public const int ON_START_TIMER = 2000;

    public bool _isSnapshootActive;
    private BioObserver<ICaptureDeviceObserver> _observer           ;
    private readonly DialogsHolder              _dialogsHolder      ;
    private readonly ICaptureDeviceEngine       _captureDeviceEngine;
    private readonly INotifier                  _notifier           ;
    #endregion

  }

  public enum FaceEnrollmentBarStyle
  {
    EnrollButton = 1 << 0
  }
}
