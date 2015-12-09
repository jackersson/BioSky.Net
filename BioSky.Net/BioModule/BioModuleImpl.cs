using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using BioContracts;
using BioModule.ViewModels;
using BioModule.Model;

namespace BioModule
{
  class BioModuleImpl : IBioModule
  {
    private readonly IBioShell   _shell     ;
    private readonly TabViewModel          _tabControlViewModel     ;   
    private readonly UsersViewModel        _usersViewModel          ;
    private readonly VisitorsViewModel     _visitorsViewModel       ;
    private readonly SettingsViewModel     _settingsViewModel       ;
    private readonly UserPageViewModel     _userPageViewModel       ;
    private readonly TrackControlViewModel _trackingControlViewModel;

    private readonly IBioEngine _bioEngine;

    public BioModuleImpl( IBioShell shell
                        , TabViewModel tabControlViewModel
                        , UsersViewModel usersViewModel
                        , VisitorsViewModel visitorsViewModel
                        , SettingsViewModel settingsViewModel
                        , UserPageViewModel userPageViewModel
                        , TrackControlViewModel trackingControlViewModel
                        , IBioEngine bioEngine )
    {
      _shell = shell;     
      _tabControlViewModel      = tabControlViewModel;
      _usersViewModel           = usersViewModel;
      _visitorsViewModel        = visitorsViewModel;
      _settingsViewModel        = settingsViewModel;
      _userPageViewModel        = userPageViewModel;
      _trackingControlViewModel = trackingControlViewModel;

      _bioEngine = bioEngine;

    }

    public void Init()
    {

      //_usersViewModel.Init(_bioEngine);
      //_trackingControlViewModel.TrackControl.TrackItems.Add()
      _shell.TabControl.TabPages.Add(new ShellTabPage() { Caption = "Tracking"    , ScreenViewModel = _trackingControlViewModel });
      _shell.TabControl.TabPages.Add(new ShellTabPage() { Caption = "Users"       , ScreenViewModel = _usersViewModel });
      _shell.TabControl.TabPages.Add(new ShellTabPage() { Caption = "Visitors"    , ScreenViewModel = _visitorsViewModel });
      _shell.TabControl.TabPages.Add(new ShellTabPage() { Caption = "Add New User", ScreenViewModel = _userPageViewModel });

      _tabControlViewModel.update(_shell.TabControl/*, _usersViewModel*/);

      _shell.TabControl.ScreenViewModel = _tabControlViewModel;

      _shell.FlyoutControl.FlyoutPages.Add(new ShellFlyoutPage() { Caption = "Settings", ScreenViewModel = _settingsViewModel });
    }
  }
}
