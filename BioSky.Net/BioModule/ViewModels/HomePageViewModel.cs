using BioModule.Utils;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.ViewModels
{
  public class HomePageViewModel : Screen
  {
    public HomePageViewModel(ViewModelSelector viewModelSelector)
    {
      DisplayName = "HomePage";
      _viewModelSelector = viewModelSelector;
    }

    public void OnAddNewUser()
    {
      _viewModelSelector.ShowContent( ShowableContentControl.TabControlContent
                                    , ViewModelsID.UserPage
                                    , new object[] { null });
    }

    public void OnAddNewLocation()
    {
      _viewModelSelector.ShowContent( ShowableContentControl.FlyoutControlContent
                                    , ViewModelsID.LocationSettings
                                    , new object[] { null });
    }

    public void OnSettings()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.FlyoutControlContent, ViewModelsID.GeneralSettings);
    }

    public void OnTracking()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.TrackPage);
    }
    public void OnUsers()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.UsersPage);
    }
    public void OnVisitors()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.VisitorsPage);
    }

    private readonly ViewModelSelector _viewModelSelector;
  }
}
