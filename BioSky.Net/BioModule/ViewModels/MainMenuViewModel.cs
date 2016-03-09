using System;

using Caliburn.Micro;
using BioModule.Utils;
using BioContracts;


namespace BioModule.ViewModels
{
  public class MainMenuViewModel : Screen
  {
    public MainMenuViewModel( IProcessorLocator locator)
    {
      _locator = locator;

      _viewModelSelector = _locator.GetProcessor<ViewModelSelector>();
      _dialogsHolder     = _locator.GetProcessor<DialogsHolder>();

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
       _dialogsHolder.AboutDialog.Show();
       var result = _dialogsHolder.AboutDialog.GetDialogResult();  
    }
    public void ShowLogInDialog()
    {
      _dialogsHolder.LoginDialog.Show();
      LoginDialogResult result = _dialogsHolder.LoginDialog.GetDialogResult();
      if(result == null)
        return;

      switch(result.status)
      {
        case LoginDialogStatus.Register:
          Console.WriteLine("Register " + result.userName + " " + result.password);
          break;
                    
        case LoginDialogStatus.SignIn:
          Console.WriteLine("Register " + result.userName + " " + result.password);
          break;
      }

    }

    public void ShowSettingsFlayout()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.FlyoutControlContent
                           , ViewModelsID.GeneralSettings
                           , new object[] {  });    }



    private ViewModelSelector          _viewModelSelector;
    private readonly DialogsHolder     _dialogsHolder    ;
    private readonly IProcessorLocator _locator          ;

  }
}
