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
      _viewModelSelector.OpenTab(ViewModelsID.LocationSettings);
    }

    public void OpenTabVisitors()
    {
      _viewModelSelector.OpenTab(ViewModelsID.VisitorsPage);
    }

    private ViewModelSelector _viewModelSelector;
  }
}
