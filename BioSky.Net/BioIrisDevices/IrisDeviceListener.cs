using BioContracts;
using BioContracts.Common;
using BioContracts.IrisDevices;
using BioIrisDevices.Utils;
using BioService;
using Iddk2000Demo;
using Iddk2000DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace BioIrisDevices
{
  public class IrisDeviceListener : IBioObservable<IIrisDeviceObserver>
  {    
    public IrisDeviceListener(string deviceName, IDeviceConnectivity<string> deviceConnectivity)
    {
      _deviceName         = deviceName;
      _deviceConnectivity = deviceConnectivity;

      _observer = new BioObserver<IIrisDeviceObserver>();
    }

    private void ClearCapture()
    {
      IddkResult ret = _apis.ClearCapture(IddkEyeSubtype.Unknown);
      if (ret != IddkResult.OK)
        OnError(ret);
    }

    private void CaptureProc(object param, List<IddkImage> images, IddkCaptureStatus captureStatus, IddkResult captureError)
    {
      if (!_timer.IsRunning)
      {
        _timer.Reset();
        _timer.Start();
      }

      if (_captureConfig.StreamMode)
      {
        if (captureError == IddkResult.OK)
        {
          Eyes eyes = _captureConfig.GetEyes(images);
          OnFrame( eyes.LeftEye,  eyes.RightEye);
          Console.WriteLine("Capturing ok");
        }        
      }
      else              
        Iddk2000Utils.Wait(DELAY_CAPTURE_PROCESS);      

      if (captureError == IddkResult.OK)
      {      
        bool eyesDetected = false;
        switch (captureStatus)
        {
          case IddkCaptureStatus.Capturing:
            OnState(CaptureState.Capturing);
            eyesDetected = true;
            break;

          case IddkCaptureStatus.Complete:
            OnState(CaptureState.Complete);
            break;

          case IddkCaptureStatus.Abort:
            OnState(CaptureState.Abort);
            break;

          default:
            {
              if (_timer.ElapsedMilliseconds > EYES_DETECTION_TIMEOUT)
                StopCapture();
              break;
            }
        }
       
        OnEyesDetected(eyesDetected);
      }
      else
      {
        OnError(captureError);
        StopCapture();
      }
    }

    private void StopCapture()
    {
      _timer.Stop();

      IddkResult ret = _apis.StopCapture();
      if (ret != IddkResult.OK)
      {
        OnError(ret);
        Deinit();
        return;
      }

      List<EyeScore> scores = GetIrisQualities();
      OnIrisQualities(scores); 
    }
    
    private List<EyeScore> GetIrisQualities()
    {
      IddkResult ret = IddkResult.OK;
      List<IddkIrisQuality> qualities = new List<IddkIrisQuality>();
      List<EyeScore>        scores    = new List<EyeScore>();

      ret = _apis.GetResultQuality(qualities);
      if (ret == IddkResult.OK)
      {
        if (qualities.Count == 1)
        {
          EyeType type = EyeType.Left;
          if (_captureConfig.EyeSubtype == IddkEyeSubtype.Right)
            type = EyeType.Right;
          else if (_captureConfig.EyeSubtype == IddkEyeSubtype.Unknown)
            type = EyeType.NoneEye;
          
          scores.Add(new EyeScore(type, qualities[0].TotalScore, qualities[0].UsableArea));
        }
        else if (qualities.Count > 1)
        {
          scores.Add(new EyeScore(EyeType.Left , qualities[0].TotalScore, qualities[0].UsableArea));
          scores.Add(new EyeScore(EyeType.Right, qualities[1].TotalScore, qualities[1].UsableArea));
        }
      }
      else if (ret == IddkResult.SE_LeftFrameUnqualified)
      {
        scores.Add(new EyeScore(EyeType.Left, 0, 0));
        scores.Add(new EyeScore(EyeType.Right, qualities[1].TotalScore, qualities[1].UsableArea));      
      }
      else if (ret == IddkResult.SE_RightFrameUnqualified)
      {
        scores.Add(new EyeScore(EyeType.Right, 0, 0));
        scores.Add(new EyeScore(EyeType.Left, qualities[1].TotalScore, qualities[1].UsableArea));        
      }
      else
        OnError(ret);

      return scores;
    }    

    public void Capture()
    {
      ClearCapture();
     
      IddkResult ret = _apis.GetDeviceConfig(_deviceConfig);       
      _captureConfig.StreamMode = (ret == IddkResult.OK) ? _deviceConfig.EnableStream : false;
          
      ret = _apis.InitCamera(_captureConfig.ImageWidth, _captureConfig.ImageHeight);
      if (ret != IddkResult.OK)
      {
        OnError(ret);
        return;
      }
      
      ret = _apis.StartCapture( _captureConfig.CaptureMode, _captureConfig.CaptureModeParam
                              , _captureConfig.QualityMode, _captureConfig.CaptureOperationMode
                              , _captureConfig.EyeSubtype , _captureConfig.AutoLeds
                              , CaptureProc, IntPtr.Zero);
      
      if (ret != IddkResult.OK)
      {
        OnError(ret);
        Deinit();
      }    
    }

    private void Deinit()
    {
      IddkResult ret = _apis.DeinitCamera();
      if (ret != IddkResult.OK)
        OnError(ret);

      ClearCapture();
    }

    private void Connect(string deviceName)
    {
      if (string.IsNullOrEmpty(deviceName ))
        return;

      Console.WriteLine("Connecting...");

      IddkResult ret = _apis.OpenDevice(deviceName);
      if (ret == IddkResult.OK)
      {
        //IddkDeviceInfo deviceInfo = new IddkDeviceInfo();
        //ret = _apis.GetDeviceInfo(deviceInfo);
        OnReady(true);
      }
      else
        OnError(ret);             
    }
    
    public void Start()
    {
      Connect(_deviceName);
    }

    public bool IsActive()
    {
      if (string.IsNullOrEmpty(_deviceName))
        return false;

      return _deviceConnectivity.IsDeviceConnected(_deviceName);
    }
    
    public void Kill()
    {
      Deinit();
      IddkResult ret = _apis.CloseDevice();
      if (ret != IddkResult.OK)
        OnError(ret);
    }
    
    #region observers

    public void OnFrame( Bitmap leftEye,  Bitmap rightEye)
    {
      foreach (KeyValuePair<int, IIrisDeviceObserver> observer in _observer.Observers)
        observer.Value.OnFrame( leftEye,  rightEye);
    }

    public void OnError(IddkResult error)
    {
      Exception ex = new Exception(IrisUtils.Instance.GetErrorMessage(error));
      OnError(ex);
    }

    public void OnError(Exception ex)
    {
      foreach (KeyValuePair<int, IIrisDeviceObserver> observer in _observer.Observers)
        observer.Value.OnError(ex);
    }

    public void OnState(CaptureState state)
    {
      foreach (KeyValuePair<int, IIrisDeviceObserver> observer in _observer.Observers)
        observer.Value.OnState(state);
    }

    public void OnEyesDetected(bool detected)
    {
      foreach (KeyValuePair<int, IIrisDeviceObserver> observer in _observer.Observers)
        observer.Value.OnEyesDetected(detected);
    }

    public void OnIrisQualities(IList<EyeScore> scores)
    {
      foreach (KeyValuePair<int, IIrisDeviceObserver> observer in _observer.Observers)
        observer.Value.OnIrisQualities(scores);
    }

    public void OnReady(bool isReady)
    {
      foreach (KeyValuePair<int, IIrisDeviceObserver> observer in _observer.Observers)
        observer.Value.OnReady(isReady);
    }



    public void Subscribe(IIrisDeviceObserver observer)
    {
      _observer.Subscribe(observer);
    }

    public void Unsubscribe(IIrisDeviceObserver observer)
    {
      _observer.Unsubscribe(observer);
    }

    public void UnsubscribeAll()
    {
      _observer.UnsubscribeAll();
    }
    public bool HasObserver(IIrisDeviceObserver observer)
    {
      return _observer.HasObserver(observer);
    }
    #endregion

    #region global variables
    private Iddk2000APIs      _apis          = new Iddk2000APIs();
    private IddkDeviceConfig  _deviceConfig  = new IddkDeviceConfig ();
    private IddkCaptureConfig _captureConfig = new IddkCaptureConfig();

    private Stopwatch _timer = new Stopwatch();

    private const int EYES_DETECTION_TIMEOUT = 100000;
    private const int DELAY_CAPTURE_PROCESS  = 100;

    private IDeviceConnectivity<string> _deviceConnectivity;

    private string _deviceName;   

    private readonly BioObserver<IIrisDeviceObserver> _observer;
    #endregion

    //public event IrisFrameEventHandler        FrameChanged       ;
    //public event IrisCaptureStateEventHandler CaptureStateChanged;
  }
}
