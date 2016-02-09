using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using Accord.Vision.Tracking;
using AForge.Video;
using AForge.Video.DirectShow;
using BioContracts;
using BioContracts.Abstract;
using System;
using System.Drawing;
using System.Threading;


namespace BioEngine.CaptureDevices
{
  public class СaptureDeviceListener : Threadable
  {
    private IVideoSource _videoSource;
    private readonly ICaptureDeviceConnectivity _capDevConnectivity;
    private readonly string _cameraName;
  

    private event EventHandler      CanConnect;
    public  event FrameEventHandler NewFrame;


    public СaptureDeviceListener( string cameraName, ICaptureDeviceConnectivity capDevConnectivity)
    {
      _cameraName = cameraName;

      _capDevConnectivity = capDevConnectivity;   

      CanConnect += Connect;
    }

    private void Connect(object sender, EventArgs e)
    {
      FilterInfo fi = (FilterInfo)sender;

      if (fi == null)      
        return;
        
      _videoSource = new VideoCaptureDevice(fi.MonikerString);
     
      _videoSource.PlayingFinished += _videoSource_PlayingFinished;
      _videoSource.NewFrame += _videoSource_NewFrame;
      _videoSource.Start();
  
    }

    private void _videoSource_PlayingFinished(object sender, ReasonToFinishPlaying reason)
    {
      this.Start();
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

    private void OnCanConnect( FilterInfo fi)
    {
      if (CanConnect != null)
        CanConnect(fi, EventArgs.Empty);
    }

    public override void Run()
    {     
      Active = true;

      ReleaseVideoDevice();

      while (Active)
      {       
        FilterInfo fi = _capDevConnectivity.CaptureDeviceConnected(_cameraName);
        if (fi != null)
        {
          Active = false;         
          OnCanConnect(fi);
        }                    
        Thread.Sleep(2000);
      }
      
    }

    private void ReleaseVideoDevice()
    {     
      if (null == _videoSource)
        return;

      _videoSource.SignalToStop();

      for (int i = 0; (i < 50) && (_videoSource.IsRunning); i++)      
        Thread.Sleep(100);       
      
      _videoSource = null;
    }

    public bool IsActive()
    {
      if (_videoSource == null)
        return false;
      return _videoSource.IsRunning;
    }
    

    public void Kill()
    {
      ReleaseVideoDevice();
      base.Stop();
    }
  }
}
