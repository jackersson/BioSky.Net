using AForge.Video;
using AForge.Video.DirectShow;
using BioContracts;
using BioContracts.Abstract;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BioEngine.CaptureDevices
{
  //Todo add camera settings

  public class СaptureDeviceListener : Threadable
  {
    private enum CaptureDeviceCommands
    {
        Kill = 0
      , Stop
      , Connect
      , Start
    }   

    public СaptureDeviceListener( string cameraName, ICaptureDeviceConnectivity capDevConnectivity)
    {
      _cameraName         = cameraName;
      _capDevConnectivity = capDevConnectivity;

      _commands = new ConcurrentQueue<CaptureDeviceCommands>();      
      _commands.Enqueue(CaptureDeviceCommands.Connect);    
    }

    private void Connect(FilterInfo fi)
    {    
      if (fi == null)      
        return;

      _videoSource = new VideoCaptureDevice(fi.MonikerString);  
     
      _videoSource.PlayingFinished += _videoSource_PlayingFinished;
      _videoSource.NewFrame        += _videoSource_NewFrame;

      _videoSource.VideoResolution = _videoSource.VideoCapabilities.LastOrDefault();
      _videoSource.Start();
    }

    public void ShowPropertyPage( IntPtr parentWindow)
    {
      if (_videoSource == null)
        return;

      _videoSource.DisplayPropertyPage(parentWindow);      
    }

    public void ShowConfigurationPage(IPropertiesShowable propertiesShowable)
    {
      propertiesShowable.Show(_videoSource);
    }

    private void _videoSource_PlayingFinished(object sender, ReasonToFinishPlaying reason)
    {
      if ( reason != ReasonToFinishPlaying.StoppedByUser)
        _commands.Enqueue(CaptureDeviceCommands.Connect);
    }    

    private void OnNewFrame( Bitmap bmp )
    {
      if (NewFrame != null)
        NewFrame(this, ref bmp);
    }

    private void _videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
    {
      OnNewFrame(eventArgs.Frame);
    } 
    
    public VideoCapabilities[] GetVideoCapabilities()
    {
      return _videoSource.VideoCapabilities;
    }   

    public void SetVideoCapability()
    {

    }

    public override void Run()
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
              ReleaseVideoDevice();
              _videoSource.Start();
              break;

            case CaptureDeviceCommands.Stop:
              ReleaseVideoDevice();
              break;

            case CaptureDeviceCommands.Connect:
              {
                FilterInfo fi = _capDevConnectivity.CaptureDeviceConnected(_cameraName);
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
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    public bool IsActive()
    {
      if (_videoSource == null)
        return false;
      return _videoSource.IsRunning;
    }    

    public void Kill()
    {
      _commands.Enqueue(CaptureDeviceCommands.Kill);     
    }

    public void StopPlay()
    {
      _commands.Enqueue(CaptureDeviceCommands.Stop);
    }

    private VideoCaptureDevice _videoSource;    

    private readonly ICaptureDeviceConnectivity _capDevConnectivity;
    private readonly string _cameraName;
    private ConcurrentQueue<CaptureDeviceCommands> _commands;

    public event FrameEventHandler NewFrame;


  }
}
