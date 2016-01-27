using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioShell.ViewModels;
using BioContracts;
using Grpc.Core;

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

      try
      {
        Channel _clientChannel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
      
    }    
    public object TabControl
    {
      get { return _shellViewModel.TabControl;  }
      set
      {        
         _shellViewModel.TabControl = value;        
      }
    }

    public object FlyoutControl
    {
      get { return _shellViewModel.FlyoutControl; }
      set
      {
        _shellViewModel.FlyoutControl = value;
      }
    }   
    
    public object ToolBar
    {
      get { return _shellViewModel.ToolBar;  }
      set
      {
        _shellViewModel.ToolBar = value;
      }
    }

    public object MainMenu
    {
      get { return _shellViewModel.MainMenu; }
      set
      {
        _shellViewModel.MainMenu = value;
      }
    }
    

    public IBioModule LoadModule(Assembly assembly)
    {
      return _loader.LoadModule(assembly);
    }
  }
}
