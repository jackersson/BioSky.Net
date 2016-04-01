using AForge.Video.DirectShow;
using BioService;
using Google.Protobuf.Collections;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Interop;

namespace BioContracts.Common
{
  public abstract class BiometricPerformerBase
  {    
    public BiometricPerformerBase(IProcessorLocator locator )
    {
      _utils  = new BioImageUtils();
      _photos = new RepeatedField<Photo>();

      _bioService          = locator.GetProcessor<IServiceManager>();      
      _notifier            = locator.GetProcessor<INotifier>();
      
      resetTimer = new Timer(MAX_CAPTURE_TIME);
      resetTimer.Elapsed += ResetTimer_Elapsed;
      resetTimer.Stop();
    }

    public bool Busy { get { return _busy; } }

    //Returns ImageData that should be sent to Recognition Server
    public Photo GetCapturedPhoto()
    {
      return _photos.FirstOrDefault();
    }

    protected abstract void PerformRequest();
    protected abstract void SubscribeOnFeedback(bool subscribe);
    protected abstract void StartAquireDataFromDevice();
    protected abstract void StopAquireDataFromDevice ();


    //Starts verification directly from device (aquiring information)
    protected void Start(string deviceName)
    {
      if (_busy)
        return;

      Init(deviceName);      
    }

    //Starts verification from ImageData uploaded by user
    protected void Start(Photo photo)
    {
      if (_busy)
        return;

      Init();
      AddPhoto(photo);       
    }       

    protected void OnFrame(ref Bitmap bitmap)
    {
      if (bitmap == null)
        return;

      Google.Protobuf.ByteString description = _utils.ImageToByteString(bitmap);
      Photo photo = new Photo() { Bytestring = description };
      AddPhoto(photo);
    }

    private void AddPhoto( Photo photo)
    {
      _photos.Add(photo);
      Check(_photos.Count);
    }

    private void Check(int count )
    {
      if (count >= MAX_IMAGES_COUNT)
      {
        StopAquireDataFromDevice();      
        SubscribeOnFeedback(true);
        PerformRequest();        
        Reset();
      }
    }

    private void Reset()
    {      
      _busy = false;
      StopAquireDataFromDevice();
    }

    private void Init(string deviceName)
    {
      Init();
      _deviceName = deviceName;

      StartAquireDataFromDevice();
    }
    
    private void Init()
    {
      resetTimer.Start();
      SubscribeOnFeedback(false);
      Reset();
      _busy = true;      
    }
    
    //If acquiring process from device longer than MAX_CAPTURE_TIME stop process automatically
    private void ResetTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      resetTimer.Stop();
      Reset();
    }

    public string DeviceName { get { return _deviceName; } }

    private readonly Timer resetTimer;

    private string _deviceName;
    private bool   _busy      ;
  
    private readonly BioImageUtils _utils;

    private const int MAX_CAPTURE_TIME = 10000;
    private const int MAX_IMAGES_COUNT = 1    ;

    protected RepeatedField<Photo> _photos;
   
    protected readonly IServiceManager    _bioService;
    protected readonly INotifier          _notifier;
  }


  public class Enroller : BiometricPerformerBase, ICaptureDeviceObserver
  {
    public event RecognitionEventHandler EnrollmentDone;
    public Enroller( IProcessorLocator locator ) : base(locator)
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
        if (_person != null)
          data.Personid = _person.Id;
       
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

        Photo photo         = GetCapturedPhoto();
        Photo feedbackPhoto = feedback.Photo;

        if (photo == null || feedbackPhoto == null || !feedback.Success )
        {
          OnEnrolled(null, _person);
          return;
        }

        feedbackPhoto.Height     = photo.Height;
        feedbackPhoto.Width      = photo.Width;
        feedbackPhoto.Bytestring = photo.Bytestring;
        feedbackPhoto.Datetime   = DateTime.Now.Ticks;

        feedbackPhoto.OriginType = PhotoOriginType.Enrolled;
        feedbackPhoto.SizeType   = PhotoSizeType.Croped    ;
        
        OnEnrolled(photo, _person);
      }
    }

    protected override void StartAquireDataFromDevice() {
      _captureDeviceEngine.Subscribe(this, DeviceName);
    }

    protected override void StopAquireDataFromDevice() {
      _captureDeviceEngine.Unsubscribe(this);
    }

    void ICaptureDeviceObserver.OnFrame(ref Bitmap frame) {
      OnFrame(ref frame);
    }

    public void OnStop(bool stopped, string message) {}
    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) {}
    
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private Person _person;
  }

  public delegate void RecognitionEventHandler(Photo photo, Person person);


  public class Verifyer : BiometricPerformerBase, ICaptureDeviceObserver
  {
    public event RecognitionEventHandler VerificationDone;

    public Verifyer( IProcessorLocator locator ) 
                   : base(locator)
    {
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
    }

    public void Start( string deviceName, Person person)
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
       
        Photo photo         = GetCapturedPhoto();
        Photo feedbackPhoto = enrollFeedback.Photo;

        if (photo == null || feedbackPhoto == null || !enrollFeedback.Success)
        {
          OnVerified(null, _person);
          return;
        }

        feedbackPhoto.Height     = photo.Height;
        feedbackPhoto.Width      = photo.Width;
        feedbackPhoto.Bytestring = photo.Bytestring;
        feedbackPhoto.Datetime   = DateTime.Now.Ticks;

        feedbackPhoto.OriginType = PhotoOriginType.Verified;
        feedbackPhoto.SizeType   = PhotoSizeType.Full;            

        OnVerified(feedbackPhoto, _person);       
      }      
    }

    void ICaptureDeviceObserver.OnFrame(ref Bitmap frame) {
      OnFrame(ref frame);
    }

    public void OnStop (bool stopped, string message) {}
    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) {}

    protected override void StartAquireDataFromDevice() {
      _captureDeviceEngine.Subscribe(this, DeviceName);
    }

    protected override void StopAquireDataFromDevice() {
      _captureDeviceEngine.Unsubscribe(this);
    }
    
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private Person _person;
  }
  
  public class BioImageUtils
  {
    public byte[] ImageToByte(Image img)
    {
      byte[] byteArray = new byte[0];
      using (MemoryStream stream = new MemoryStream())
      {
        img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
        stream.Close();
       

        byteArray = stream.ToArray();
      }
      return byteArray;
    }

    public byte[] ImageToByte(Stream stream)
    {

      byte[] buffer = null;
      if (stream != null && stream.Length > 0)
      {
        using (BinaryReader br = new BinaryReader(stream))
        {
          buffer = br.ReadBytes((int)stream.Length);
        }
      }
      return buffer;
    }  

    public Google.Protobuf.ByteString ImageToByteString(Image img)
    {
      byte[] bytes = ImageToByte(img);      
      return Google.Protobuf.ByteString.CopyFrom(bytes);
    }

    public Google.Protobuf.ByteString ImageToByteString(Stream img)
    {
      byte[] bytes = ImageToByte(img);
      return Google.Protobuf.ByteString.CopyFrom(bytes);
    }     

  }
}
