using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using Castle.Windsor;

namespace BioModule.ViewModels
{
  public class FlyoutControlViewModel : PropertyChangedBase, IFlyoutControl
  {

    public FlyoutControlViewModel(IWindsorContainer container, IBioShell shell)
    {
      _container     = container;
      _flyoutControl = shell.FlyoutControl;

      _flyoutControl.FlyoutPages.Add(new ShellFlyoutPage()
      {
          Caption = "Settings"
        , ScreenViewModel = _container.Resolve<SettingsViewModel>()
      });

      FlyoutOpenState = false;
    }


    public void ShowPage(Type flyoutPage)
    {  
      CurrentFlyoutPage = _flyoutControl.Find(flyoutPage);
      FlyoutOpenState = true;
    }

    public ObservableCollection<ShellFlyoutPage> FlyoutPages
    {
      get { return _flyoutControl.FlyoutPages; }
    }

    private bool _flyoutOpenState;
    public bool FlyoutOpenState
    {
      get { return _flyoutOpenState; }
      set
      {
        if (_flyoutOpenState == value)
          return;

        _flyoutOpenState = value;
        NotifyOfPropertyChange(() => FlyoutOpenState);
      }
    }

    private ShellFlyoutPage _currentFlyoutPage;
    public ShellFlyoutPage CurrentFlyoutPage
    {
      get { return _currentFlyoutPage; }
      set
      {
        if (_currentFlyoutPage == value)
          return;

        _currentFlyoutPage = value;
        NotifyOfPropertyChange(() => CurrentFlyoutPage);
      }
    }

    private ShellFlyoutControl _flyoutControl;
    public ShellFlyoutControl FlyoutControl
    {
      get { return _flyoutControl; }
      set
      {
        if (_flyoutControl == value)
          return;

        _flyoutControl = value;
        NotifyOfPropertyChange(() => FlyoutControl);
        NotifyOfPropertyChange(() => FlyoutPages  );
      }
    }

    private IWindsorContainer _container;
  }
}
