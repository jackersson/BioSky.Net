using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Collections.ObjectModel;
using BioContracts;
using System.Windows;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Reflection;

namespace BioModule.ViewModels
{
  public class TabViewModel : PropertyChangedBase, ITabControl
  {
       
    public TabViewModel( IWindsorContainer container, IBioShell shell)
    {
      _container  = container;
      _tabControl = shell.TabControl;
      
      TabPages    = _tabControl.TabPages;
    }

    public void Init()
    {
      
      _tabControl.TabPages.Add(new ShellTabPage() { Caption = "Tracking"
                                                  , ScreenViewModel = _container.Resolve<TrackControlViewModel>()
                                                  , CanClose = false });
                                                  
      _tabControl.TabPages.Add(new ShellTabPage() { Caption = "Users"       
                                                  , ScreenViewModel = _container.Resolve<UsersViewModel>()
                                                  , CanClose = true});

      _tabControl.TabPages.Add(new ShellTabPage() { Caption = "Visitors"    
                                                  , ScreenViewModel = _container.Resolve<VisitorsViewModel>()
                                                  , CanClose = true });

      _tabControl.TabPages.Add(new ShellTabPage() { Caption = "Add New User"
                                                  , ScreenViewModel = _container.Resolve<UserPageViewModel>()
                                                  , CanClose = true });
    }

    public void OpenTab( Type tabType )
    {      
      _tabControl.TabPages.Add(new ShellTabPage()
      {
          Caption = "New Tab"
        , ScreenViewModel = _container.Resolve(tabType)                                               
        , CanClose = true
      });      
    }


    private ObservableCollection<ShellTabPage> _tabPages;
    public ObservableCollection<ShellTabPage> TabPages
    {
      get { return _tabPages; }
      private set
      {
        if (_tabPages!=value)
        {
          _tabPages = value;
          NotifyOfPropertyChange(() => TabPages);
        }        
      }
    }
      
    private ShellTabPage _selectedTabPage;
    public ShellTabPage SelectedTabPage
    {
      get { return _selectedTabPage; }
      set
      {
        if (_selectedTabPage == value)
          return;
        _selectedTabPage = value;
        NotifyOfPropertyChange(() => SelectedTabPage);
        NotifyOfPropertyChange(() => CurrentViewTab);        
      }
    }
       
    public object CurrentViewTab
    {
      get { return _selectedTabPage == null ? null : _selectedTabPage.ScreenViewModel; }    
    }

    private ShellTabControl   _tabControl; 
    
    public ShellTabControl TabControl
    {
      get { return _tabControl; }  
    }
    
    private IWindsorContainer _container ;
  }
}
