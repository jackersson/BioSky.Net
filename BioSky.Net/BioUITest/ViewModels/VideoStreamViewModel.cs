using System;

using Caliburn.Micro;
using AForge.Video.DirectShow;
using AForge.Video;

using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Threading;

using BioContracts;



namespace BioUITest.ViewModels
{

  /*
  public class ItemType : PropertyChangedBase
  {
    public string ItemText
    {
      get { return (ItemEnabled) ? "True" : "False"; }
    }

    private bool _itemEnabled;
    public bool ItemEnabled
    {
      get
      {
        return _itemEnabled;
      }
      set
      {
        if ( _itemEnabled != value)
        {
          _itemEnabled = value;
          NotifyOfPropertyChange(() => ItemEnabled);
          NotifyOfPropertyChange(() => ItemText);
        }
      }
    }
  }
  */
  public static class BitmapConversion
  {
    public static BitmapSource BitmapToBitmapSource(Bitmap source)
    {

      return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    source.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
    }
  }

  public class VideoStreamViewModel : Screen
  {

    private AForge.Video.IVideoSource videoSource;

    //private BioServiceManager _bioFaceServiceManager;

    public VideoStreamViewModel()
    {
      
      DevicesNames = new FilterInfoCollection(FilterCategory.VideoInputDevice);
      /*
      _bioFaceServiceManager = new BioServiceManager();
      _bioFaceServiceManager.Start();

      _bioFaceServiceManager.FaceClient.FaceDetected += FaceClient_FaceDetected;

      _bioFaceServiceManager.FaceClient.EnrollFeedback += FaceClient_EnrollFeedback; ;

      _faces = new AsyncObservableCollection<BioFaceService.Rectangle>();

      // ItemTest = new ObservableCollection<ItemType>();

      // ItemTest.Add(new ItemType() { ItemEnabled = false });
      // ItemTest.Add(new ItemType() { ItemEnabled = true });
      // ItemTest = _itemTest;

      ItemTest = new BioVideoPlayerViewModel();

      ItemTest.ItemTest.Add(new ItemType() { ItemEnabled = false });
      ItemTest.ItemTest.Add(new ItemType() { ItemEnabled = true });
      */

      _faces = new AsyncObservableCollection<Rectangle>();

      //_faces.Add(new BioFaceService.Rectangle() { FacePos = new BioFaceService.Point() { XPos = 1, YPos = 10 }, Height = 100, Width = 100 });
      _faces.Add(new Rectangle() { X = 1, Y = 10,  Height = 100, Width = 100 });
      Faces = _faces;

    }

    public BioVideoPlayerViewModel _itemTest;
    public BioVideoPlayerViewModel ItemTest
    {
      get { return _itemTest; }
      set
      {
        if (_itemTest != value)
        {
          _itemTest = value;
          NotifyOfPropertyChange(() => ItemTest);
        }
      }
    }
    /*
    public ObservableCollection<ItemType> _itemTest;
    public ObservableCollection<ItemType> ItemTest
    {
      get { return _itemTest; }
      set
      {
        if ( _itemTest != value )
        {
          _itemTest = value;
          NotifyOfPropertyChange(() => ItemTest);
        }
      }
    }
    */
    public void EnableItem()
    {
      //foreach (ItemType it in ItemTest.ItemTest)
      //it.ItemEnabled = !it.ItemEnabled;

      ItemTest.SelectedItemTest = null;

      //ItemTest.Add(new ItemType() { ItemEnabled = true });
    }

    private void FaceClient_EnrollFeedback(object sender, BioFaceService.EnrollmentFeedback feedback)
    {
      /*
      Progress = feedback.Progress;
      if (Progress == 0)
        ProgressInfo = "Enrollmment beginnig";
      else if (Progress == 100)
        ProgressInfo = "Enrollment " + (feedback.Success ? "Success" : "Failed");
      else
        ProgressInfo = "Enrollment procesing... " + (feedback.Eyesfound ? "Eyes found" : "");
        */
    }

