using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;

using Microsoft.Win32;
using System.IO;
using System.Drawing;
using MahApps.Metro.Controls;
using BioData;
using BioService;
using System.Windows.Threading;
using BioModule.Utils;
using BioContracts;
using System.Windows;
using BioContracts.Common;
using Grpc.Core;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Drawing.Drawing2D;
using Accord.Imaging.Filters;
using AForge.Imaging;

namespace BioModule.ViewModels
{
  public class PhotoImageViewModel : ImageViewModel
  {
    public PhotoImageViewModel(IProcessorLocator locator)
      : base(locator)
    {
      _locator       = locator;


      _serviceManager      = locator.GetProcessor<IServiceManager>();
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      _notifier            = _locator.GetProcessor<INotifier>();


      PhotoInfoExpanderView = new PhotoInfoExpanderViewModel();
      _enroller             = new Enroller(locator);
      SetBitmapSource       = new TemporayBitmapSourceHolder();

      SetBitmapSource.BasePhoto = CurrentImageSource;

      PhotoInfoExpanderView.ExpanderChanged += Expander_Changed;
    }

    #region Interface

    public void EnrollFromPhoto()
    {
      if (!_enroller.Busy)
      {
        Photo photo = UploadPhotoFromFile();

        if (photo == null || photo.Description.Length <= 0)
        {
          MessageBox.Show("Upload New photo");
          return;
        }

        _serviceManager.FaceService.EnrollFeedbackChanged += FaceService_EnrollFeedbackChanged;
        //EnrollmentData data = new EnrollmentData();
        //_enroller.Start(photo, data);
      }
      else
        MessageBox.Show("Wait for finnishing previous operation");
    }

    public void EnrollFromCamera()
    {
      var result = false; // _windowManager.ShowDialog(new CameraDialogViewModel(_locator, this));
      
      if(result == true)
      {
        if (captureDeviceConnected && !_enroller.Busy)
        {
          _serviceManager.FaceService.EnrollFeedbackChanged += FaceService_EnrollFeedbackChanged;
          EnrollmentData data = new EnrollmentData();
          //_enroller.Start(selectedCaptureDevice);
        }

        if (!captureDeviceConnected)
          MessageBox.Show("Choose CaptureDevice first ! ");

        if (_enroller.Busy)
          MessageBox.Show("Wait for finnishing previous operation");
      }
    }

    public void Expander_Changed(bool isExpanded)
    {
      if (isExpanded)
        DrawingOnImage();
      else
        CurrentImageSource = SetBitmapSource.BasePhoto;     
    }


    public void DrawingOnImage()
    {
      Rectangle rect = new Rectangle(100, 50, 500, 500);
      Rectangle rect2 = new Rectangle(200, 100, 500, 500);
      Rectangle rect3 = new Rectangle(700, 50, 500, 500);

      List<Rectangle> rectList = new List<Rectangle>();
      rectList.Add(rect);
      rectList.Add(rect2);
      rectList.Add(rect3);

      AForge.IntPoint p = new AForge.IntPoint(100,100);
      AForge.IntPoint p2 = new AForge.IntPoint(300, 100);
      AForge.IntPoint p3 = new AForge.IntPoint(500, 100);
      AForge.IntPoint p4 = new AForge.IntPoint(100, 500);
      AForge.IntPoint p5 = new AForge.IntPoint(100, 300);


      List<AForge.IntPoint> pointList = new List<AForge.IntPoint>();
      pointList.Add(p);
      pointList.Add(p2);
      pointList.Add(p3);
      pointList.Add(p4);
      pointList.Add(p5);

      // Create a rectangles marker to draw some rectangles around the faces



      //     if (CurrentImagePhoto == null)
      //       return;

      //     if(_isPhotoChanged)
      //      {


      RectanglesMarker marker = new RectanglesMarker(rectList, System.Drawing.Color.Fuchsia);
        PointsMarker marker2 = new PointsMarker(pointList, System.Drawing.Color.Red, 10);

        Bitmap bm = BitmapFromSource(CurrentImageSource);
        Bitmap rectMarker = marker.Apply(bm);
        Bitmap pointMarker = marker2.Apply(rectMarker);

        SetBitmapSource.PhotoWithRectangles = BitmapConversion.BitmapToBitmapSource(pointMarker);

        _isPhotoChanged = false;
      //     }

      CurrentImageSource = SetBitmapSource.PhotoWithRectangles;
      
    }

