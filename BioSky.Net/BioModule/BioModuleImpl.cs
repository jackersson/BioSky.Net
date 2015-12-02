using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

using BioContracts;
using BioModule.ViewModels;

namespace BioModule
{
  class BioModuleImpl : IBioModule
  {
    private readonly IBioShell   _shell     ;
    private readonly TabViewModel      _tabControlViewModel;   
    private readonly UsersViewModel    _usersViewModel     ;
    private readonly VisitorsViewModel _visitorsViewModel  ;


    public BioModuleImpl( IBioShell shell
                        , TabViewModel tabControlViewModel
                        , UsersViewModel usersViewModel
                        , VisitorsViewModel visitorsViewModel )
    {
      _shell = shell;     
      _tabControlViewModel = tabControlViewModel;
      _usersViewModel      = usersViewModel;
      _visitorsViewModel   = visitorsViewModel;
    }

    public void Init()
    {
      _shell.TabControl.TabPages.Add(new ShellTabPage() { Caption = "Users"   , ScreenViewModel = _usersViewModel });
      _shell.TabControl.TabPages.Add(new ShellTabPage() { Caption = "Visitors", ScreenViewModel = _visitorsViewModel });

      _tabControlViewModel.update(_shell.TabControl);

      _shell.TabControl.ScreenViewModel = _tabControlViewModel;    
    }
  }
}
