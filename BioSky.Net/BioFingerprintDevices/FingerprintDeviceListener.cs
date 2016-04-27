using BioContracts;
using BioContracts.Abstract;
using BioContracts.Common;
using BioContracts.FingerprintDevices;
using BioFingerprintDevices.Common;
using ScanAPIHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

namespace BioFingerprintDevices
{
  class FingerprintDeviceListener : Threadable, IBioObservable<IFingerprintDeviceObserver>
  {     
    public FingerprintDeviceListener(string deviceName, IDeviceConnectivity<FingerprintDeviceInfo> deviceConnectivity)
    {
      DeviceName         = deviceName;
      _deviceConnectivity = deviceConnectivity;

      _observer = new BioObserver<IFingerprintDeviceObserver>();
    }

    private void Connect(FingerprintDeviceInfo fi)
    {
      if (fi == null)
        return;
      
      if (CurrentDevice != null)
        ReleaseDevice();

      try
      {
        Device.BaseInterface = fi.InterfaceNumber;
        CurrentDevice = new Device();
        CurrentDevice.Open();
        OnReady(true);
      }
      catch (ScanAPIException ex) {
        OnError(ex);
      }
    }

    private void ReleaseDevice()
    {
      if (null == CurrentDevice)
        return;

      try
      {
        CurrentDevice.Dispose();
        CurrentDevice = null;

        Device.BaseInterface = FingerprintDeviceInfo.BaseInterface;
        OnError(new Exception("Stoped"));
      }
      catch (Exception ex) {
        OnError(ex);      
      }
    }

    private int GetFrame(out byte[] frame)
    {
      frame = null;
      if (CurrentDevice == null)
      {
        OnError(FingerprintDeviceErrorInfo.ERROR_DISCONNECTED);
        return FingerprintDeviceErrorInfo.ERROR_DISCONNECTED;
      }

      try {        
        frame = _scanMode == 0 ? CurrentDevice.GetFrame() : CurrentDevice.GetImage(_scanMode);   
      }
      catch (ScanAPIException ex)
      {
        if (frame != null)
          frame = null;

        
        OnError(ex);
        return ex.ErrorCode;
      }

      return FingerprintDeviceErrorInfo.DEVICE_OK;
    }

    protected override void Run()
    {
      Active = true;

      while (Active)
      {
        byte[] frame;
        int error = GetFrame(out frame);
        if (frame != null)
        {          
          GrayScaleBitmap fingerprintBitmap = new GrayScaleBitmap(CurrentDevice.ImageSize.Width, CurrentDevice.ImageSize.Height, frame);
          using (MemoryStream stream = new MemoryStream(fingerprintBitmap.BitmatFileData))
          {
            Bitmap image = new Bitmap(stream);
            OnFrame(ref image);
          }         
        }
        else
        {       
          if (error == FingerprintDeviceErrorInfo.ERROR_DISCONNECTED)
          {
            Connect(_deviceConnectivity.GetDeviceInfo(DeviceName));
            Thread.Sleep(DELAY_BETWEEN_CONNECTION);
          }    
        }
        Thread.Sleep(DELAY_BETWEEN_FRAMES);
      }
    }

    public bool IsActive() { return Active; }

    public void Kill()
    {
      Stop();
      ReleaseDevice();      
    }

    #region observer

    public void OnFrame( ref Bitmap frame)
    {
      foreach (KeyValuePair<int, IFingerprintDeviceObserver> observer in _observer.Observers)    
          observer.Value.OnFrame(ref frame);      
    }

    public void OnError(int error)
    {
      Exception ex = new Exception(FingerprintDeviceErrorInfo.Instance.GetErrorMessage(error));
      OnError(ex);
    }

    public void OnError(ScanAPIException error)
    {
      Exception ex = new Exception(FingerprintDeviceErrorInfo.Instance.GetErrorMessage(error));
      OnError(ex);
    }

    public void OnError(Exception ex)
    {
      foreach (KeyValuePair<int, IFingerprintDeviceObserver> observer in _observer.Observers)
        observer.Value.OnError(ex);
    }

    public void OnMessage(string message)
    {
      foreach (KeyValuePair<int, IFingerprintDeviceObserver> observer in _observer.Observers)
        observer.Value.OnMessage(message);
    }

    public void OnReady(bool isReady)
    {
      foreach (KeyValuePair<int, IFingerprintDeviceObserver> observer in _observer.Observers)
        observer.Value.OnReady(isReady);
    }

    public void UnsubscribeAll() {
      _observer.UnsubscribeAll();
    }

    public bool HasObserver(IFingerprintDeviceObserver observer) {
      return _observer.HasObserver(observer);
    }

    public void Subscribe(IFingerprintDeviceObserver observer) {
      _observer.Subscribe(observer);
    }

    public void Unsubscribe(IFingerprintDeviceObserver observer) {
      _observer.Unsubscribe(observer);
    }
    #endregion

    #region variables
    private BioObserver<IFingerprintDeviceObserver> _observer;

    private readonly IDeviceConnectivity<FingerprintDeviceInfo> _deviceConnectivity;
    private string DeviceName  {  get; set;  }
    private byte   _scanMode = 0;
    private Device CurrentDevice;

    private const int DELAY_BETWEEN_FRAMES     = 10  ;
    private const int DELAY_BETWEEN_CONNECTION = 2000;
    #endregion

  }
}
