using BioContracts;
using BioContracts.Common;
using BioContracts.IrisDevices;
using BioModule.BioModels;
using BioModule.ResourcesLoader;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioModule.ViewModels
{
  public class IrisEnrollmentBarViewModel : Screen, IIrisDeviceObserver, IBioObservable<IIrisDeviceObserver>
  {
    public IrisEnrollmentBarViewModel(IProcessorLocator locator, IEyeSelector selector)
    {
      _deviceEngine = locator.GetProcessor<IIrisDeviceEngine>();
      _observer = new BioObserver<IIrisDeviceObserver>();
      _selector = selector;          
    }

    private void DevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }

    public string AvaliableDevicesCount
    {
      get
      {
        if (DevicesNames == null)
          DevicesNames = _deviceEngine.GetDevicesNames();

        return String.Format("Available Devices ({0})", _devicesNames.Count);
      }
    }

    protected override void OnActivate()
    {
      DevicesNames = _deviceEngine.GetDevicesNames();
      DevicesNames.CollectionChanged += DevicesNames_CollectionChanged;

      StartDevice();

      base.OnActivate();            
    }

    protected override void OnDeactivate(bool close)
    {
      if(DevicesNames != null)
        DevicesNames.CollectionChanged -= DevicesNames_CollectionChanged;

      StopDevice();
      base.OnDeactivate(close);      
    }
    private void StartDevice()
    {
      if (string.IsNullOrEmpty(SelectedDevice))
        return;

      _deviceEngine.Add(SelectedDevice);
      _deviceEngine.Subscribe(this, SelectedDevice);
      _deviceEngine.Capture(SelectedDevice);
    }

    private void StopDevice()
    {
      _deviceEngine.Unsubscribe(this);     
      _deviceEngine.Remove(SelectedDevice);
    }

    public void OnFrame( Bitmap left,  Bitmap right)
    {
      foreach (KeyValuePair<int, IIrisDeviceObserver> observer in _observer.Observers)
        observer.Value.OnFrame( left,  right);
    }

    public void OnIrisQualities(IList<EyeScore> scores)
    {
      //throw new NotImplementedException();
    }

    public void OnEyesDetected(bool detected)
    {
      //throw new NotImplementedException();
    }

    public void OnState(CaptureState captureState)
    {
      //throw new NotImplementedException();
    }
   
    public void OnError(Exception ex)
    {
      NotifyOfPropertyChange(() => DeviceConnectedIcon);
    }

    public void OnReady(bool isReady)
    {
      NotifyOfPropertyChange(() => DeviceConnectedIcon);
    }

    #region observer
    public void Subscribe(IIrisDeviceObserver observer)
    {
      _observer.Subscribe(observer);
    }

    public void Unsubscribe(IIrisDeviceObserver observer)
    {
      _observer.Unsubscribe(observer);
    }

    public void UnsubscribeAll() { _observer.UnsubscribeAll(); }

    public bool HasObserver(IIrisDeviceObserver observer) { return _observer.HasObserver(observer); }

    public void OnMessage(string message) { }
  
    #endregion

    #region UI

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
      get {
        return _deviceEngine.IsDeviceActive(SelectedDevice) ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource;
      }
    }

    private string _selectedDevice;
    public string SelectedDevice
    {
      get { return _selectedDevice; }
      set
      {
        if (_selectedDevice != value)
        {
          StopDevice();
          _selectedDevice = value;
          StartDevice();

          NotifyOfPropertyChange(() => SelectedDevice);
        }
      }
    }
    public EyeType SelectedEye
    {
      get { return _selector != null ? _selector.SelectedEye : EyeType.Both; }
      set
      {
        if (_selector != null && _selector.SelectedEye != value)
          _selector.SelectedEye = value;
      }    
    }

    private List<string> _eyeNames;
    public List<string> EyeNames
    {
      get
      {
        if (_eyeNames == null)
          _eyeNames = Enum.GetNames(typeof(EyeType)).ToList();
        return _eyeNames;
      }
    }
    #endregion

    #region Global Variables   
    private BioObserver<IIrisDeviceObserver> _observer;
    private readonly IIrisDeviceEngine _deviceEngine  ;
    private readonly IEyeSelector      _selector      ;
    #endregion

  }
}
