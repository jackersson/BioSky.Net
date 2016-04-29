using AForge.Video.DirectShow;
using BioContracts.CaptureDevices;
using BioContracts.Locations;
using BioService;
using System;
using System.Drawing;

namespace BioContracts.BioTasks
{
  public class FaceVeryfier : BiometricPerformerBase, ICaptureDeviceObserver
  {
    public event RecognitionEventHandler VerificationDone;

    public FaceVeryfier(IProcessorLocator locator)
                   : base(locator)
    {
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
    }

    public void Start(string deviceName, Person person)
    {
      _person = null;
      _person = person;

      Start(deviceName);
    }

    protected override void SubscribeOnFeedback(bool subscribe)
    {
      if (subscribe)
        _bioService.FaceService.VerifyFeedbackChanged += OnFeedback;
      else
        _bioService.FaceService.VerifyFeedbackChanged -= OnFeedback;
    }

    protected async override void PerformRequest()
    {
      try
      {
        VerificationData data = new VerificationData();
        data.Images.Add(_photos);
        //data.Person  = _visitor.Personid;       
        await _bioService.FaceService.Verify(data);
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }
    }

    private void OnVerified(Photo photo, Person person)
    {
      if (VerificationDone != null)
        VerificationDone(photo, person);
    }

    private void OnFeedback(object sender, VerificationFeedback feedback)
    {
      EnrollmentFeedback enrollFeedback = feedback.EnrollmentFeedback;
      if (enrollFeedback == null)
        return;

      if (enrollFeedback.Progress == 100)
      {
        SubscribeOnFeedback(false);

        Photo photo = GetCapturedPhoto();
        Photo feedbackPhoto = null;// enrollFeedback.Photo;

        if (photo == null || feedbackPhoto == null || enrollFeedback.Result != Result.Success)
        {
          OnVerified(null, _person);
          return;
        }

        feedbackPhoto.Height = photo.Height;
        feedbackPhoto.Width = photo.Width;
        feedbackPhoto.Bytestring = photo.Bytestring;
        feedbackPhoto.Datetime = DateTime.Now.Ticks;

        feedbackPhoto.OriginType = PhotoOriginType.Verified;
        //feedbackPhoto.SizeType = PhotoSizeType.Full;

        OnVerified(feedbackPhoto, _person);
      }
    }

    void ICaptureDeviceObserver.OnFrame(ref Bitmap frame)
    {
      OnFrame(ref frame);
    }

    public void OnStop(bool stopped, string message, LocationDevice device) { }
    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) { }

    protected override void StartAquireDataFromDevice()
    {
      _captureDeviceEngine.Subscribe(this, DeviceName);
    }

    protected override void StopAquireDataFromDevice()
    {
      _captureDeviceEngine.Unsubscribe(this);
    }

    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private Person _person;
  }
}
