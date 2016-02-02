using BioFaceService;
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

      BioImage image = _utils.ImageToBioImage(bitmap);
      UpdateData(image);

    }

    protected virtual void UpdateData( BioImage image )
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

    public BioImage GetImage()
    {
      if (_data.Images == null)
        _data.Images = new BioImagesList();
      RepeatedField<BioImage> images = _data.Images.Images;
      return images[0];
    }

    protected override void UpdateData(BioImage image)
    {
      if (_data.Images == null)
        _data.Images = new BioImagesList();
      RepeatedField<BioImage> images = _data.Images.Images;
      images.Add(image);
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

    public BioImage ImageToBioImage(Image img)
    {
      Google.Protobuf.ByteString description = ImageToByteString(img);
      return new BioImage() { Description = description };
    }
  }
}
