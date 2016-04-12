using AForge.Video;
using AForge.Video.DirectShow;
using BioContracts;
using BioContracts.Abstract;
using BioContracts.CaptureDevices;
using BioContracts.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BioCaptureDevices
{
  public class СaptureDeviceListener : Threadable, IBioObservable<ICaptureDeviceObserver>
  {
    private enum CaptureDeviceCommands
    {
        Kill = 0
      , Stop
      , Connect
      , Start
    }   

    public СaptureDeviceListener( string cameraName, IDeviceConnectivity<FilterInfo> capDevConnectivity)
    {
      _cameraName         = cameraName;
      _capDevConnectivity = capDevConnectivity;

      _observer = new BioObserver<ICaptureDeviceObserver>();

      _commands = new ConcurrentQueue<CaptureDeviceCommands>();      
      _commands.Enqueue(CaptureDeviceCommands.Connect);    
    }

    public void Kill()
    {
      _commands.Enqueue(CaptureDeviceCommands.Kill);
    }

    public void StopPlay()
    {
      _commands.Enqueue(CaptureDeviceCommands.Stop);
    }
    public void StartPlay()
    {
      _commands.Enqueue(CaptureDeviceCommands.Start);
    }

    public void Subscribe(ICaptureDeviceObserver observer)
    {
      _observer.Subscribe(observer);
    }

    public void Unsubscribe(ICaptureDeviceObserver observer)
    {
      _observer.Unsubscribe(observer);
    }

    public void UnsubscribeAll()
    {
      _observer.UnsubscribeAll();
    }

    public bool HasObserver(ICaptureDeviceObserver observer)
    {
      return _observer.HasObserver(observer);
    }

    public void ApplyProperties(IntPtr parentWindow)
    {
      if (_videoSource == null)
        return;

      _videoSource.DisplayPropertyPage(parentWindow);
    }

    public void ApplyResolution(int selectedResolution)
    {
      if (_videoSource.VideoCapabilities.Length <= selectedResolution || selectedResolution < 0)
        return;

      VideoCapabilities selected = _videoSource.VideoCapabilities[selectedResolution];

      if (selected != _videoSource.VideoResolution)
      {
        StopPlay();
        _videoSource.VideoResolution = _videoSource.VideoCapabilities[selectedResolution];
        StartPlay();
      }
    }
    

    private void Connect(FilterInfo fi)
    {    
      if (fi == null)      
        return;

      _videoSource = new VideoCaptureDevice(fi.MonikerString);  
     
      _videoSource.PlayingFinished += OnVideoSourcePlayingFinished;
      _videoSource.NewFrame        += _videoSource_NewFrame;

      _videoSource.VideoResolution = GetBestResolution();
      _videoSource.Start();

      OnStart(true, _videoSource.VideoResolution, _videoSource.VideoCapabilities);     
    }    

    private VideoCapabilities GetBestResolution()
    {
      VideoCapabilities result = _videoSource.VideoCapabilities.FirstOrDefault();
      if (result == null)
        return result;

      foreach (VideoCapabilities vc in _videoSource.VideoCapabilities)
      {
        if (result.FrameSize.Height * result.FrameSize.Width < vc.FrameSize.Width * vc.FrameSize.Height)
          result = vc;
      }

      return result;
    }

    private void OnVideoSourcePlayingFinished(object sender, ReasonToFinishPlaying reason)
    {
      if ( reason != ReasonToFinishPlaying.StoppedByUser)
        _commands.Enqueue(CaptureDeviceCommands.Connect);

      OnStop(true, reason.ToString());      
    }    
    
    private void _videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
    {
      Bitmap frame = eventArgs.Frame;
      OnFrame(ref frame);
    }

    protected override void Run()
    {     
      Active = true;
      
      while (Active)
      {
        CaptureDeviceCommands command;
        bool result = Dequeue(out command);

        if ( result )
        {
          switch (command)
          {
            case CaptureDeviceCommands.Kill:
              ReleaseVideoDevice();
              base.Stop();
              break;

            case CaptureDeviceCommands.Start:             
              _videoSource.Start();
              break;

            case CaptureDeviceCommands.Stop:
              ReleaseVideoDevice();
              break;

            case CaptureDeviceCommands.Connect:
              {
                FilterInfo fi = _capDevConnectivity.GetDeviceInfo(_cameraName);
                if (fi != null)                
                  Connect(fi);   
                else
                  _commands.Enqueue(CaptureDeviceCommands.Connect);
                break;
              }         

          }
        }                    
        Thread.Sleep(2000);
      }      
    }

    private bool Dequeue(out CaptureDeviceCommands command)
    {
      lock (_commands)
      {
        while (_commands.IsEmpty)       
          Thread.Sleep(500);        

        return _commands.TryDequeue(out command);
      }
    }

    private void ReleaseVideoDevice()
    {     
      if (null == _videoSource)
        return;

      try
      {       
        _videoSource.SignalToStop();
        _videoSource.WaitForStop();      

        if (_videoSource.IsRunning)
          _videoSource.Stop();         
         
      }
      catch (Exception ex) {
        Console.WriteLine(ex.Message);
      }
    }

    public bool IsActive()
    {
      if (_videoSource == null)
        return false;
      return _videoSource.IsRunning;
    }

    private void OnFrame(ref Bitmap frame)
    {
      foreach (KeyValuePair<int, ICaptureDeviceObserver> observer in _observer.Observers)      
        observer.Value.OnFrame(ref frame);
    }

    private void OnStop(bool stopped, string message)
    {
      foreach (KeyValuePair<int, ICaptureDeviceObserver> observer in _observer.Observers)       
        observer.Value.OnStop(true, message);
    }

    private void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all)
    {
      foreach (KeyValuePair<int, ICaptureDeviceObserver> observer in _observer.Observers)
        observer.Value.OnStart(started, active, all);
    }
    

    private BioObserver<ICaptureDeviceObserver> _observer;
    private VideoCaptureDevice _videoSource;    
    private readonly IDeviceConnectivity<FilterInfo> _capDevConnectivity;
    private readonly string _cameraName;
    private ConcurrentQueue<CaptureDeviceCommands> _commands;
    

  }
}