    private void FaceClient_FaceDetected(object sender, BioFaceService.DetectedObjectsInfo objectsInfo)
    {
      /*
      _faces.Clear();
      foreach (BioFaceService.ObjectInfo oi in objectsInfo.Objects)
      {
        _faces.Add(oi.ObjectLocation);

        Console.WriteLine(oi.ObjectLocation.Width + " " + oi.ObjectLocation.Height + " " + oi.ObjectLocation.FacePos.XPos + " " + oi.ObjectLocation.FacePos.YPos);
      }

      Faces = _faces;
      */
      //NotifyOfPropertyChange(() => Faces);
    }

    private FilterInfoCollection _devicesNames;
    public FilterInfoCollection DevicesNames
    {
      get { return _devicesNames; }
      set
      {
        if (_devicesNames != value)
        {
          _devicesNames = value;
          NotifyOfPropertyChange(() => DevicesNames);
        }
      }
    }

    private FilterInfo _selectedDevice;
    public FilterInfo SelectedDevice
    {
      get { return _selectedDevice; }
      set
      {
        if (_selectedDevice != value)
        {
          _selectedDevice = value;
          NotifyOfPropertyChange(() => SelectedDevice);
          OpenVideoSource();
        }
      }
    }

    private int _progress;
    public int Progress
    {
      get { return _progress; }
      set
      {
        if (_progress != value)
        {
          _progress = value;
          NotifyOfPropertyChange(() => Progress);
        }
      }
    }

    private string _progressInfo;
    public string ProgressInfo
    {
      get { return _progressInfo; }
      set
      {
        if (_progressInfo != value)
        {
          _progressInfo = value;
          NotifyOfPropertyChange(() => ProgressInfo);
        }
      }
    }


    private int _frameWidth;
    public int FrameWidth
    {
      get { return _frameWidth; }
      set
      {
        if (_frameWidth != value)
        {
          _frameWidth = value;
          NotifyOfPropertyChange(() => FrameWidth);
        }
      }
    }

    private int _frameHeight;
    public int FrameHeight
    {
      get { return _frameHeight; }
      set
      {
        if (_frameHeight != value)
        {
          _frameHeight = value;
          NotifyOfPropertyChange(() => FrameHeight);
        }
      }
    }


    private BitmapSource _newFrame;
    public BitmapSource NewFrame
    {
      get
      {
        return _newFrame;
      }
      set
      {
        if (_newFrame != value)
        {
          _newFrame = value;
          NotifyOfPropertyChange(() => NewFrame);
        }
      }
    }


    public void UpdateImage(BitmapSource img)
    {
      img.Freeze();

      Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render
        , new System.Action(() =>
      {
        NewFrame = img;
      }));
    }

    public static byte[] ImageToByte(Image img)
    {
      ImageConverter converter = new ImageConverter();
      return (byte[])converter.ConvertTo(img, typeof(byte[]));
    }

    public static byte[] ImageToByte2(Image img)
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

    private static BitmapImage LoadImage(byte[] imageData)
    {
      if (imageData == null || imageData.Length == 0) return null;
      var image = new BitmapImage();
      using (var mem = new MemoryStream(imageData))
      {
        Image.FromStream(mem).Save("F:\\C#\\BioSkyNetSuccess\\BioSky.Net\\BioSky.Net\\BioUITest\\4.jpg");

      }

      return image;
    }


    public IVideoSource VideoSource
    {
      get { return videoSource;  }
      set
      {
        if (videoSource != value)
        {
          videoSource = value;
          NotifyOfPropertyChange(() => VideoSource);
        }
      }
    }


    private void CloseVideoSource()
    {
      if (VideoSource == null)
        return;
      // stop current video source
      VideoSource.SignalToStop();

      // wait 2 seconds until camera stops
      for (int i = 0; (i < 50) && (VideoSource.IsRunning); i++)
      {
        Thread.Sleep(100);
      }
      if (VideoSource.IsRunning)
        VideoSource.Stop();     
    }


