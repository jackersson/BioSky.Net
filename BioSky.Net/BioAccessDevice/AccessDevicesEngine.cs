using System.Collections.Generic;
using BioContracts;
using BioContracts.AccessDevices;
using System.Linq;
using System.Collections.Concurrent;

namespace BioAccessDevice
{
  public class AccessDevicesEngine : IAccessDeviceEngine
  {
    public AccessDevicesEngine()
    {
      _devices = new ConcurrentDictionary<string, AccessDeviceListener>();

      _deviceEnumerator = new AccessDevicesEnumerator();
      _deviceEnumerator.Start();
    }

    public void RemoveAll()
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
        _devices.TryAdd(deviceName, listener);
      }
    }

    public void Remove(string deviceName)
    {
      if (deviceName == null)
        return;

      AccessDeviceListener listener;
      _devices.TryRemove(deviceName, out listener);
      if (listener != null)
        listener.Stop();
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
    
    public AsyncObservableCollection<string> GetDevicesNames() {
      return _deviceEnumerator.DevicesNames;
    }

    public void UpdateFromSet(ICollection<string> devices)
    {
      if (devices == null || devices.Count <= 0)
      {
        RemoveAll();
        return;
      }

      IEnumerable<string> devicesToAdd    = devices.Where(x => !ContainsKey(x));
      IEnumerable<string> devicesToRemove = _devices.Keys.Where(x => !devices.Contains(x));

      if (devicesToAdd != null)
      {
        foreach (string deviceName in devicesToAdd)
        {
          if (!string.IsNullOrEmpty(deviceName))
            Add(deviceName);
        }
      }

      if (devicesToRemove != null)
      {
        foreach (string deviceName in devicesToRemove)
        {
          if (!string.IsNullOrEmpty(deviceName))
            Remove(deviceName);
        }
      }
    }

    private bool ContainsKey( string key)
    {
      AccessDeviceListener result;
      return _devices.TryGetValue(key, out result);
    }

    private readonly AccessDevicesEnumerator _deviceEnumerator;
    private ConcurrentDictionary<string, AccessDeviceListener> _devices;

  }
}
