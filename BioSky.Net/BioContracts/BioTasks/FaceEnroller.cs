using AForge.Video.DirectShow;
using BioContracts.CaptureDevices;
using BioContracts.Common;
using BioContracts.Locations;
using BioService;
using System;
using System.Drawing;

namespace BioContracts.BioTasks
{
  public class FaceEnroller : BiometricPerformerBase, ICaptureDeviceObserver
  {
    public event RecognitionEventHandler EnrollmentDone;
    public FaceEnroller(IProcessorLocator locator) : base(locator)
    {
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
    }

    public void Start(string deviceName, Person person)
    {
      _person = null;
      _person = person;

      Start(deviceName);
    }

    public void Start(Photo photo, Person person)
    {
      _person = null;
      _person = person;

      Start(photo);
    }

    protected override void SubscribeOnFeedback(bool subscribe)
    {
      if (subscribe)
        _bioService.FaceService.EnrollFeedbackChanged += OnFeedback;
      else
        _bioService.FaceService.EnrollFeedbackChanged -= OnFeedback;
    }

    private void OnEnrolled(Photo photo, Person person)
    {
      if (EnrollmentDone != null)
        EnrollmentDone(photo, person);
    }

    protected async override void PerformRequest()
    {
      try
      {
        EnrollmentData data = new EnrollmentData();
        data.Images.Add(_photos);
       /// if (_person != null)
         // data.Personid = _person.Id;

        await _bioService.FaceService.Enroll(data);
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }
    }

    private void OnFeedback(object sender, EnrollmentFeedback feedback)
    {
      if (feedback == null)
        return;

      if (feedback.Progress == 100)
      {
        SubscribeOnFeedback(false);

        Photo photo = GetCapturedPhoto();
        Photo feedbackPhoto = null;// feedback.Photo;

        if (photo == null || feedbackPhoto == null || feedback.Result != Result.Success)
        {
          OnEnrolled(null, _person);
          return;
        }

        feedbackPhoto.Height = photo.Height;
        feedbackPhoto.Width = photo.Width;
        feedbackPhoto.Bytestring = photo.Bytestring;
        feedbackPhoto.Datetime = DateTime.Now.Ticks;

        feedbackPhoto.OriginType = PhotoOriginType.Enrolled;
        //feedbackPhoto.SizeType = PhotoSizeType.Croped;

        OnEnrolled(photo, _person);
      }
    }

    protected override void StartAquireDataFromDevice()
    {
      _captureDeviceEngine.Subscribe(this, DeviceName);
    }

    protected override void StopAquireDataFromDevice()
    {
      _captureDeviceEngine.Unsubscribe(this);
    }

    void ICaptureDeviceObserver.OnFrame(ref Bitmap frame)
    {
      OnFrame(ref frame);
    }

    public void OnStop(bool stopped, ErrorMessage message, LocationDevice device) { }
    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) { }

    public void OnMessage(string message)
    {
      Console.WriteLine(message);
    }

    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private Person _person;
  }

  public delegate void RecognitionEventHandler(Photo photo, Person person);

}
