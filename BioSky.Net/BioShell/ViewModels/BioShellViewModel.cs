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

namespace BioShell.ViewModels
{
  public class BioShellViewModel : PropertyChangedBase
  {

    public BioShellViewModel()
    {
      _tabControl   = new ShellTabControl   ();
      _flyouts      = new ShellFlyoutControl();
      //_trackControl = new ShellTrackControl ();

      IsSettingsOpen = true;
    }


    private ShellTabControl    _tabControl;
    private ShellFlyoutControl _flyouts   ;
   // private ShellTrackControl  _trackControl;

    public ShellTabControl TabControl
    {
      get { return _tabControl; }          
    }

  

    public ShellFlyoutControl FlyoutControl
    {
      get
      {
        return _flyouts;
      }
      set
      {
        if ( _flyouts == value )
          return;

        _flyouts = value;
        NotifyOfPropertyChange(() => FlyoutControl);
      }
    }

    public ObservableCollection<ShellFlyoutPage> FlyoutPages
    {
      get
      {        
        return _flyouts.FlyoutPages;
      }      
    }
   
    public object CurrentTabControl
    {
      get { return _tabControl.ScreenViewModel;  }     
    }

    public object CurrentFlyout
    {
      get { return _flyouts.FlyoutPages[0].ScreenViewModel; }
    }

    public object CurrentFlyoutCaption
    {
      get { return _flyouts.FlyoutPages[0].Caption; }
    }


    private bool _isSettingOpen;
    public bool IsSettingsOpen
    {
      get { return _isSettingOpen; }
      set
      {
        _isSettingOpen = value;
        NotifyOfPropertyChange(() => IsSettingsOpen);
      }
    }

    public void ShowSettings()
    {      
      IsSettingsOpen = !IsSettingsOpen;      
    }


  

  }
}
