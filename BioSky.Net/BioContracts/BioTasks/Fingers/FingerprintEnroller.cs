using BioContracts.FingerprintDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BioService;
using BioContracts.BioTasks.Utils;

namespace BioContracts.BioTasks.Fingers
{
  public class FingerprintEnroller : IFingerprintDeviceObserver, IFingerprintServiceObserver
  {

    public FingerprintEnroller(IProcessorLocator locator)
    {
      _locator    = locator;
      _bioService = locator.GetProcessor<IServiceManager>();
      _deviceEngine = locator.GetProcessor<IFingerprintDeviceEngine>();

      _utils = new BioImageUtils();
      Busy = false;
      _isActive = false;
    }

    public void Start(string deviceName, Person person)
    {
      if (IsActive)
        return;

      _deviceEngine.Add(deviceName);
      _deviceEngine.Subscribe(this, deviceName);

      _bioService.FingerprintService.Subscribe(this);

      _isActive = true;
    }

    public void Stop()
    { 
      _deviceEngine.Unsubscribe(this);
      _bioService.FingerprintService.Unsubscribe(this);
      _isActive = false;
    }

    public void OnError(Exception ex){}

    public void OnFrame(ref Bitmap frame)
    {
      

      if (Busy)
        return;

      Bitmap newFrame = frame;
      //Task task = Task.Factory.StartNew( () => Performer(newFrame));
      Performer(newFrame);
    }

    private async void Performer( Bitmap image )
    {
      Busy = true;
      try
      {
        FingerprintData data = new FingerprintData();
        Google.Protobuf.ByteString description = _utils.ImageToByteString(image);
        data.Image = new Photo() { Bytestring = description, Width = image.Width, Height = image.Height };

        await _bioService.FingerprintService.Enroll(data);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        //_notifier.Notify(e);
      }

      Busy = false;
    }

    public void OnMessage(string message){}

    public void OnReady(bool isReady){}

    public void OnFeedback(EnrollmentFeedback feedback)
    {
      Console.WriteLine(feedback);
    }

    private   readonly BioImageUtils     _utils;
    protected readonly IServiceManager   _bioService;
    private   readonly IProcessorLocator _locator;

    private readonly IFingerprintDeviceEngine _deviceEngine;
    private bool Busy { get; set; }


    private bool _isActive;
    public bool IsActive { get { return _isActive; } }
  }
}
