using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using Accord.Vision.Tracking;
using AForge.Video.DirectShow;
using System.Threading;


namespace BioEngine.CaptureDevices
{
  public class СaptureDeviceListener
  {

    public СaptureDeviceListener()
    {
      HaarCascade cascade = new FaceHaarCascade();
      detector = new HaarObjectDetector( cascade, 25
                                        , ObjectDetectorSearchMode.Default, 1.2f
                                        , ObjectDetectorScalingMode.GreaterToSmaller);
   
    }


    public void Start(string monikerString)
    {
      _monikerString = monikerString;

      videoSource = new VideoCaptureDevice(_monikerString);
      videoSource.Start();
      videoSource.NewFrame += VideoSource_NewFrame;
    }

    private void Stop()
    {

      // stop current video source
      videoSource.SignalToStop();

      // wait 2 seconds until camera stops
      for (int i = 0; (i < 50) && (videoSource.IsRunning); i++)
      {
        Thread.Sleep(100);
      }
      if (videoSource.IsRunning)
        videoSource.Stop();

     
      tracker              = new Camshift();
      tracker.Mode         = CamshiftMode.RGB;
      tracker.Conservative = true;
      tracker.AspectRatio  = 1.5f;   
    }

    private void videoSourcePlayer_NewFrame(object sender /*, ref Bitmap image*/)
    {
      /*
      if (!detecting && !tracking)
        return;

      lock (this)
      {
        if (detecting)
        {
          detecting = false;
          tracking = false;

          UnmanagedImage im = UnmanagedImage.FromManagedImage(image);

          float xscale = image.Width / 160f;
          float yscale = image.Height / 120f;

          ResizeNearestNeighbor resize = new ResizeNearestNeighbor(160, 120);
          UnmanagedImage downsample = resize.Apply(im);

          Rectangle[] regions = detector.ProcessFrame(downsample);

          if (regions.Length > 0)
          {
            tracker.Reset();

            // Will track the first face found
            Rectangle face = regions[0];

            // Reduce the face size to avoid tracking background
            Rectangle window = new Rectangle(
                (int)((regions[0].X + regions[0].Width / 2f) * xscale),
                (int)((regions[0].Y + regions[0].Height / 2f) * yscale),
                1, 1);

            window.Inflate(
                (int)(0.2f * regions[0].Width * xscale),
                (int)(0.4f * regions[0].Height * yscale));

            // Initialize tracker
            tracker.SearchWindow = window;
            tracker.ProcessFrame(im);

            marker = new RectanglesMarker(window);
            marker.ApplyInPlace(im);

            image = im.ToManagedImage();

            tracking = true;
            //detecting = true;
          }
          else
          {
            detecting = true;
          }
        }
        else if (tracking)
        {
          UnmanagedImage im = UnmanagedImage.FromManagedImage(image);

          // Track the object
          tracker.ProcessFrame(im);

          // Get the object position
          var obj = tracker.TrackingObject;
          var wnd = tracker.SearchWindow;

          if (displayBackprojectionToolStripMenuItem.Checked)
          {
            var backprojection = tracker.GetBackprojection(PixelFormat.Format24bppRgb);
            im = UnmanagedImage.FromManagedImage(backprojection);
          }

          if (drawObjectAxisToolStripMenuItem.Checked)
          {
            LineSegment axis = obj.GetAxis();

            // Draw X axis
            if (axis != null)
              Drawing.Line(im, axis.Start.Round(), axis.End.Round(), Color.Red);
            else detecting = true;
          }


          if (drawObjectBoxToolStripMenuItem.Checked && drawTrackingWindowToolStripMenuItem.Checked)
          {
            marker = new RectanglesMarker(new Rectangle[] { wnd, obj.Rectangle });
          }
          else if (drawObjectBoxToolStripMenuItem.Checked)
          {
            marker = new RectanglesMarker(obj.Rectangle);
          }
          else if (drawTrackingWindowToolStripMenuItem.Checked)
          {
            marker = new RectanglesMarker(wnd);
          }
          else
          {
            marker = null;
          }


          if (marker != null)
            marker.ApplyInPlace(im);
          image = im.ToManagedImage();
        }
        else
        {
          if (marker != null)
            image = marker.Apply(image);
        }

      }
      */
    }


    private void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
    {
     /*
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
      */
     }

    private AForge.Video.IVideoSource videoSource = null;

    HaarObjectDetector detector;

    // object tracker
    Camshift tracker = null;

    // window marker
    RectanglesMarker marker;

    string _monikerString;




  }
}
