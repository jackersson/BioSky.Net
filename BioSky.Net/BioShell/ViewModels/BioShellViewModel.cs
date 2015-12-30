using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

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

    public BitmapSource LogoIconSource
    {
      get { return ResourceLoader.LogoIconSource; }
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
  }
}
