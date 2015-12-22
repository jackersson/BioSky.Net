using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using BioContracts;
using System.Drawing;

using System.Windows.Media.Imaging;

using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using MahApps.Metro.Controls;
using BioModule.ResourcesLoader;


namespace BioShell.ViewModels
{
  public class BioShellViewModel : PropertyChangedBase
  {

    public BioShellViewModel()
    {
      _tabControl       = new ShellTabControl      ();
      _flyoutControl    = new ShellFlyoutControl   ();
      _toolBar          = new ShellToolBar         ();
      _mainMenu         = new ShellMainMenu        ();
      _loginInformation = new ShellLoginInformation();
    }

    private ShellLoginInformation _loginInformation;
    public ShellLoginInformation LoginInformation
    {
      get { return _loginInformation; }
    }

    private ShellMainMenu _mainMenu;
    public ShellMainMenu MainMenu
    {
      get { return _mainMenu; }
    }

    private ShellToolBar _toolBar;
    public ShellToolBar ToolBar
    {
      get { return _toolBar; }  
    }

    private ShellTabControl    _tabControl;  
    public ShellTabControl TabControl
    {
      get { return _tabControl; }          
    }

    private ShellFlyoutControl _flyoutControl;
    public ShellFlyoutControl FlyoutControl
    {
      get  { return _flyoutControl; }
    }
 
    public object CurrentTabControl
    {
      get { return _tabControl.ScreenViewModel;  }     
    }

    public object CurrentToolBar
    {
      get { return _toolBar.ScreenViewModel; }
    }

    public object CurrentFlyoutControl
    {
      get { return _flyoutControl.ScreenViewModel; }
    }

    public object CurrentMainMenu
    {
      get { return _mainMenu.ScreenViewModel; }
    }

    public object CurrentLoginInformation
    {
      get { return _loginInformation.ScreenViewModel; }
    }

    public BitmapSource LogoIconSource
    {
      get { return ResourceLoader.LogoIconSource; }
    }
  }
}
