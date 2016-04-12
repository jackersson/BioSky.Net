using BioContracts;
using BioContracts.FingerprintDevices;
using System.Collections.Generic;

namespace BioFingerprintDevices
{
  public class FingerprintDeviceEngine : IFingerprintDeviceEngine
  {
    public FingerprintDeviceEngine()
    {
      _devices = new Dictionary<string, FingerprintDeviceListener>();

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
        _devices.Add(deviceName, listener);
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
      if (_devices.TryGetValue(deviceName, out listener))
      {
        listener.Kill();
        _devices.Remove(deviceName);
      }
    }

    public void Stop()
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

    private readonly FingerprintDeviceEnumerator          _deviceEnumerator;
    private Dictionary<string, FingerprintDeviceListener> _devices         ;
  }
}
