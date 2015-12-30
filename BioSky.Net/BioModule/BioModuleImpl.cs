using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using BioContracts;
using BioModule.ViewModels;
using BioModule.Model;
using BioModule.Utils;
using System.Globalization;
using System.Threading;
using BioModule.ResourcesLoader;

namespace BioModule
{
  class BioModuleImpl : IBioModule
  {
    private readonly IBioShell              _shell                 ;
    private readonly TabViewModel           _tabControlViewModel   ;
    private readonly FlyoutControlViewModel _flyoutControlViewModel;
    private readonly ViewModelSelector      _viewModelSelector     ;

    private readonly IBioEngine _bioEngine;

    public BioModuleImpl( IBioShell shell                   
                        , IBioEngine bioEngine
                        , TabViewModel tabControlViewModel
                        , FlyoutControlViewModel flyoutControlViewModel
                        , ViewModelSelector viewModelSelector )
    {
      _shell = shell;
      _bioEngine = bioEngine;

      _flyoutControlViewModel = flyoutControlViewModel;
      _viewModelSelector      = viewModelSelector;      

      _tabControlViewModel = tabControlViewModel;
    }

    public void Init()
    {      
      _viewModelSelector.ShowContent( ShowableContentControl.TabControlContent,  ViewModelsID.TrackPage);

      _tabControlViewModel.Init();
      _shell.TabControl    = _tabControlViewModel;
      _shell.FlyoutControl = _flyoutControlViewModel;

      _shell.ToolBar       = new ToolBarViewModel(_viewModelSelector);
      _shell.MainMenu      = new MainMenuViewModel(_viewModelSelector);      
    }
  }
}
