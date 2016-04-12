using BioContracts.BioTasks.Utils;
using BioContracts.Common;
using BioService;
using Google.Protobuf.Collections;
using System.Drawing;
using System.Linq;
using System.Timers;


namespace BioContracts.BioTasks
{
  public abstract class BiometricPerformerBase
  {
    public BiometricPerformerBase(IProcessorLocator locator)
    {
      _utils = new BioImageUtils();
      _photos = new RepeatedField<Photo>();

      _bioService = locator.GetProcessor<IServiceManager>();
      _notifier = locator.GetProcessor<INotifier>();

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
    protected abstract void StopAquireDataFromDevice();


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

    private void AddPhoto(Photo photo)
    {
      _photos.Add(photo);
      Check(_photos.Count);
    }

    private void Check(int count)
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
    private bool _busy;

    private readonly BioImageUtils _utils;

    private const int MAX_CAPTURE_TIME = 10000;
    private const int MAX_IMAGES_COUNT = 1;

    protected RepeatedField<Photo> _photos;

    protected readonly IServiceManager _bioService;
    protected readonly INotifier _notifier;
  }

}
