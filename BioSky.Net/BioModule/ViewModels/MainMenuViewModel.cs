using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class MainMenuViewModel : PropertyChangedBase
  {

    public MainMenuViewModel(ViewModelSelector viewModelSelector)
    {
      _viewModelSelector = viewModelSelector;     
    }

    public void OpenTabAddNewPerson()
    {
      _viewModelSelector.OpenTab(ViewModelsID.UserPage);
    }

    public void OpenTabAddNewLocation()
    {
      _viewModelSelector.ShowFlyout(ViewModelsID.LocationSettings);
    }

    public void OpenTabVisitors()
    {
      _viewModelSelector.OpenTab(ViewModelsID.VisitorsPage);
    }

    public void OpenFlayoutSettings()
    {
      _viewModelSelector.ShowFlyout(ViewModelsID.LocationSettings);
    }

    public void OpenTabUsers()
    {
      _viewModelSelector.OpenTab(ViewModelsID.UsersPage);
    }

    public void OpenTabTrack()
    {
      _viewModelSelector.OpenTab(ViewModelsID.TrackPage);
    }

    private ViewModelSelector _viewModelSelector;
  }
}