    private VideoDetector vd = new VideoDetector();
    private void OpenVideoSource()
    {
      //CloseVideoSource();

      //videoSource.Source = SelectedDevice.MonikerString;
      VideoSource = new VideoCaptureDevice(SelectedDevice.MonikerString);
      VideoSource.NewFrame += VideoSource_NewFrame;
    //  VideoSource.Start();
      //videoSource.NewFrame += VideoSource_NewFrame;
    }

    private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
    {
      Bitmap image = eventArgs.Frame;
      Rectangle[] regions = vd.Detect(ref image);

      Faces.Clear();
      if ( regions.Length > 0)
      {
        foreach (Rectangle r in regions)
        {
          Console.WriteLine(r);
          Faces.Add(r);
        }        
      }
      
     // eventArgs.Frame
    }

    /*
    private async void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
    {
      /*
      Bitmap newFrame = (Bitmap)eventArgs.Frame.Clone();
      if (newFrame != null)
      {
        IntPtr hBitMap = newFrame.GetHbitmap();
        UpdateImage(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitMap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));

        if (_enroll)
        {
          byte[] bytes = ImageToByte2(newFrame);
          Google.Protobuf.ByteString bs = Google.Protobuf.ByteString.CopyFrom(bytes);
          BioFaceService.BioImage imageRequest = new BioFaceService.BioImage() { Description = bs };
          _processingImages.Images.Add(imageRequest);

          if (_processingImages.Images.Count >= 4)
          {
            _enroll = false;
            await _bioFaceServiceManager.FaceClient.EnrollFace(_processingImages);
          }
        }

      }
      */
    // }
    /*
    public async void CloseSocket()
    {
      /*
      Image newFrame = Bitmap.FromFile("F:\\C#\\BioSkyNetSuccess\\BioSky.Net\\BioSky.Net\\BioUITest\\1.jpg");
      byte[] bytes = ImageToByte2(newFrame);


      BioFaceService.BioImagesList imageList = new BioFaceService.BioImagesList();


      Google.Protobuf.ByteString bs = Google.Protobuf.ByteString.CopyFrom(bytes);

      BioFaceService.BioImage imageRequest = new BioFaceService.BioImage() { Description = bs };
      imageList.Images.Add(imageRequest);
      imageList.Images.Add(imageRequest);

      // await _bioFaceServiceManager.FaceClient.Identify(imageList);
      await _bioFaceServiceManager.FaceClient.Identify(imageList);
      */
    //}


    private AsyncObservableCollection<Rectangle> _faces;
    public AsyncObservableCollection<Rectangle> Faces
    {
      get { return _faces; }
      set
      {
        if (_faces != value)
        {
          _faces = value;
          NotifyOfPropertyChange(() => Faces);
        }
      }
    }

    
    public async void Enroll()
    {

      VideoDetector vd = new VideoDetector();
      Bitmap newFrame = (Bitmap)Image.FromFile("F:\\C#\\BioSkyNetSuccess\\BioSky.Net\\BioSky.Net\\BioUITest\\1.jpg");
      vd.Detect(ref newFrame);
      //vd.Detect(ref newFrame);

      // _processingImages.Images.Clear();
      // _enroll = true;
      //
    }

    public async void PersonRequest()
    {
      //BioFaceService.CommandPerson cmd = new BioFaceService.CommandPerson();
      //await _bioFaceServiceManager.FaceClient.PersonRequest(cmd);
    }

