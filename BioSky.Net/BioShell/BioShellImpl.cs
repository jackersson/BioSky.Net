using System.Reflection;

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

    public object TabControl
    {
      get { return _shellViewModel.TabControl;  }
      set {        
         _shellViewModel.TabControl = value;        
      }
    }

    public object FlyoutControl
    {
      get { return _shellViewModel.FlyoutControl; }
      set {
        _shellViewModel.FlyoutControl = value;
      }
    }   
    
    public object ToolBar
    {
      get { return _shellViewModel.ToolBar;  }
      set {
        _shellViewModel.ToolBar = value;
      }
    }

    public object MainMenu
    {
      get { return _shellViewModel.MainMenu; }
      set {
        _shellViewModel.MainMenu = value;
      }
    }
    public object ProgressRing
    {
      get { return _shellViewModel.ProgressRing; }
      set {
        _shellViewModel.ProgressRing = value;
      }
    }
    public object LoginInformation
    {
      get { return _shellViewModel.LoginInformation; }
      set {
        _shellViewModel.LoginInformation = value;
      }
    }

    public IBioModule LoadModule(Assembly assembly) {
      return _loader.LoadModule(assembly);
    }
  }
}
