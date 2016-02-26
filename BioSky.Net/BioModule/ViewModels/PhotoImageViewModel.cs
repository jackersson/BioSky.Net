﻿using System;
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


namespace BioModule.ViewModels
{
  public class PhotoImageViewModel : ImageViewModel
  {
    public PhotoImageViewModel(IProcessorLocator locator, IWindowManager windowManager)
      : base(locator, windowManager)
    {
      _locator       = locator      ;
      _windowManager = windowManager;


      _serviceManager      = locator.GetProcessor<IServiceManager>();
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();

      _enroller = new Enroller(_captureDeviceEngine, _serviceManager);
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
        EnrollmentData data = new EnrollmentData();
        _enroller.Start(photo, data);
      }
      else
        MessageBox.Show("Wait for finnishing previous operation");
    }

    public void EnrollFromCamera()
    {
      var result = _windowManager.ShowDialog(new CameraDialogViewModel(_locator, this));
      
      if(result == true)
      {
        if (captureDeviceConnected && !_enroller.Busy)
        {
          _serviceManager.FaceService.EnrollFeedbackChanged += FaceService_EnrollFeedbackChanged;
          EnrollmentData data = new EnrollmentData();
          _enroller.Start(selectedCaptureDevice, data);
        }

        if (!captureDeviceConnected)
          MessageBox.Show("Choose CaptureDevice first ! ");

        if (_enroller.Busy)
          MessageBox.Show("Wait for finnishing previous operation");
      }
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
        NewPhoto = _enroller.GetImage();
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
    private readonly IWindowManager           _windowManager      ;
    private readonly IServiceManager          _serviceManager     ;
    private readonly ICaptureDeviceEngine     _captureDeviceEngine;

    #endregion

  }
}
