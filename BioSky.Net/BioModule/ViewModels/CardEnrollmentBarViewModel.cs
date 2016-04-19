using BioContracts;
using BioContracts.AccessDevices;
using BioContracts.Common;
using BioContracts.Locations;
using BioModule.ResourcesLoader;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioModule.ViewModels
{  
  public class CardEnrollmentBarViewModel : Screen, IAccessDeviceObserver
  { 
    public CardEnrollmentBarViewModel( IProcessorLocator locator  )
    {   
      _deviceEngine = locator.GetProcessor<IAccessDeviceEngine>();            
    }

    private void DevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }
      
    protected override void OnActivate()
    {
      DevicesNames = _deviceEngine.GetDevicesNames();

      DevicesNames.CollectionChanged   += DevicesNames_CollectionChanged;     
      base.OnActivate();
    }    

    protected override void OnDeactivate( bool close )
    {      
      DevicesNames.CollectionChanged   -= DevicesNames_CollectionChanged;
      base.OnDeactivate(close);
    }
         
    private void StopPreviousDevice()
    {
      _deviceEngine.Remove(_selectedDevice);
      _deviceEngine.Unsubscribe(this);
    }

    private void StartSelectedDevice()
    {
      _deviceEngine.Add(_selectedDevice);
      _deviceEngine.Subscribe(this, SelectedDevice);
    }

    #region Observer
    public void OnCardDetected(string cardNumber)
    {
      foreach (IAccessDeviceObserver observer in Observers)
        observer.OnCardDetected(cardNumber);
    }

    public void OnError(Exception ex, LocationDevice device)
    {
      NotifyOfPropertyChange(() => DeviceConnectedIcon);
    }

    public void OnReady(bool isReady)
    {
      NotifyOfPropertyChange(() => DeviceConnectedIcon);
    }

    #endregion

    #region Observerable

    public void Subscribe(IAccessDeviceObserver observer)
    {
      if (!HasObserver(observer))
        Observers.Add(observer);
    }

    public void Unsubscribe(IAccessDeviceObserver observer)
    {
      if (HasObserver(observer))
        Observers.Remove(observer);
    }

    public void UnsubscribeAll()
    {
      Observers.RemoveAll(x => true);
    }

    private bool HasObserver(IAccessDeviceObserver observer)
    {
      return Observers.Contains(observer);
    }
    #endregion

    #region UI
    public string AvaliableDevicesCount
    {
      get
      {
        if (_devicesNames == null)
          DevicesNames = _deviceEngine.GetDevicesNames();
        return String.Format("Available Devices ({0})", _devicesNames.Count);
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
      get { return _deviceEngine.IsDeviceActive(SelectedDevice) ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }


    private string _selectedDevice;
    public string SelectedDevice
    {
      get { return _selectedDevice; }
      set
      {
        if (_selectedDevice != value)
        {
          StopPreviousDevice();

          _selectedDevice = value;

          StartSelectedDevice();

          NotifyOfPropertyChange(() => SelectedDevice);
        }
      }
    }

    private List<IAccessDeviceObserver> _observers;
    private List<IAccessDeviceObserver> Observers
    {
      get {
        if (_observers == null)
          _observers = new List<IAccessDeviceObserver>();
        return _observers; 
      }
    }
    #endregion

    private readonly IAccessDeviceEngine   _deviceEngine  ;
  }
}
