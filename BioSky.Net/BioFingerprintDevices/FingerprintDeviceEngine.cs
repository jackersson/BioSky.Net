using BioContracts;
using BioContracts.Common;
using BioContracts.FingerprintDevices;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BioFingerprintDevices
{
  public class FingerprintDeviceEngine : IFingerprintDeviceEngine
  {
    public FingerprintDeviceEngine()
    {
      _devices = new ConcurrentDictionary<string, FingerprintDeviceListener>();

      _deviceEnumerator = new FingerprintDeviceEnumerator();
      _deviceEnumerator.Start();
    }

    public void Add(string deviceName)
    {
      if (deviceName == null)
        return;

      FingerprintDeviceListener listener;
      if (!_devices.TryGetValue(deviceName, out listener))
      {
        listener = new FingerprintDeviceListener(deviceName, _deviceEnumerator);
        listener.Start();
        _devices.TryAdd(deviceName, listener);
      }
    }

    public AsyncObservableCollection<FingerprintDeviceInfo> GetDevicesNames()
    {
      return _deviceEnumerator.DevicesNames;
    }

    public bool HasObserver(IFingerprintDeviceObserver observer, string deviceName)
    {
      if (deviceName == null)
        return false;

      FingerprintDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))
        return listener.HasObserver(observer);

      return false;
    }

    public bool IsDeviceActive(string deviceName)
    {
      if (deviceName == null)
        return false;

      FingerprintDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))
        return listener.IsActive();
      return false;
    }

    public void Remove(string deviceName)
    {
      if (deviceName == null)
        return;

      FingerprintDeviceListener listener;
      _devices.TryRemove(deviceName, out listener);
      if (listener != null)
        listener.Kill();
    }

    public void RemoveAll()
    {
      _deviceEnumerator.Stop();

      foreach (KeyValuePair<string, FingerprintDeviceListener> par in _devices)
        par.Value.Kill();

      _devices.Clear();
    }

    public void Subscribe(IFingerprintDeviceObserver observer, string deviceName)
    {
      if (deviceName == null)
        return;

      observer.OnMessage(DevicesInfo.Instance.GetErrorMessage(DevicesInfoEnum.CONNECTING_TO_DEVICE));

      FingerprintDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))
        listener.Subscribe(observer);
    }

    public void Unsubscribe(IFingerprintDeviceObserver observer)
    {
      if (observer == null)
        return;

      foreach (KeyValuePair<string, FingerprintDeviceListener> par in _devices)
      {
        FingerprintDeviceListener listener = par.Value;
        if (listener.HasObserver(observer))
          listener.Unsubscribe(observer);
      }
    }

    public void UpdateFromSet(ICollection<string> devices)
    {
      if (devices == null || devices.Count <= 0)
      {
        RemoveAll();
        return;
      }

      IEnumerable<string> devicesToAdd    =  devices.Where(x => !ContainsKey(x));
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
      FingerprintDeviceListener result;
      return _devices.TryGetValue(key, out result);
    }

    private readonly FingerprintDeviceEnumerator          _deviceEnumerator;
    private ConcurrentDictionary<string, FingerprintDeviceListener> _devices         ;
  }
}
