using BioContracts;
using System.Collections.Generic;
using BioContracts.Common;
using BioContracts.CaptureDevices;
using System.Collections;
using System.Linq;
using System.Collections.Concurrent;

namespace BioCaptureDevices
{
  public class CaptureDeviceEngine : ICaptureDeviceEngine
  {
    public CaptureDeviceEngine()
    {
      _devices = new ConcurrentDictionary<string, СaptureDeviceListener>(); 
        
      _deviceEnumerator = new CaptureDeviceEnumerator();
      _deviceEnumerator.Start();
    }

    public void Add(string cameraName)
    {
      if (cameraName == null)
        return;

      СaptureDeviceListener listener;
      if (!_devices.TryGetValue(cameraName, out listener))
      {      
        listener = new СaptureDeviceListener(cameraName, _deviceEnumerator);
        listener.Start();
        _devices.TryAdd(cameraName, listener);
      }
    }    

    public void Remove(string cameraName)
    {
      if (cameraName == null)
        return;

      СaptureDeviceListener listener;
      _devices.TryRemove(cameraName, out listener);
      if (listener != null)
        listener.Kill();      
    }

    public bool IsDeviceActive(string cameraName)
    {
      if (cameraName == null)
        return false;

      СaptureDeviceListener listener;
      if (_devices.TryGetValue(cameraName, out listener))
        return listener.IsActive();
      return false;
    }

    
    public void ApplyProperties( string cameraName, System.IntPtr parentWindow)
    {
      if (cameraName == null)
        return;

      СaptureDeviceListener listener;
      if (_devices.TryGetValue(cameraName, out listener))      
        listener.ApplyProperties(parentWindow);      
    }

    public void ApplyResolution(string cameraName, int resolutionIndex)
    {
      if (cameraName == null)
        return;

      СaptureDeviceListener listener;
      if (_devices.TryGetValue(cameraName, out listener))
      {
        if (listener != null)
          listener.ApplyResolution(resolutionIndex);
      }
    }

    public AsyncObservableCollection<string> GetDevicesNames()
    {
      return _deviceEnumerator.CaptureDevicesNames;
    }

    public void Subscribe( ICaptureDeviceObserver observer, string cameraName)
    {
      if (observer == null || cameraName == null)
        return;

      observer.OnMessage(""/*DevicesInfo.Instance.GetErrorMessage(DevicesInfoEnum.CONNECTING_TO_DEVICE)*/);

      СaptureDeviceListener listener;
      if (_devices.TryGetValue(cameraName, out listener))      
        listener.Subscribe(observer);      
    }

    public void Unsubscribe(ICaptureDeviceObserver observer)
    {
      if (observer == null)
        return;

      foreach (KeyValuePair<string, СaptureDeviceListener> par in _devices)
      {
        СaptureDeviceListener listener = par.Value;
        if (listener.HasObserver(observer))
          listener.Unsubscribe(observer);        
      }
    }  
    public bool HasObserver(ICaptureDeviceObserver observer, string deviceName)
    {
      if (deviceName == null)
        return false;

      СaptureDeviceListener listener;
      if (_devices.TryGetValue(deviceName, out listener))
        return listener.HasObserver(observer);

      return false;
    }


    public void RemoveAll()
    {
      _deviceEnumerator.Stop();
   
      foreach (KeyValuePair<string, СaptureDeviceListener> par in _devices)
        par.Value.Kill();

      _devices.Clear();    
    }

    public void UpdateFromSet(ICollection<string> devices)
    {
      if (devices == null || devices.Count <= 0)
      {
        RemoveAll();
        return;
      }

      IEnumerable<string> devicesToAdd    = devices.Where      (x => !ContainsKey(x));
      IEnumerable<string> devicesToRemove = _devices.Keys.Where(x => !devices.Contains(x)   );

      if (devicesToAdd != null)
      {
        foreach (string deviceName in devicesToAdd)
        {
          if(!string.IsNullOrEmpty(deviceName))
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
      СaptureDeviceListener result;
      return _devices.TryGetValue(key, out result);
    }

    private readonly CaptureDeviceEnumerator _deviceEnumerator;
    private ConcurrentDictionary<string, СaptureDeviceListener> _devices;
    
  
  }
}
