using BioContracts;
using BioContracts.Common;
using BioContracts.IrisDevices;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BioIrisDevices
{
  public class IrisDeviceEngine : IIrisDeviceEngine
  {
    public IrisDeviceEngine()
    {
      _devices = new ConcurrentDictionary<string, IrisDeviceListener>();

      _deviceEnumerator = new IrisDeviceEnumerator();
      _deviceEnumerator.Start();
    }

    public void Add(string deviceName)
    {
      if (string.IsNullOrEmpty(deviceName))
        return;

      IrisDeviceListener listener;
      if (!_devices.TryGetValue(deviceName, out listener))
      {
        listener = new IrisDeviceListener(deviceName, _deviceEnumerator);
        listener.Start();       
        _devices.TryAdd(deviceName, listener);    
      }
    }

    public void Capture(string deviceName)
    {
      if (string.IsNullOrEmpty(deviceName))
        return;

      IrisDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))      
        listener.Capture();     
    }
         
    public AsyncObservableCollection<string> GetDevicesNames()
    {
      return _deviceEnumerator.DevicesNames;
    }
    
    public bool IsDeviceActive(string deviceName)
    {
      if (deviceName == null)
        return false;

      IrisDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))
        return listener.IsActive();
      return false;
    }

    public void Remove(string deviceName)
    {
      if (deviceName == null)
        return;

      IrisDeviceListener listener;
      _devices.TryRemove(deviceName, out listener);
      if (listener != null)
        listener.Kill();
    }

    public void RemoveAll()
    {
      _deviceEnumerator.Stop();

      foreach (KeyValuePair<string, IrisDeviceListener> par in _devices)
        par.Value.Kill();

      _devices.Clear();
    }

    public void Subscribe(IIrisDeviceObserver observer, string deviceName)
    {
      if (deviceName == null)
        return;

      observer.OnMessage(DevicesInfo.Instance.GetErrorMessage(DevicesInfoEnum.CONNECTING_TO_DEVICE));

      IrisDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))
        listener.Subscribe(observer);
    }

    public void Unsubscribe(IIrisDeviceObserver observer)
    {
      if (observer == null)
        return;

      foreach (KeyValuePair<string, IrisDeviceListener> par in _devices)
      {
        IrisDeviceListener listener = par.Value;
        if (listener.HasObserver(observer))
          listener.Unsubscribe(observer);
      }
    }

    public bool HasObserver(IIrisDeviceObserver observer, string deviceName)
    {
      if (deviceName == null)
        return false;

      IrisDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))
        return listener.HasObserver(observer);

      return false;
    }

    public void UpdateFromSet(ICollection<string> devices)
    {
      if (devices == null || devices.Count <= 0)
      {
        RemoveAll();
        return;
      }

      IEnumerable<string> devicesToAdd = devices.Where(x => !ContainsKey(x));
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

    private bool ContainsKey(string key)
    {
      IrisDeviceListener result;
      return _devices.TryGetValue(key, out result);
    }

    private readonly IrisDeviceEnumerator _deviceEnumerator;
    private ConcurrentDictionary<string, IrisDeviceListener> _devices;

  }
}
