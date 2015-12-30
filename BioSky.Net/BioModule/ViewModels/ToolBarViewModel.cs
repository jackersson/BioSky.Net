using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using BioContracts;
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
      _viewModelSelector.ShowContent( ShowableContentControl.TabControlContent,  ViewModelsID.UserPage, new object[] { null });
    }

    public void OpenTabAddNewLocation()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.FlyoutControlContent,  ViewModelsID.LocationSettings);
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

    public BitmapSource AddPersonIconSource
    {
      get { return ResourceLoader.AddUserIconSource; }
    }

    public BitmapSource AddLocationIconSource
    {
      get { return ResourceLoader.AddLocationIconSource; }
    }

    public BitmapSource JournalListIconSource
    {
      get { return ResourceLoader.JournalListIconSource; }
    }

    public BitmapSource UsersListIconSource
    {
      get { return ResourceLoader.UsersListIconSource; }
    }

    public BitmapSource TrackingIconSource
    {
      get { return ResourceLoader.TrackingIconSource; }
    }

    private readonly ViewModelSelector _viewModelSelector;
  }
}
