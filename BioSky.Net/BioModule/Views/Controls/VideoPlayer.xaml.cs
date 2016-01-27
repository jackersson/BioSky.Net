

using Accord.Imaging.Filters;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace BioModule.Views.Controls
{
  public partial class VideoPlayer
  {

    #region Variable declaration


    public static readonly DependencyProperty VideoPreviewWidthProperty
                                             = DependencyProperty.Register("VideoPreviewWidth"
                                             , typeof(double)
                                             , typeof(VideoPlayer)
                                             , new PropertyMetadata(VideoPreviewWidthPropertyChangedCallback));


    public static readonly DependencyProperty VideoPreviewHeightProperty
                                             = DependencyProperty.Register("VideoPreviewHeight"
                                             , typeof(double)
                                             , typeof(VideoPlayer)
                                             , new PropertyMetadata(VideoPreviewHeightPropertyChangedCallback));


    public static readonly DependencyProperty VideoSourceProperty
                                             = DependencyProperty.Register("VideoSource"
                                             , typeof(IVideoSource)
                                             , typeof(VideoPlayer)
                                             , new PropertyMetadata(VideoSourcePropertyChangedCallback));

    #endregion

    #region Constructor
    public VideoPlayer()
    {
      InitializeComponent();

      InitializeComponent();

      Dispatcher.ShutdownStarted += DispatcherShutdownStarted;
    }

    #endregion
    private bool isVideoSourceInitialized;

      #region Properties

    [TypeConverter(typeof(LengthConverter))]
    public double VideoPreviewWidth
    {
      get { return (double)GetValue(VideoPreviewWidthProperty); }
      set
      {
        this.SetValue(VideoPreviewWidthProperty, value);
      }
    }

    [TypeConverter(typeof(LengthConverter))]
    public double VideoPreviewHeight
    {
      get { return (double)GetValue(VideoPreviewHeightProperty); }
      set
      {
        this.SetValue(VideoPreviewHeightProperty, value);
      }
    }

    public IVideoSource VideoSource
    {
      get { return (IVideoSource)GetValue(VideoSourceProperty); }
      set
      {
        this.SetValue(VideoSourceProperty, value);
      }

    }

    #endregion

    #region Methods

    private static void VideoSourcePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
    {
      var oldValue = eventArgs.OldValue as IVideoSource;
      var newValue = eventArgs.NewValue as IVideoSource;


      var videoPlayer = sender as VideoPlayer;

      if (null == videoPlayer)
        return;

      if (null == newValue)
        return;

      videoPlayer.InitializeVideoDevice(newValue);
    }


    private static void VideoPreviewWidthPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
    {
      var videoPlayer = sender as VideoPlayer;
      if (null == videoPlayer)
        return;

      if (null == eventArgs.NewValue)
        return;

      var newValue = (double)eventArgs.NewValue;
      if (double.IsNaN(newValue))
      {
        var parentControl = (videoPlayer.VisualParent as Grid);
        videoPlayer.SetVideoPlayerWidth(null != parentControl ? parentControl.Width : newValue);
      }
      else
        videoPlayer.SetVideoPlayerWidth(newValue);

    }

    private static void VideoPreviewHeightPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
    {
      var webCamDevice = sender as VideoPlayer;
      if (null == webCamDevice)
        return;


      if (null == eventArgs.NewValue)
        return;

      var newValue = (double)eventArgs.NewValue;
      if (double.IsNaN(newValue))
      {
        var parentControl = (webCamDevice.VisualParent as Grid);
        webCamDevice.SetVideoPlayerHeight(null != parentControl ? parentControl.Height : newValue);
      }
      else
        webCamDevice.SetVideoPlayerHeight(newValue);
    }



    private void InitializeVideoDevice(IVideoSource videoDeviceSource)
    {
      if (isVideoSourceInitialized)
        return;

      var errorAction = new Action(() => this.SetVideoPlayer(false, "Unable to set video device source"));

      ReleaseVideoDevice();

      if (videoDeviceSource == null)
        return;

      try
      {
        HaarCascade cascade = new FaceHaarCascade();
        detector = new HaarObjectDetector(cascade, 25
                                          , ObjectDetectorSearchMode.NoOverlap, 1.2f
                                          , ObjectDetectorScalingMode.GreaterToSmaller);


        //VideoSourcePlayer.VideoSource = videoDeviceSource;
        //VideoSourcePlayer.NewFrame += videoSourcePlayer_NewFrame;
       // VideoSourcePlayer.Start();
        isVideoSourceInitialized = true;
        SetVideoPlayer(true);
      }
      catch (ArgumentNullException)
      {
        errorAction();
      }
      catch (ArgumentException)
      {
        errorAction();
      }

    }



    private void SetVideoPlayerWidth(double newWidth)
    {
      this.NoVideoSourceGrid.Width = newWidth;
      //this.VideoSourceWindowsFormsHost.Width = newWidth;
    }

    private void SetVideoPlayerHeight(double newHeight)
    {
      this.NoVideoSourceGrid.Height = newHeight;
      //this.VideoSourceWindowsFormsHost.Height = newHeight;
    }


    private void ReleaseVideoDevice()
    {
      isVideoSourceInitialized = false;
      SetVideoPlayer(false);

      if (null == VideoSource)
        return;



      VideoSource.SignalToStop();
      VideoSource.WaitForStop();
      VideoSource.Stop();
      VideoSource = null;
    }

    private void SetVideoPlayer(bool isVideoSourceFound, string noVideoSourceMessage = "")
    {
      //// If video source found is true show the video source player or else show no video source message.
      if (isVideoSourceFound)
      {
       // this.VideoSourceWindowsFormsHost.Visibility = Visibility.Visible;
        this.NoVideoSourceGrid.Visibility = Visibility.Hidden;
        this.NoVideoSourceMessage.Text = string.Empty;
      }
      else
      {
       // this.VideoSourceWindowsFormsHost.Visibility = Visibility.Hidden;
        this.NoVideoSourceGrid.Visibility = Visibility.Visible;
        this.NoVideoSourceMessage.Text = string.IsNullOrWhiteSpace(noVideoSourceMessage) ? "No video source device found" : noVideoSourceMessage;
      }
    }


    private void DispatcherShutdownStarted(object sender, EventArgs eventArgs)
    {
      this.ReleaseVideoDevice();
    }

    private void WebcamDeviceOnUnloaded(object sender, RoutedEventArgs eventArgs)
    {
      this.ReleaseVideoDevice();
    }


    private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
    {
      lock (this)
      {
        UnmanagedImage im = UnmanagedImage.FromManagedImage(image);

        float xscale = image.Width / 160f;
        float yscale = image.Height / 120f;

        ResizeNearestNeighbor resize = new ResizeNearestNeighbor(160, 120);
        UnmanagedImage downsample = resize.Apply(im);

        Rectangle[] regions = detector.ProcessFrame(downsample);

        if (regions.Length > 0)
        {
          foreach (Rectangle face in regions)
          {

            Rectangle window = new Rectangle(
                (int)((face.X + face.Width / 2.5f) * xscale), (int)((face.Y + face.Height / 2.5f) * yscale),
                1, 1);

            window.Inflate(
                (int)(0.5f * face.Width * xscale),
                (int)(0.7f * face.Height * yscale));

            marker = new RectanglesMarker(window);
            marker.ApplyInPlace(im);

            image = im.ToManagedImage();
          }
        }
      }
    }

    HaarObjectDetector detector;
    RectanglesMarker marker;


    #endregion

  }
}
