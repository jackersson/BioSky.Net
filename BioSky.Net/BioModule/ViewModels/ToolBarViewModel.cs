using Caliburn.Micro;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class ToolBarViewModel : PropertyChangedBase
  {
    public ToolBarViewModel( ViewModelSelector viewModelSelector)
    {
      _viewModelSelector = viewModelSelector;
    }

    public void OpenTabAddNewPerson()
    {
      _viewModelSelector.ShowContent( ShowableContentControl.TabControlContent
                                    , ViewModelsID.UserPage
                                    , new object[] { null });
    }

    public void OpenTabAddNewLocation()
    {
      _viewModelSelector.ShowContent( ShowableContentControl.FlyoutControlContent
                                    , ViewModelsID.LocationSettings
                                    , new object[] { null });
    }

    public void OpenTabVisitors()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.VisitorsPage);
    }

    public void OpenTabUsers()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.UsersPage);
    }
    public void OpenTabTrack()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.TrackPage);
    }

    private readonly ViewModelSelector _viewModelSelector;
  }
}