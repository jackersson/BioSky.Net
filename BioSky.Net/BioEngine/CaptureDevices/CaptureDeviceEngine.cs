using BioContracts;
using System;
using System.Collections.Generic;

using AForge.Video.DirectShow;

namespace BioEngine.CaptureDevices
{
  public class CaptureDeviceEngine : ICaptureDeviceEngine
  {
    public CaptureDeviceEngine()
    {
      _captureDevices = new Dictionary<string, СaptureDeviceListener>();
        
      _captureDeviceEnumerator = new CaptureDeviceEnumerator();
      _captureDeviceEnumerator.Start();
    }

    public void Add(string cameraName)
    {
      if (cameraName == null)
        return;

      СaptureDeviceListener listener;
      if (!_captureDevices.TryGetValue(cameraName, out listener))
      {      
        listener = new СaptureDeviceListener(cameraName, _captureDeviceEnumerator);
        listener.Start();
        _captureDevices.Add(cameraName, listener);
        OnListenerStart();
      }
    }

    public event ListenerStartEventHandler ListenerStart;

    private void OnListenerStart()
    {
      if (ListenerStart != null)
        ListenerStart();
    }

    public void Remove(string cameraName)
    {
      СaptureDeviceListener listener;
      if (_captureDevices.TryGetValue(cameraName, out listener))
      {       
        listener.Kill();       
        _captureDevices.Remove(cameraName);
      }
    }

    public bool CaptureDeviceActive(string cameraName)
    {
      if (cameraName == null)
        return false;

      СaptureDeviceListener listener;
      if (_captureDevices.TryGetValue(cameraName, out listener))
        return listener.IsActive();
      return false;
    }

    public void ShowCaptureDevicePropertyPage( string cameraName, IntPtr parentWindow)
    {
      if (cameraName == null)
        return;

      СaptureDeviceListener listener;
      if (_captureDevices.TryGetValue(cameraName, out listener))      
        listener.ShowPropertyPage(parentWindow);      
    }

    public VideoCapabilities[] GetCaptureDeviceVideoCapabilities(string cameraName)
    {
      if (cameraName == null)
        return null;

      СaptureDeviceListener listener;
      if (_captureDevices.TryGetValue(cameraName, out listener))
        return listener.GetVideoCapabilities();

      return null;
    }

    public void SetCaptureDeviceVideoCapabilities(string cameraName, int selectedResolution)
    {
      if (cameraName == null)
        return;

      СaptureDeviceListener listener;
      if (_captureDevices.TryGetValue(cameraName, out listener))
      {
        if (listener != null)
          listener.SetVideoCapability(selectedResolution);
      }
    }

    public VideoCapabilities GetVideoResolution(string cameraName)
    {
      if (cameraName == null)
        return null;

      СaptureDeviceListener listener;
      if (_captureDevices.TryGetValue(cameraName, out listener))
      {
        if (listener != null)
          return listener.GetVideoResolution();
      }
      return null;
    }

    public AsyncObservableCollection<string> GetCaptureDevicesNames()
    {
      return _captureDeviceEnumerator.CaptureDevicesNames;
    }

    public void Subscribe( FrameEventHandler eventListener, string cameraName)
    {
      if (cameraName == null)
        return;

      СaptureDeviceListener listener;
      if (_captureDevices.TryGetValue(cameraName, out listener))      
        listener.NewFrame += eventListener;      
    }

    public void Unsubscribe(FrameEventHandler eventListener, string cameraName)
    {
      if (cameraName == null)
        return;

      СaptureDeviceListener listener;
      if (_captureDevices.TryGetValue(cameraName, out listener))
        listener.NewFrame -= eventListener;
    }


    public void Stop()
    {
      _captureDeviceEnumerator.Stop();
   
      foreach (KeyValuePair<string, СaptureDeviceListener> par in _captureDevices)
        par.Value.Kill();

      _captureDevices.Clear();
    }

    private readonly CaptureDeviceEnumerator _captureDeviceEnumerator;
    private Dictionary<string, СaptureDeviceListener> _captureDevices;
  
  }
}