    public async void VisitorRequest()
    {
      /*
      BioFaceService.CommandPerson cmd1 = new BioFaceService.CommandPerson();
      await _bioFaceServiceManager.FaceClient.PersonRequest(cmd1);

      BioFaceService.CommandVisitor cmd2 = new BioFaceService.CommandVisitor();
      await _bioFaceServiceManager.FaceClient.VisitorRequest(cmd2);

      BioFaceService.CommandAccessDevice cmd3 = new BioFaceService.CommandAccessDevice();
      await _bioFaceServiceManager.FaceClient.AccessDeviceRequest(cmd3);

      BioFaceService.CommandCaptureDevice cmd4 = new BioFaceService.CommandCaptureDevice();
      await _bioFaceServiceManager.FaceClient.CaptureDeviceRequest(cmd4);

      BioFaceService.CommandCard cmd5 = new BioFaceService.CommandCard();
      await _bioFaceServiceManager.FaceClient.CardRequest(cmd5);

      BioFaceService.CommandLocation cmd6 = new BioFaceService.CommandLocation();
      await _bioFaceServiceManager.FaceClient.LocationRequest(cmd6);

      BioFaceService.CommandPhoto cmd7 = new BioFaceService.CommandPhoto();
      await _bioFaceServiceManager.FaceClient.PhotoRequest(cmd7);
      */
      //}

      //private bool _enroll = false;
      // private BioFaceService.BioImagesList _processingImages = new BioFaceService.BioImagesList();


      /*
    public async void SendImage()
    {
      /*
      Image newFrame = Bitmap.FromFile("F:\\C#\\BioSkyNetSuccess\\BioSky.Net\\BioSky.Net\\BioUITest\\1.jpg");
      byte[] bytes = ImageToByte2(newFrame);


      BioFaceService.BioImagesList imageList = new BioFaceService.BioImagesList();


      Google.Protobuf.ByteString bs = Google.Protobuf.ByteString.CopyFrom(bytes);

      BioFaceService.BioImage imageRequest = new BioFaceService.BioImage() { Description = bs };
      imageList.Images.Add(imageRequest);
      imageList.Images.Add(imageRequest);

      // await _bioFaceServiceManager.FaceClient.Identify(imageList);
      await _bioFaceServiceManager.FaceClient.EnrollFace(imageList);
      /*
      BioFaceService.VerificationData vd = new BioFaceService.VerificationData();
      vd.Images = imageList;
      vd.VerificationTarget = "F:\\C#\\BioSkyNetSuccess\\BioSky.Net\\BioSky.Net\\BioUITest\\123.fir";

      await _bioFaceServiceManager.FaceClient.VerifyImages(vd);
      */
      //await _bioFaceServiceManager.FaceClient.EnrollFace(imageList);

      /*
      Google.Protobuf.ByteString bs = Google.Protobuf.ByteString.CopyFrom(bytes);

      bytes = bs.ToByteArray();
      LoadImage(bytes);
      */
      //try
      //{
      //await _bioFaceServiceManager.FaceClient.DetectFace(bytes);
      /*
      int i = 0;
      System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
      watch.Start();
      while (watch.ElapsedMilliseconds < 1000)
      {
        await _bioFaceServiceManager.FaceClient.(bytes);


         i++;
      }
      Console.Write(i + " " + watch.ElapsedMilliseconds);
      watch.Stop();
      */
      /*
        //CommandInformation commandInformation = new CommandInformation() { PacketSize = bytes.Length + 2 * sizeof(int), CommandID = 0 };


        CommandInformation mediaInformation = new CommandInformation() { PacketSize = bytes.Length };


        int i = 0;
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
       // while (watch.ElapsedMilliseconds < 1000)
       // {
         _client.Serialize(mediaInformation);

          Thread.Sleep(1);
         _client.Write(bytes, bytes.Length);

        //  i++;
      //  }
      //  Console.Write(i + " " + watch.ElapsedMilliseconds);
        watch.Stop();


       // Thread.Sleep(8000);

        //Thread.Sleep(8000);

        //_client.Close();
        */
      /*
            }
            catch (Exception ex)
            {
              MessageBox.Show(ex.Message);
            }
          }
      */
    }
  }
}
