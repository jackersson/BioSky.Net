using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioShell.ViewModels;
using BioContracts;

namespace BioShell
{
  class BioShellImpl : IBioShell
  {
    private readonly BioModuleLoader _loader;
    private readonly BioShellViewModel _shellViewModel;

    public BioShellImpl(BioModuleLoader loader, BioShellViewModel shellViewModel)
    {
      _loader = loader;
      _shellViewModel = shellViewModel;
    }    
    public ShellTabControl TabControl       { get { return _shellViewModel.TabControl;  } }

    public ShellFlyoutControl FlyoutControl { get { return _shellViewModel.FlyoutControl; } }   
    
    public ShellToolBar ToolBar   { get { return _shellViewModel.ToolBar;  }  }

    public ShellMainMenu MainMenu { get { return _shellViewModel.MainMenu; }  }

    public IBioModule LoadModule(Assembly assembly)
    {
      return _loader.LoadModule(assembly);
    }
  }
}
