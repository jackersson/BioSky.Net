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

namespace BioShell.ViewModels
{

  public class BioShellViewModel : PropertyChangedBase
  {
    private ShellTabControl _tabControl;
    public ShellTabControl TabControl
    {
      get { return _tabControl; }          
    }

    public void AddTabPage()
    {
      NotifyOfPropertyChange(() => CurrentTabControl);
    }
    public object CurrentTabControl
    {
      get { return _tabControl.ScreenViewModel;  }     
    }
   
    public BioShellViewModel()
    {
      _tabControl = new ShellTabControl();     
    }

  

  }
}
