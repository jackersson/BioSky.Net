using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;
using System.Windows;
using System;
using System.Windows.Interop;

namespace BioShell.ViewModels
{
  public class BioShellViewModel : Screen
  {
    public BioShellViewModel()
    {
      DisplayName = "BioSkyNet";     
    }  

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

    private object _progressRing;
    public object ProgressRing
    {
      get { return _progressRing; }
      set
      {
        if (_progressRing != value)
        {
          _progressRing = value;
          NotifyOfPropertyChange(() => ProgressRing);
        }
      }
    }

    public BitmapSource LogoIconSource
    {
      get { return ResourceLoader.LogoIconSource; }
    }

  }
}
