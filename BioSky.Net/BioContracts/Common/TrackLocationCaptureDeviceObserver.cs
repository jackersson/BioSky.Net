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
  public class BiometricPerformerBase
  {    
    public BiometricPerformerBase(IProcessorLocator locator )
    {
      _utils  = new BioImageUtils();
      _photos = new RepeatedField<Photo>();

      _bioService          = locator.GetProcessor<IServiceManager>();
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      _notifier            = locator.GetProcessor<INotifier>();
      
      resetTimer = new Timer(MAX_CAPTURE_TIME);
      resetTimer.Elapsed += ResetTimer_Elapsed;
      resetTimer.Stop();
    }

    protected void Start(string deviceName)
    {
      if (_busy)
        return;

      Init(deviceName);      
    }

    protected void Start(Photo photo)
    {
      if (_busy)
        return;

      Init();
      AddPhoto(photo);       
    }   

    public bool Busy
    {
       get { return _busy; }
    }

    public Photo GetCapturedPhoto()
    {     
      return _photos.FirstOrDefault();  
    }

    private void OnImage(object sender, ref Bitmap bitmap)
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
        _captureDeviceEngine.Unsubscribe(OnImage, _deviceName);
        SubscribeOnFeedback(true);
        PerformRequest();        
        Reset();
      }
    }

    //Only this to override
    protected virtual void PerformRequest() { }

    protected virtual void SubscribeOnFeedback( bool subscribe ) { }

    private void Reset()
    {      
      _busy = false;      
      _captureDeviceEngine.Unsubscribe(OnImage, _deviceName);      
    }

    private void Init(string deviceName)
    {
      Init();
      _deviceName = deviceName;     
      _captureDeviceEngine.Add(deviceName);
      _captureDeviceEngine.Subscribe(OnImage, deviceName);
    }

    private void ResetTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      resetTimer.Stop();
      Reset();
    }

    private void Init()
    {
      resetTimer.Start();
      SubscribeOnFeedback(false);
      Reset();
      _busy = true;      
    }
    
    private readonly Timer resetTimer;
    private const int MAX_CAPTURE_TIME = 10000;

    private string _deviceName;
    private bool   _busy      ;
  
    private readonly BioImageUtils _utils;    

    private const int MAX_IMAGES_COUNT = 1;
    protected RepeatedField<Photo> _photos;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    protected readonly IServiceManager _bioService;
    protected readonly INotifier         _notifier;
  }


  public class Enroller : BiometricPerformerBase
  {
    public event RecognitionEventHandler EnrollmentDone;
    public Enroller( IProcessorLocator locator ) : base(locator) { }

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
    
    private Person _person;
  }

  public delegate void RecognitionEventHandler(Photo photo, Person person);


  public class Verifyer : BiometricPerformerBase
  {
    public event RecognitionEventHandler VerificationDone;

    public Verifyer( IProcessorLocator locator ) 
                   : base(locator)
    {}

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
    
    private Person _person;
  }


  public class TrackLocationCaptureDeviceObserver
  {
    public TrackLocationCaptureDeviceObserver(IProcessorLocator locator)
    {
      Init(locator);
      _captureDeviceEngine.ListenerStart += OnDeviceChanged;
    }

    public TrackLocationCaptureDeviceObserver(IProcessorLocator locator, string deviceName)
    {
      Init(locator);
      Update(deviceName);
    }

    public TrackLocationCaptureDeviceObserver(IProcessorLocator locator, CaptureDevice captureDevice)
    {
      Init(locator);
      Update(captureDevice.Devicename);
    }   

    public void Update(string deviceName)
    {
      if (DeviceName != null && DeviceName != "")      
        _captureDeviceEngine.Remove(DeviceName);      

      DeviceName = deviceName;
      _captureDeviceEngine.Add(DeviceName);     
    }

    public void ShowPropertyPage()
    {
      System.Windows.Window window = _locator.GetProcessor<System.Windows.Window>();
      IntPtr parentWindow = new WindowInteropHelper(window).Handle;
      _captureDeviceEngine.ShowCaptureDevicePropertyPage(DeviceName, parentWindow);
    }

    public void Subscribe(FrameEventHandler handler)
    {
      _captureDeviceEngine.Subscribe(handler, DeviceName);
    }

    public void Unsubscribe(FrameEventHandler handler)
    {
      _captureDeviceEngine.Unsubscribe(handler, DeviceName);
    }

    public void Stop()
    {        
      _captureDeviceEngine.Remove(DeviceName);
    }

    public VideoCapabilities[] GetVideoCapabilities()
    {
      return _captureDeviceEngine.GetCaptureDeviceVideoCapabilities(DeviceName);
    }
    public void SetVideoCapabilities(int selectedResolution)
    {
      _captureDeviceEngine.SetCaptureDeviceVideoCapabilities(DeviceName, selectedResolution);
    }

    public VideoCapabilities GetVideoResolution()
    {
      return _captureDeviceEngine.GetVideoResolution(DeviceName);
    }

    private void Init(IProcessorLocator locator)
    {
      _locator = locator;
      _captureDeviceEngine = _locator.GetProcessor<ICaptureDeviceEngine>();
    }

    private string _deviceName;
    public string DeviceName
    {
      get { return _deviceName; }
      private set
      {
        if (_deviceName != value)
          _deviceName = value;
      }
    }

    public event DeviceChangedEventHandler DeviceChanged;
    public delegate void DeviceChangedEventHandler();

    private void OnDeviceChanged()
    {
      if (DeviceChanged != null)
        DeviceChanged();
    }

    private IProcessorLocator _locator      ;

    private ICaptureDeviceEngine _captureDeviceEngine;
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
