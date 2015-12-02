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
    private ShellTabControl    _tabControl;
    private ShellFlyoutControl _flyouts;

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
      get { return _flyouts.FlyoutPages; }      
    }

    public void AddTabPage()
    {
      NotifyOfPropertyChange(() => CurrentTabControl);
    }
    public object CurrentTabControl
    {
      get { return _tabControl.ScreenViewModel;  }     
    }

    public object CurrentFlyout
    {
      get { return _flyouts.FlyoutPages[0].ScreenViewModel; }
    }
   
    public BioShellViewModel()
    {
      _tabControl = new ShellTabControl();
      _flyouts    = new ShellFlyoutControl();

      //FlyoutControl.FlyoutPages.Add(new ShellFlyoutPage { Caption = "Test" });
      //_flyouts.Items.Add( new Flyout() )
    }

    public void ShowSettings()
    {
      NotifyOfPropertyChange(() => FlyoutPages);
      NotifyOfPropertyChange(() => CurrentFlyout);


      //this.ToggleFlyout(0);
    }

    private void ToggleFlyout(int index)
    {
      /*var flyout = this.FlyoutControl.Items[index] as Flyout;
      if (flyout == null)
      {
          return;
      }

      flyout.IsOpen = !flyout.IsOpen;*/
    }

  

  }
}
