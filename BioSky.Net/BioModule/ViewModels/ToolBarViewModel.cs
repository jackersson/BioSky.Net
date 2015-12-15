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
    public ToolBarViewModel( ViewModelSelector viewModelSelector /*ITabControl tabControl, IFlyoutControl flyoutControl */)
    {
      _viewModelSelector = viewModelSelector;
      //_tabControl    = tabControl   ;
      //_flyoutControl = flyoutControl;
      //Console.Write("Here");
    }

    public void OpenTabAddNewPerson()
    {
      _viewModelSelector.OpenTab(ViewModelsID.UserPage);
    }

    public void OpenTabAddNewLocation()
    {
      _viewModelSelector.OpenTab(ViewModelsID.LocationSettings);
    }

    public void OpenTabVisitors()
    {
      _viewModelSelector.OpenTab(ViewModelsID.VisitorsPage);
    }

    public void OpenTabUsers()
    {
      _viewModelSelector.OpenTab(ViewModelsID.UsersPage);
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

    //private ITabControl    _tabControl   ;
    // private IFlyoutControl _flyoutControl;
    private readonly ViewModelSelector _viewModelSelector;
  }
}
