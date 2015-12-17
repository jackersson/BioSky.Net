using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;

namespace BioModule.Utils
{

  public enum ViewModelsID
  {
      UserPage = 0
    , LocationSettings
    , VisitorsPage
    , UsersPage
    , TrackPage
  }

  public class ViewModelSelector
  {
    public ViewModelSelector( ITabControl tabControl, IFlyoutControl flyoutControl )
    {
      _tabControl    = tabControl   ;
      _flyoutControl = flyoutControl;

      _viewModels = new Dictionary<ViewModelsID, Type>();

      _viewModels.Add(ViewModelsID.UserPage         , Type.GetType("BioModule.ViewModels.UserPageViewModel"    ));
      _viewModels.Add(ViewModelsID.LocationSettings , Type.GetType("BioModule.ViewModels.SettingsViewModel"    ));
      _viewModels.Add(ViewModelsID.VisitorsPage     , Type.GetType("BioModule.ViewModels.VisitorsViewModel"    ));
      _viewModels.Add(ViewModelsID.UsersPage        , Type.GetType("BioModule.ViewModels.UsersViewModel"       ));
      _viewModels.Add(ViewModelsID.TrackPage        , Type.GetType("BioModule.ViewModels.TrackControlViewModel"));

    }

    public void OpenTab(ViewModelsID pageID )
    {
      Type pageType;
      bool flag = _viewModels.TryGetValue(pageID, out pageType);
      if (flag)
        _tabControl.OpenTab(pageType);
    }

    public void ShowFlyout(ViewModelsID pageID)
    {
      Type pageType;
      bool flag = _viewModels.TryGetValue(pageID, out pageType);
      if (flag)
        _flyoutControl.ShowPage(pageType);
    }
      
    private Dictionary<ViewModelsID, Type> _viewModels;

    private ITabControl    _tabControl   ;
    private IFlyoutControl _flyoutControl;
  }
}
