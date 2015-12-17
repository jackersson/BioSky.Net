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

    }

    public void OpenTab( Type tabType, object[] args )
    {
      ShellTabPage newTabPage = new ShellTabPage() { ScreenViewModel = _container.Resolve(tabType) };
      
      InvokeMethod(tabType, "Update" , newTabPage.ScreenViewModel, args);

      string caption = (string)InvokeMethod(tabType, "Caption", newTabPage.ScreenViewModel );

      if (caption != null)
        newTabPage.Caption = caption;

      _tabControl.TabPages.Add(newTabPage);

      SelectedTabPage = newTabPage;
    }

    private object InvokeMethod ( Type objectType, string methodName, object source, object[] args = null )
    {
      MethodInfo method = objectType.GetMethod(methodName);
      if (method != null)
        return method.Invoke(source, args);
      return null;
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
