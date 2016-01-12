namespace BioUITest.Controls
{
  /// <summary>
  /// Interaction logic for VideoPlayer.xaml
  /// </summary>
  #region Namespace

  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Drawing;
  using System.Globalization;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;

  using AForge.Video.DirectShow;

  using Drawing = System.Drawing;
  using Point = System.Windows.Point;

  using AForge.Video;

  #endregion

  /// <summary>
  /// Interaction logic for web-cam device XAML.
  /// </summary>
  public partial class VideoPlayer
  {
    #region Variable declaration

    /// <summary>
    /// Dependency property for video display width.
    /// </summary>
    public static readonly DependencyProperty  VideoPreviewWidthProperty
                                             = DependencyProperty.Register("VideoPreviewWidth"
                                             , typeof(double)
                                             , typeof(VideoPlayer)
                                             , new PropertyMetadata(VideoPreviewWidthPropertyChangedCallback));

    /// <summary>
    /// Dependency property for video display height.
    /// </summary>
    public static readonly DependencyProperty  VideoPreviewHeightProperty 
                                             = DependencyProperty.Register("VideoPreviewHeight"
                                             , typeof(double)
                                             , typeof(VideoPlayer)
                                             , new PropertyMetadata(VideoPreviewHeightPropertyChangedCallback));

    /// <summary>
    /// Dependency property for video device source Id.
    /// </summary>
    public static readonly DependencyProperty  VideoSourceProperty 
                                             = DependencyProperty.Register("VideoSource"
                                             , typeof(string)
                                             , typeof(VideoPlayer)
                                             , new PropertyMetadata( string.Empty
                                                                    , VideoSourcePropertyChangedCallback
                                                                    , VideoSourcePropertyCoherceValueChanged));
    
    /// <summary>
    /// Instance of video capture device.
    /// </summary>
    private IVideoSource videoCaptureDevice;

    /// <summary>
    /// The is video source initialized.
    /// </summary>
    private bool isVideoSourceInitialized;

    #endregion

    #region Constructor

    public VideoPlayer()
    {
      InitializeComponent();

      //// Subcribe to dispatcher shutdown event and dispose all used resources gracefully.
      Dispatcher.ShutdownStarted += DispatcherShutdownStarted;      
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets video device source collection current available.
    /// </summary>
    public static FilterInfoCollection GetVideoDevices
    {
      get
      {      
        return new FilterInfoCollection(FilterCategory.VideoInputDevice);
      }
    }

    /// <summary>
    /// Gets or sets video source device display width.
    /// </summary>
    /// <remarks>
    /// Important: Do not write any logic in dependency property stub.
    /// </remarks>
    [TypeConverter(typeof(LengthConverter))]
    public double VideoPreviewWidth
    {
      get
      {
        return (double)GetValue(VideoPreviewWidthProperty);
      }

      set
      {
        this.SetValue(VideoPreviewWidthProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets video source device display height.
    /// </summary>
    /// <remarks>
    /// Important: Do not write any logic in dependency property stub.
    /// </remarks>
    [TypeConverter(typeof(LengthConverter))]
    public double VideoPreviewHeight
    {
      get
      {
        return (double)GetValue(VideoPreviewHeightProperty);
      }

      set
      {
        this.SetValue(VideoPreviewHeightProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets video device source Id / USB ID / Moniker string.
    /// </summary>
    /// <remarks>
    /// Note: If the Id start with "Message:" followed by a message,
    /// the following message will be display instead of the video source device.
    /// Important: Do not write any logic in dependency property stub.
    /// </remarks>
    public string VideoSource
    {
      get
      {
        return (string)GetValue(VideoSourceProperty);
      }

      set
      {
        this.SetValue(VideoSourceProperty, value);
      }
    }   

    #endregion

    #region Methods

    /// <summary>
    /// Call back function for video device source Id property changed event.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="eventArgs">Event argument.</param>
    private static void VideoSourcePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
    {
      var oldValue = eventArgs.OldValue as string;
      var newValue = eventArgs.NewValue as string;
      var videoPlayer = sender as VideoPlayer;
      if (null == videoPlayer)
      {
        return;
      }

      if (null == eventArgs.NewValue)
      {
        return;
      }

      if (string.IsNullOrWhiteSpace(newValue))
      {
        if (!string.IsNullOrWhiteSpace(oldValue))
        {
          videoPlayer.InitializeVideoDevice(oldValue);
        }
      }
      else
      {
        if (string.IsNullOrWhiteSpace(oldValue))
        {
          videoPlayer.InitializeVideoDevice(newValue);
        }
        else
        {
          if (oldValue != newValue)
          {
            videoPlayer.isVideoSourceInitialized = false;
          }

          videoPlayer.InitializeVideoDevice(oldValue.Equals(newValue) ? oldValue : newValue);
        }
      }
    }

    /// <summary>
    /// Event handler for video device Id value changed event.
    /// </summary>
    /// <param name="dependencyObject">Instance of dependency object.</param>
    /// <param name="basevalue">Base value.</param>
    /// <returns>Return base value / NULL or the new Id value of the video device source.</returns>
    private static object VideoSourcePropertyCoherceValueChanged(DependencyObject dependencyObject, object basevalue)
    {
       var baseValueStringFormat = Convert.ToString(basevalue, CultureInfo.InvariantCulture);
       var availableMediaList = GetVideoDevices;      
       if (string.IsNullOrEmpty(baseValueStringFormat) || availableMediaList.Count <= 0)
       {
         return null;
       }

      FilterInfo filteredVideoDevice = null;
      foreach ( FilterInfo fi in availableMediaList)
      {
        if (fi.MonikerString == baseValueStringFormat)
          filteredVideoDevice = fi;
      }
       
      return null != filteredVideoDevice ? filteredVideoDevice.MonikerString : baseValueStringFormat; 
      
    }

    /// <summary>
    /// Call back function for source video device preview width property changed event.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="eventArgs">Event argument.</param>
    private static void VideoPreviewWidthPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
    {
      var videoPlayer = sender as VideoPlayer;
      if (null == videoPlayer)
      {
        return;
      }

      if (null == eventArgs.NewValue)
      {
        return;
      }

      var newValue = (double)eventArgs.NewValue;
      if (double.IsNaN(newValue))
      {
        var parentControl = (videoPlayer.VisualParent as Grid);
        videoPlayer.SetVideoPlayerWidth(null != parentControl ? parentControl.Width : newValue);
      }
      else
      {
        videoPlayer.SetVideoPlayerWidth(newValue);
      }
    }

    /// <summary>
    /// Call back function for source video device preview height property changed event.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="eventArgs">Event argument.</param>
    private static void VideoPreviewHeightPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
    {
      var webCamDevice = sender as VideoPlayer;
      if (null == webCamDevice)
      {
        return;
      }

      if (null == eventArgs.NewValue)
      {
        return;
      }

      var newValue = (double)eventArgs.NewValue;
      if (double.IsNaN(newValue))
      {
        var parentControl = (webCamDevice.VisualParent as Grid);
        webCamDevice.SetVideoPlayerHeight(null != parentControl ? parentControl.Height : newValue);
      }
      else
      {
        webCamDevice.SetVideoPlayerHeight(newValue);
      }
    }


    /// <summary>
    /// Initialize video device.
    /// </summary>
    /// <param name="videoDeviceSourceId">Video device source Id.</param>
    /// <exception cref="InvalidOperationException">Throws invalid operation exception if video device source setup fails.</exception>
    private void InitializeVideoDevice(string videoDeviceSource)
    {
      if (isVideoSourceInitialized)
      {
        return;
      }

      var errorAction = new Action(() => this.SetVideoPlayer(false, "Unable to set video device source"));

      ReleaseVideoDevice();

      if (string.IsNullOrEmpty(videoDeviceSource))
      {
        return;
      }

      if (videoDeviceSource.StartsWith("Message:", StringComparison.OrdinalIgnoreCase))
      {
        var splitString = videoDeviceSource.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
        if (splitString.Length == 2)
        {
          this.SetVideoPlayer(false, splitString[1]);
        }
        else
        {
          this.SetVideoPlayer(false);
        }
      }
      else
      {
        try
        {
          if (!VideoDeviceEnabled(videoDeviceSource))
          {
            return;
          }

          this.videoCaptureDevice = new VideoCaptureDevice(videoDeviceSource);
          this.VideoSourcePlayer.VideoSource = this.videoCaptureDevice;
          this.VideoSourcePlayer.Start();
          this.isVideoSourceInitialized = true;
          this.SetVideoPlayer(true);
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
    }

    private bool VideoDeviceEnabled( string videoDeviceSource)
    {
      foreach (FilterInfo fi in GetVideoDevices)
      {
        if (fi.MonikerString.Equals(videoDeviceSource))
          return true;
      }
      return false;
    }

    /// <summary>
    /// Set video player width.
    /// </summary>
    /// <param name="newWidth">New width value.</param>
    private void SetVideoPlayerWidth(double newWidth)
    {
      this.NoVideoSourceGrid.Width = newWidth;
      this.VideoSourceWindowsFormsHost.Width = newWidth;
    }

    /// <summary>
    /// Set video player height.
    /// </summary>
    /// <param name="newHeight">New height value.</param>
    private void SetVideoPlayerHeight(double newHeight)
    {
      this.NoVideoSourceGrid.Height = newHeight;
      this.VideoSourceWindowsFormsHost.Height = newHeight;
    }

    /// <summary>
    /// Event handler for camera video device on loaded event.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="eventArgs">Event argument.</param>
    private void WebcamDeviceOnLoaded(object sender, RoutedEventArgs eventArgs)
    {
      //// Set controls width / height based on VideoPreviewWidth / VideoPreviewHeight binding properties.
      this.NoVideoSourceGrid.Width = this.VideoPreviewWidth;
      this.VideoSourceWindowsFormsHost.Width = this.VideoPreviewWidth;
      this.NoVideoSourceGrid.Height = this.VideoPreviewHeight;
      this.VideoSourceWindowsFormsHost.Height = this.VideoPreviewHeight;
      this.InitializeVideoDevice(this.VideoSource);
    }

    /// <summary>
    ///  Disconnect video source device.
    /// </summary>
    private void ReleaseVideoDevice()
    {
      this.isVideoSourceInitialized = false;
      this.SetVideoPlayer(false);
      if (null == this.videoCaptureDevice)
      {
        return;
      }

      this.videoCaptureDevice.SignalToStop();
      this.videoCaptureDevice.WaitForStop();
      this.videoCaptureDevice.Stop();
      this.videoCaptureDevice = null;
    }

    /// <summary>
    /// Set video source player visibility.
    /// </summary>
    /// <param name="isVideoSourceFound">Indicates a value weather video source device found or not.</param>
    /// <param name="noVideoSourceMessage">Message to display when no video source found, optional will use empty string.</param>
    private void SetVideoPlayer(bool isVideoSourceFound, string noVideoSourceMessage = "")
    {
      //// If video source found is true show the video source player or else show no video source message.
      if (isVideoSourceFound)
      {
        this.VideoSourceWindowsFormsHost.Visibility = Visibility.Visible;
        this.NoVideoSourceGrid.Visibility = Visibility.Hidden;
        this.NoVideoSourceMessage.Text = string.Empty;
      }
      else
      {
        this.VideoSourceWindowsFormsHost.Visibility = Visibility.Hidden;
        this.NoVideoSourceGrid.Visibility = Visibility.Visible;
        this.NoVideoSourceMessage.Text = string.IsNullOrWhiteSpace(noVideoSourceMessage) ? "No video source device found" : noVideoSourceMessage;
      }
    }

    /// <summary>
    /// Event handler for dispatcher shutdown started event.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="eventArgs">Event arguments.</param>
    private void DispatcherShutdownStarted(object sender, EventArgs eventArgs)
    {
      this.ReleaseVideoDevice();
    }

    /// <summary>
    /// Event handler for camera video device on unloaded event.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="eventArgs">Event arguments.</param>
    private void WebcamDeviceOnUnloaded(object sender, RoutedEventArgs eventArgs)
    {
      this.ReleaseVideoDevice();
    }

    #endregion
  }
}
