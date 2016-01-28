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

    //public delegate void FrameEventHandler(object sender, Bitmap bitmap );
    

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
      _videoSource.NewFrame += _videoSource_NewFrame;
      _videoSource.Start();
    }

    private void OnNewFrame( Bitmap bmp )
    {
      if (NewFrame != null)
        NewFrame(this, bmp);
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
        FilterInfo fi =  _capDevConnectivity.CaptureDeviceConnected(_cameraName);
        if ( fi != null )
        {
          Active = false;
          OnCanConnect(fi);         
        }        
        Thread.Sleep(2000);
      }

      Console.WriteLine("Stop");
    }

    private void ReleaseVideoDevice()
    {     
      if (null == _videoSource)
        return;

      _videoSource.SignalToStop();
      _videoSource.WaitForStop();
      _videoSource.Stop();
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
        
    /*
    private void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
    {
     
      Bitmap newFrame = (Bitmap)eventArgs.Frame.Clone();
      if (newFrame != null)
      {
        IntPtr hBitMap = newFrame.GetHbitmap();
        UpdateImage(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitMap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));

        if (_enroll)
        {
          byte[] bytes = ImageToByte2(newFrame);
          Google.Protobuf.ByteString bs = Google.Protobuf.ByteString.CopyFrom(bytes);
          BioFaceService.BioImage imageRequest = new BioFaceService.BioImage() { Description = bs };
          _processingImages.Images.Add(imageRequest);

          if (_processingImages.Images.Count >= 4)
          {
            _enroll = false;
            await _bioFaceServiceManager.FaceClient.EnrollFace(_processingImages);
          }
        }

      }
      
     }
  */




  }
}
