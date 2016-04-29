using BioContracts;
using BioContracts.BioTasks.Fingers;
using BioContracts.Common;
using BioContracts.FingerprintDevices;
using BioModule.ResourcesLoader;
using BioModule.Utils;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioModule.ViewModels
{
  public class FingerprintEnrollmentBarViewModel : Screen, IFingerprintDeviceObserver, IBioObservable<IFingerprintDeviceObserver>
  {
    public FingerprintEnrollmentBarViewModel(IProcessorLocator locator)
    {
      _notifier                = locator.GetProcessor<INotifier>();
      _fingerprintDeviceEngine = locator.GetProcessor<IFingerprintDeviceEngine>();
      _observer                = new BioObserver<IFingerprintDeviceObserver>();

      _enroller = new FingerprintEnroller(locator);
    }

    public void UpdateSelector(IFingerSelector fingerSelector)
    {  
      _fingerSelector = fingerSelector;           
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
          DevicesNames = _fingerprintDeviceEngine.GetDevicesNames();

        return string.Format("Available Devices ({0})", _devicesNames.Count);
      }
    }

    protected override void OnActivate()
    {
      DevicesNames = _fingerprintDeviceEngine.GetDevicesNames();
      DevicesNames.CollectionChanged += DevicesNames_CollectionChanged;

      if (_fingerSelector != null)
        _fingerSelector.SelectedFingerChanged += OnSelectedFingerChanged;

      StartDevice();

      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      DevicesNames.CollectionChanged -= DevicesNames_CollectionChanged;
      if (_fingerSelector != null)
        _fingerSelector.SelectedFingerChanged -= OnSelectedFingerChanged;

      _notifier.Hide();

      StopDevice();
      base.OnDeactivate(close);
    }

    private void OnSelectedFingerChanged(Finger finger)
    {
      NotifyOfPropertyChange(() => SelectedFinger);
    }

    private void StartDevice()
    {
      if (string.IsNullOrEmpty(DeviceName))
        return;
     
      _fingerprintDeviceEngine.Add(DeviceName);
      _fingerprintDeviceEngine.Subscribe(this, DeviceName);
    }

    private void StopDevice()
    {
      _fingerprintDeviceEngine.Unsubscribe(this);
      _fingerprintDeviceEngine.Remove(DeviceName);
    }


    public void OnFrame(ref Bitmap frame) {

      _notifier.Hide();

     // if (!_enroller.IsActive)
      //  _enroller.Start(DeviceName, null);

      foreach (KeyValuePair<int, IFingerprintDeviceObserver> observer in _observer.Observers)
        observer.Value.OnFrame(ref frame);
    }

    public void OnError(Exception ex)
    {
      _notifier.ShowInformation(ex.Message);

       NotifyOfPropertyChange(() => DeviceConnectedIcon); 
    }

    public void OnReady(bool isReady)
    {
      NotifyOfPropertyChange(() => DeviceConnectedIcon);         
    }
     
    #region observer
    public void Subscribe(IFingerprintDeviceObserver observer)
    {
      _observer.Subscribe(observer);
    }

    public void Unsubscribe(IFingerprintDeviceObserver observer)
    {
      _observer.Unsubscribe(observer);
    }

    public void UnsubscribeAll() {  _observer.UnsubscribeAll(); }

    public bool HasObserver(IFingerprintDeviceObserver observer) { return _observer.HasObserver(observer); }

    public void OnMessage(string message) {}
    #endregion

    #region UI
    public Finger SelectedFinger
    {
      get { return _fingerSelector != null ? _fingerSelector.SelectedFinger : Finger.Any; }
      set
      {
        if (_fingerSelector != null && _fingerSelector.SelectedFinger != value)
          _fingerSelector.SelectedFinger = value;       
      }
    }

    private List<string> _fingers;
    public List<string> Fingers
    {
      get
      {
        if(_fingers == null)
          _fingers = Enum.GetNames(typeof(Finger)).ToList();
        return _fingers;
      }
    }
     

    private AsyncObservableCollection<FingerprintDeviceInfo> _devicesNames;
    public AsyncObservableCollection<FingerprintDeviceInfo> DevicesNames
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

    private string DeviceName { get { return SelectedDevice == null ? string.Empty : SelectedDevice.Name; }  }

    public BitmapSource DeviceConnectedIcon
    {
      get {      
        return _fingerprintDeviceEngine.IsDeviceActive(DeviceName) ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource;
      }
    }

    private FingerprintDeviceInfo _selectedDevice;
    public FingerprintDeviceInfo SelectedDevice
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
    #endregion

    #region Global Variables   
    private BioObserver<IFingerprintDeviceObserver> _observer;
    private readonly IFingerprintDeviceEngine       _fingerprintDeviceEngine;
    private          IFingerSelector                _fingerSelector         ;
    private readonly INotifier                      _notifier               ;

    private readonly FingerprintEnroller             _enroller;
    #endregion

  }
}
