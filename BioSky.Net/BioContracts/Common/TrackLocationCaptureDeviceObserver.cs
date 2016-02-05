using BioService;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Common
{
  public class BiometricPerformerBase<T>
  {
    public BiometricPerformerBase(ICaptureDeviceEngine captureDeviceEngine)
    {
      _utils               = new BioImageUtils();  

      _captureDeviceEngine = captureDeviceEngine;
    }

    public void Start(string deviceName, T data)
    {
      if (_busy)
        return;

      Init(deviceName, data);

      _captureDeviceEngine.Subscribe(OnImage, deviceName);
    }

    public void Start(string deviceName, Bitmap image, T data)
    {
      if (_busy)
        return;

      Init(deviceName, data);

      OnImage(null, ref image);
    }   

    private void OnImage(object sender, ref Bitmap bitmap)
    {
      if (bitmap == null)
        return;

      Google.Protobuf.ByteString description = _utils.ImageToByteString(bitmap);
      Photo image = new Photo() { Description = description };
      UpdateData(image);

    }

    protected virtual void UpdateData( Photo image )
    {

    }

    protected void Check(int count )
    {
      if (count >= MAX_IMAGES_COUNT)
      {
        _captureDeviceEngine.Unsubscribe(OnImage, _deviceName);
        PerformRequest();
      }
    }

    protected virtual void PerformRequest()
    {
      Console.WriteLine("Request performed");

      Reset();
    }

    private void Reset()
    {
      _busy = false;     
      _captureDeviceEngine.Unsubscribe(OnImage, _deviceName);      
    }

    private void Init(string deviceName, T data)
    {
      Reset();

      _data       = data;
      _deviceName = deviceName;
      _captureDeviceEngine.Add(deviceName);
    }

    protected T _data;

    private string _deviceName;
    private bool   _busy      ;

    private readonly BioImageUtils _utils;    

    private const int MAX_IMAGES_COUNT = 1;

    private readonly ICaptureDeviceEngine _captureDeviceEngine;    
  }


  public class Enroller : BiometricPerformerBase<EnrollmentData>
  {

    public Enroller( ICaptureDeviceEngine captureDeviceEngine
                   , IServiceManager bioService) : base(captureDeviceEngine)
    {
      _bioService = bioService;
    }

    protected async override void PerformRequest()
    {   
      await _bioService.FaceService.Enroll(_data);
      base.PerformRequest();
    }

    public Photo GetImage()
    {
      if (_data != null && _data.Images.Count > 0)
        return _data.Images[0];
      else
        return null;
    }

    protected override void UpdateData(Photo item)
    {
      RepeatedField<Photo> images = _data.Images;
      images.Add(item);    
      Check(images.Count);
    }

    private readonly IServiceManager _bioService;
  }


  public class Verifyer : BiometricPerformerBase<VerificationData>
  {

    public Verifyer( ICaptureDeviceEngine captureDeviceEngine
                   , IServiceManager bioService) 
                   : base(captureDeviceEngine)
    {
      _bioService = bioService;
    }

    protected async override void PerformRequest()
    {
      await _bioService.FaceService.Verify(_data);
      base.PerformRequest();
    }

    public Photo GetImage()
    {
      if (_data != null && _data.Images.Count > 0)
        return _data.Images[0];
      else
        return null;
    }

    public VerificationData GetData()
    {
      return _data;
    }

    protected override void UpdateData(Photo item)
    {
      RepeatedField<Photo> images = _data.Images;
      images.Add(item);
      Check(images.Count);
    }

    private readonly IServiceManager _bioService;
  }


  public class TrackLocationCaptureDeviceObserver
  {

    public TrackLocationCaptureDeviceObserver(IProcessorLocator locator, CaptureDevice captureDevice)
    {
      _locator       = locator;
      _captureDevice = captureDevice;

      _captureDeviceEngine = _locator.GetProcessor<ICaptureDeviceEngine>();        

      _captureDeviceEngine.Add(_captureDevice.Devicename);     
    }

    private Enroller _enrollPerformer;
    public Enroller EnrollPerformer
    {
      get { return _enrollPerformer; }
    } 

    private readonly CaptureDevice     _captureDevice; 
    private readonly IProcessorLocator _locator      ;

    private readonly ICaptureDeviceEngine _captureDeviceEngine;
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

    public Google.Protobuf.ByteString ImageToByteString(Image img)
    {
      byte[] bytes = ImageToByte(img);      
      return Google.Protobuf.ByteString.CopyFrom(bytes);
    }
  }
}
