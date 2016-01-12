using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using AForge.Video.DirectShow;
using AForge.Video;
using AForge.Controls;

using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using BioUITest.Utils;
using ProtoBuf;
using BioContracts;

namespace BioUITest.ViewModels
{

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

    private BioTcpClient _client;
    
    public VideoStreamViewModel()
    {
      DevicesNames = new FilterInfoCollection(FilterCategory.VideoInputDevice);

      _client = new BioTcpClient("127.0.0.1", 13);
      _client.Start();

      _faces = new AsyncObservableCollection<FaceInformation>();
      _faces.Add(new FaceInformation() { XPos = 100, YPos = 100, Width = 100, Height = 100 });
      _faces.Add(new FaceInformation() { XPos = 0, YPos = 0, Width = 100, Height = 100 });
      Faces = _faces;
    }

    private FilterInfoCollection _devicesNames;
    public FilterInfoCollection DevicesNames
    {
      get { return _devicesNames;  }
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
        if ( _newFrame != value)
        {
          _newFrame = value;
          NotifyOfPropertyChange(()=> NewFrame);
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
        Image.FromStream(mem).Save("F:\\C#\\BioSkyNetSuccess\\BioSky.Net\\BioSky.Net\\BioUITest\\2.jpg");
       
      }
      
      return image;
    }




    private void OpenVideoSource()
    {      
      videoSource = new VideoCaptureDevice(SelectedDevice.MonikerString);
      videoSource.Start();
      videoSource.NewFrame += VideoSource_NewFrame;
    }

    private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
    {
      Bitmap newFrame = (Bitmap)eventArgs.Frame.Clone();
      using (Graphics g = Graphics.FromImage(newFrame))
      {
        g.DrawRectangle(new Pen(Color.Green, 7), 0, 0, 100, 100);
      }
      if (newFrame != null)
      {
        IntPtr hBitMap = newFrame.GetHbitmap();
        UpdateImage(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitMap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
      }
    }

    public void CloseSocket()
    {
      _client.Close();
    }

    private AsyncObservableCollection<FaceInformation> _faces;
    public AsyncObservableCollection<FaceInformation> Faces
    {
      get { return _faces; }
      set
      {
        if ( _faces != value )
        {
          _faces = value;
          NotifyOfPropertyChange(() => Faces);
        }
      }
    }


    public void SendImage()
    {
      Image newFrame = Bitmap.FromFile("F:\\C#\\BioSkyNetSuccess\\BioSky.Net\\BioSky.Net\\BioUITest\\1.jpg");
      byte[] bytes = ImageToByte2(newFrame);
     
    
      try
      {     

      
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

      }
      catch ( Exception ex )
      {
        MessageBox.Show(ex.Message);
      }
    }

    
  }
}