    public Bitmap BitmapFromSource(BitmapSource bitmapsource)
    {
      //convert image format
      var src = new FormatConvertedBitmap();
      src.BeginInit();
      src.Source = bitmapsource;
      src.DestinationFormat = PixelFormats.Bgr24;
      src.EndInit();

      //copy to bitmap
      Bitmap bitmap = new Bitmap(src.PixelWidth, src.PixelHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      var data = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      src.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
      bitmap.UnlockBits(data);

      return bitmap;
    }

    #endregion

    #region BioService
    public void Subscribe()
    {
      captureDeviceConnected = false;
      if (selectedCaptureDevice == null)
        return;

      if (activeCaptureDevice != null)
        _captureDeviceEngine.Unsubscribe(OnNewFrame, activeCaptureDevice);

      activeCaptureDevice = selectedCaptureDevice;

      if (!_captureDeviceEngine.CaptureDeviceActive(activeCaptureDevice))
        _captureDeviceEngine.Add(activeCaptureDevice);

      _captureDeviceEngine.Subscribe(OnNewFrame, activeCaptureDevice);
    }

    protected override void OnDeactivate(bool close)
    {
      captureDeviceConnected = false;

      if (activeCaptureDevice != null)
        _captureDeviceEngine.Unsubscribe(OnNewFrame, activeCaptureDevice);

      selectedCaptureDevice = null;

      base.OnDeactivate(close);
    }

    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      if (bitmap == null)
        return;

      captureDeviceConnected = true;

      UpdateImage(ref bitmap);
    }

    private void FaceService_EnrollFeedbackChanged(object sender, EnrollmentFeedback feedback)
    {
      if (feedback.Progress == 100)
      {
        _serviceManager.FaceService.EnrollFeedbackChanged -= FaceService_EnrollFeedbackChanged;

        Photo NewPhoto = new Photo();
        NewPhoto = _enroller.GetCapturedPhoto();
        Photo feedbackPhoto = feedback.Photo;


        if (NewPhoto != null && feedbackPhoto != null)
        {
          feedbackPhoto.EntityState = EntityState.Added     ;
          feedbackPhoto.Description = NewPhoto.Description  ;
          feedbackPhoto.SizeType    = PhotoSizeType.Full    ;
          feedbackPhoto.OriginType  = PhotoOriginType.Loaded;

          OnFeedbackPhotoReceive(feedbackPhoto);

        }

      }
      //if (_imageViewer != null)
      //  _imageViewer.ShowProgress(feedback.Progress, feedback.Success);      
    }





    public void SetCaptureDevice(string captureDevice, bool isConnected)
    {
      selectedCaptureDevice = captureDevice;
      captureDeviceConnected = isConnected;

      Subscribe();
    }

    #endregion

    #region UI

    private TemporayBitmapSourceHolder _setBitmapSource;
    public TemporayBitmapSourceHolder SetBitmapSource
    {
      get { return _setBitmapSource; }
      set
      {
        if (_setBitmapSource != value)
        {
          _setBitmapSource = value;
          NotifyOfPropertyChange(() => SetBitmapSource);
        }
      }
    }

    private PhotoInfoExpanderViewModel _photoInfoExpanderView;
    public PhotoInfoExpanderViewModel PhotoInfoExpanderView
    {
      get { return _photoInfoExpanderView; }
      set
      {
        if (_photoInfoExpanderView != value)
        {
          _photoInfoExpanderView = value;
          NotifyOfPropertyChange(() => PhotoInfoExpanderView);
        }
      }
    }

    public delegate void OnFeedbackPhotoReceiveHandler(Photo feedbackPhoto);
    public event OnFeedbackPhotoReceiveHandler FeedbackPhotoReceive;

    public void OnFeedbackPhotoReceive(Photo feedbackPhoto)
    {
      if (FeedbackPhotoReceive != null)
        FeedbackPhotoReceive(feedbackPhoto);
    }

    #endregion

    #region Global Variables


    string activeCaptureDevice   ;
    string selectedCaptureDevice ;
    bool   captureDeviceConnected;

    private readonly Enroller                 _enroller           ;
    private readonly IProcessorLocator        _locator            ;
    private readonly IServiceManager          _serviceManager     ;
    private readonly ICaptureDeviceEngine     _captureDeviceEngine;
    private readonly INotifier                _notifier           ;


    #endregion

  }

  public class TemporayBitmapSourceHolder
  {
    public BitmapSource BasePhoto;

    public BitmapSource PhotoWithRectangles;
  }
}
