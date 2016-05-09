using Caliburn.Micro;
using System.Windows.Media.Imaging;
using System.Windows;
using System;
using System.Windows.Interop;
using BioContracts;
using BioAccessDevice;
using System.Drawing;

namespace BioShell.ViewModels
{
  public class BioShellViewModel : Screen
  {
    public BioShellViewModel()
    {
      DisplayName = "BioSkyNet";

     // IAccessDeviceEngine ad = new AccessDevicesEngine();
     // ad.Add("COM3");

      //Teset = new TestViewModel();
      //ad.Subscribe(Teset, "COM3");
    }

    /*
    private TestViewModel _test;
    public TestViewModel Teset
    {
      get { return _test; }
      set
      {
        if (_test != value)
        {
          _test = value;
          NotifyOfPropertyChange(() => Teset);
        }
      }
    }
    */

    private object _mainMenu;
    public object MainMenu
    {
      get { return _mainMenu; }
      set
      {
        if (_mainMenu != value)
        {
          _mainMenu = value;
          NotifyOfPropertyChange(() => MainMenu);
        }
      }
    }

    private object _toolBar;
    public object ToolBar
    {
      get { return _toolBar; }
      set
      {
        if (_toolBar != value)
        {
          _toolBar = value;
          NotifyOfPropertyChange(() => ToolBar);
        }
      }
    }

    private object _tabControl;  
    public object TabControl
    {
      get { return _tabControl; }
      set
      {
        if (_tabControl != value)
        {
          _tabControl = value;
          NotifyOfPropertyChange(() => TabControl);
        }
      }
    }
       
    private object _flyoutControl;
    public object FlyoutControl
    {
      get  { return _flyoutControl; }
      set
      {
        if (_flyoutControl != value)
        {
          _flyoutControl = value;
          NotifyOfPropertyChange(() => FlyoutControl);
        }
      }
    }   

    private object _loginInformation;
    public object LoginInformation
    {
      get { return _loginInformation; }
      set
      {
        if (_loginInformation != value)
        {
          _loginInformation = value;
          NotifyOfPropertyChange(() => LoginInformation);
        }
      }
    }

    private static BitmapSource _logoIconSource;
    public static BitmapSource LogoIconSource
    {
      get
      {
        if (_logoIconSource == null)
          _logoIconSource = BitmapToBitmapSource((Properties.Resources.logo).ToBitmap());
        return _logoIconSource;
      }
    }

    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);
    public static BitmapSource BitmapToBitmapSource(Bitmap source)
    {
      if (source == null)
        return null;
      BitmapSource bmp = null;

      try
      {
        IntPtr hBitMap = source.GetHbitmap();
        bmp = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitMap
                                                                                       , IntPtr.Zero
                                                                                       , Int32Rect.Empty
                                                                                       , BitmapSizeOptions.FromEmptyOptions());
        DeleteObject(hBitMap);

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
      return bmp;
    }    
  }
}
