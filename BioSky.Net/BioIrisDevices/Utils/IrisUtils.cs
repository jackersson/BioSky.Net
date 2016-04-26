using BioContracts.Common;
using BioContracts.IrisDevices;
using Iddk2000DotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BioIrisDevices.Utils
{  
  public class IddkCaptureConfig
  {
    public IddkCaptureConfig()
    {
      ImageWidth  = new IddkInteger();
      ImageHeight = new IddkInteger();
      EyesData    = new Eyes();
      Reset();
    }

    public Eyes EyesData { get; private set; }

    public IddkInteger ImageWidth  { get; private set; }
    public IddkInteger ImageHeight { get; private set; }

    public IddkEyeSubtype           EyeSubtype           { get; set; }
    public IddkCaptureMode          CaptureMode          { get; set; }
    public IddkQualityMode          QualityMode          { get; set; }
    public IddkCaptureOperationMode CaptureOperationMode { get; set; }
    public IddkImageFormat          StreamFormat         { get; set; }

    public bool StreamMode      { get; set; }
    public bool AutoLeds        { get; set; }
    public int CaptureModeParam { get; set; }


    //TODO maybe in Utils 
    public Bitmap RawDataToBitmap(byte[] imgData, int width, int height)
    {
      GrayScaleBitmap bitmap = new GrayScaleBitmap(width, height, imgData);

      using (MemoryStream BmpStream = new MemoryStream(bitmap.BitmatFileData))
      {
        return new Bitmap(BmpStream);
      }
    }

    public Eyes GetEyes(List<IddkImage> images)
    {
      EyesData.Reset();

      if (images == null || images.Count <= 0)
        return EyesData;

      IddkImage leye = null;
      IddkImage reye = null;
      if (EyeSubtype == IddkEyeSubtype.Both && images.Count > 1)
      {
        leye = images.FirstOrDefault();
        reye = images.LastOrDefault();
      }
      else if (images.Count == 1)
      {
        if (EyeSubtype == IddkEyeSubtype.Left)
          leye = images.FirstOrDefault();
        else if (EyeSubtype == IddkEyeSubtype.Right)
          reye = images.FirstOrDefault();
      }

      if (leye != null)
        EyesData.LeftEye = RawDataToBitmap(leye.ImageData, leye.ImageWidth, leye.ImageHeight);

      if (reye != null)
        EyesData.RightEye = RawDataToBitmap(reye.ImageData, reye.ImageWidth, reye.ImageHeight);

      return EyesData;
    }

    public void Reset()
    {
      EyeSubtype           = IddkEyeSubtype.Both;
      CaptureMode          = IddkCaptureMode.FrameBased;
      QualityMode          = IddkQualityMode.Normal;
      CaptureOperationMode = IddkCaptureOperationMode.AutoCapture;

      StreamMode       = false;
      AutoLeds         = false;
      CaptureModeParam = 4;
    }
  }


  public class IrisUtils
  {
    private static volatile IrisUtils _instance;
    public static IrisUtils Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (syncObject)
          {
            if (_instance == null)
              _instance = new IrisUtils();
          }
        }
        return _instance;
      }
    }
    public string GetErrorMessage(IddkResult error )
    {     
      bool   recovery      = false;
      string error_message = string.Empty;
      
      if (error == IddkResult.DeviceNotFound)
        error_message = "No IriTech devices found !";
      else if (error == IddkResult.DeviceIOFailed || error == IddkResult.DeviceIOTimeout || error == IddkResult.DeviceIODataInvalid)
      {      
        error_message = "Try to recover from error !";       
        recovery = true;
   
        if (recovery)
        {         
          int errorLevel = -1;
          //IddkRecoveryCode recoveryCode;
          if (error != IddkResult.DeviceIODataInvalid && errorLevel == -1)
            errorLevel = 0;

          errorLevel++;

          switch (errorLevel)
          {
            case 0:
              error_message = "\nFirst time the error is detected. We suggest to flush invalid data from IO buffers and abort any pending IO operations !\n";
              //recoveryCode = IddkRecoveryCode.UsbCancelIO;
              break;
            case 1:
              error_message = "\nWe suggest to reset pipes in this time !\n";
              //recoveryCode = IddkRecoveryCode.UsbResetPipes;
              break;
            case 2:
              error_message = "\nWe suggest to reset port in this time !\n";
              //recoveryCode = IddkRecoveryCode.UsbResetPort;
              break;
            case 3:
              error_message = "\nWe suggest to cycle port in this time ! Note that users should scan device and open device again !\n";
              //recoveryCode = IddkRecoveryCode.UsbCyclePort;
              break;
            case 4:
              error_message = "\nWe suggest to soft reset device in this time !\n";
              //recoveryCode = IddkRecoveryCode.SoftReset;
              break;
            case 5:
              error_message = "We donot support this level recovery at the moment";            
              errorLevel = -1;
              break;

            default:
              //recoveryCode = IddkRecoveryCode.SoftReset;
              break;
          }

          
          /*
          result = apis.Recovery(recoveryCode);
          if (result == IddkResult.OK)
          {
            Console.Out.Write("\nRecovery OK !\n");

            if (recoveryCode == IddkRecoveryCode.UsbCyclePort || recoveryCode == IddkRecoveryCode.SoftReset)
            {
              //Wait for sure ...
              Console.Out.Write("\nPlease wait for a while ...\n");
              Iddk2000Utils.Wait(10000);

              //Scan and open device again to get the valid handle
              OpenDevice();
            }
          }
          else
          {
            Console.Out.Write("\nRecovery failed !\n");
          }
          */
        }
      }

      return string.Format("Error code {0}, Message : {1}", error, error_message);
    }

    private static object syncObject = new object();
  }
}
