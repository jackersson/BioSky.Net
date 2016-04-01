using System.Collections.Generic;
using BioContracts;


namespace BioAccessDevice
{
  public class AccessDevicesEngine : IAccessDeviceEngine
  {
    public AccessDevicesEngine()
    {
      _devices = new Dictionary<string, AccessDeviceListener>();

      _deviceEnumerator = new AccessDevicesEnumerator();
      _deviceEnumerator.Start();
    }

    public void Stop()
    {
      _deviceEnumerator.Stop();
      foreach ( KeyValuePair<string, AccessDeviceListener> par in _devices)      
        par.Value.Stop();       

      _devices.Clear();
    }

    public void Add(string deviceName)
    {
      if (deviceName == null)
        return;

      AccessDeviceListener listener;
      if (  !_devices.TryGetValue(deviceName, out listener) )
      {
        listener = new AccessDeviceListener(deviceName);
        listener.Start();       
        _devices.Add(deviceName, listener);
      }
    }

    public void Remove(string deviceName)
    {
      if (deviceName == null)
        return;

      AccessDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))
      {
        listener.Stop();
        _devices.Remove(deviceName);
      }    
    }

    public bool IsDeviceActive(string deviceName)
    {
      if (deviceName == null)
        return false;

      AccessDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))
        return listener.IsActive() ;
      return false;
    }
      
    public void Execute(AccessDeviceCommands command, string deviceName)
    {
      if (deviceName == null)
        return;

      AccessDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))         
        listener.Execute(command);      
    }

    
    public void Subscribe(IAccessDeviceObserver observer, string deviceName)
    {
      if (deviceName == null)
        return;

      AccessDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))      
        listener.Subscribe(observer);
      
    }

    public void Unsubscribe(IAccessDeviceObserver observer)
    {
      if (observer == null)
        return;

      foreach (KeyValuePair<string, AccessDeviceListener> par in _devices)
      {
        AccessDeviceListener listener = par.Value;
        if (listener.HasObserver(observer))
          listener.Unsubscribe(observer);
      }      
    }

    public bool HasObserver(IAccessDeviceObserver observer, string deviceName)
    {
      if (deviceName == null)
        return false;

      AccessDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))      
        return listener.HasObserver(observer);
      
      return false;
    }
    
    public AsyncObservableCollection<string> GetDevicesNames()
    {
      return _deviceEnumerator.DevicesNames;
    }

    private readonly AccessDevicesEnumerator _deviceEnumerator;
    private Dictionary<string, AccessDeviceListener> _devices;    
  }
}
