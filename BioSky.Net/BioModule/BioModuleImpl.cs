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

namespace BioModule
{
  class BioModuleImpl : IBioModule
  {
    private readonly IBioShell   _shell     ;
    private readonly ITabControl _tabControlViewModel     ;
    private readonly IFlyoutControl _flyoutControlViewModel  ;
    private readonly ViewModelSelector _viewModelSelector;
    //private readonly UsersViewModel        _usersViewModel          ;
    //private readonly VisitorsViewModel     _visitorsViewModel       ;
    //private readonly SettingsViewModel     _settingsViewModel       ;
    //private readonly UserPageViewModel     _userPageViewModel       ;
    //private readonly TrackControlViewModel _trackingControlViewModel;

    private readonly IBioEngine _bioEngine;

    public BioModuleImpl( IBioShell shell                   
                        , IBioEngine bioEngine
                        , ITabControl tabControlViewModel
                        , IFlyoutControl flyoutControlViewModel
                        , ViewModelSelector viewModelSelector )
    {
      _shell = shell;
      _flyoutControlViewModel = flyoutControlViewModel;
      _viewModelSelector = viewModelSelector;
      /*
      _tabControlViewModel      = tabControlViewModel;
      _usersViewModel           = usersViewModel;
      _visitorsViewModel        = visitorsViewModel;
      _settingsViewModel        = settingsViewModel;
      _userPageViewModel        = userPageViewModel;
      _trackingControlViewModel = trackingControlViewModel;
      */
      _bioEngine = bioEngine;

      _tabControlViewModel = tabControlViewModel;

    }

    public void Init()
    {
      _tabControlViewModel.Init();
      _shell.TabControl.ScreenViewModel    = _tabControlViewModel;
      _shell.FlyoutControl.ScreenViewModel = _flyoutControlViewModel;

      _shell.ToolBar.ScreenViewModel       = new ToolBarViewModel(_viewModelSelector);
      _shell.MainMenu.ScreenViewModel      = new MainMenuViewModel(_viewModelSelector);      
    }
  }
}
