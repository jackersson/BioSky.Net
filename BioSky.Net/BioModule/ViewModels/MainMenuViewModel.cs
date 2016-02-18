using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.Utils;

using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using BioContracts;


namespace BioModule.ViewModels
{
  public class MainMenuViewModel : Screen
  {
    public MainMenuViewModel( IProcessorLocator locator)
    {   
      _viewModelSelector = locator.GetProcessor<ViewModelSelector>();
      _windowManager     = locator.GetProcessor<IWindowManager>();
    }

    public void OpenTabAddNewPerson()
    {
      _viewModelSelector.ShowContent( ShowableContentControl.TabControlContent,  ViewModelsID.UserPage);
    }

    public void OpenTabAddNewLocation()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.FlyoutControlContent, ViewModelsID.LocationSettings);
    }

    public void OpenTabVisitors()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.VisitorsPage);
    }


       

    public void UpdateUserPassword(bool register, string name, System.Security.SecureString password)
    {
      Console.WriteLine(register + " " + name + " " + password);

    } 
    public void ShowAboutDialog()
    {
      string s = "ewfrnfglnsgnejgnbjetntnbkjtnet/n dsgjwnewhuhekbnkenbetknbknjdfjsbxmjnf/n wdgbscfvrjenbvfvjrenenvlwefn/n";
     // _windowManager.ShowDialog(new AboutDialogViewModel());    
       _windowManager.ShowDialog(new CustomTextDialogViewModel("Test Dialog", s, DialogStatus.Info));      
  
    }
    public void ShowLogInDialog()
    {      
      var result = _windowManager.ShowDialog(new LoginDialogViewModel(this));      
    }

    public void ShowSettingsFlayout()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.FlyoutControlContent
                           , ViewModelsID.GeneralSettings
                           , new object[] {  });    }



    private ViewModelSelector          _viewModelSelector;
    private readonly IWindowManager    _windowManager;
  }
}
