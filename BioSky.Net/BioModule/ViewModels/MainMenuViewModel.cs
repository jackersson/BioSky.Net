using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.Utils;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using BioModule.ViewModels.Dialogs;




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

    public async void OpenDialogAbout()
    {
      
      var dialog = new CustomDialog();
      dialog.DialogSettings.ColorScheme = MetroDialogColorScheme.Accented;
      dialog.ShowDialogExternally();
     // LoginDialogData result = await this.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, InitialUsername = "MahApps", EnablePasswordPreview = true });
    }


    private VerificationDialogViewModel _verificationWindow;
    private CustomDialog _customDialog;
    public async void TestDialog()
    {
      var metroWindow = (Application.Current.MainWindow as MetroWindow);
      _customDialog = new CustomDialog();
      _customDialog.DialogSettings.ColorScheme = MetroDialogColorScheme.Accented;
      
      var mySettings = new MetroDialogSettings()
      {
        AffirmativeButtonText = "Yes",
        AnimateShow = true,
        NegativeButtonText = "No",
        FirstAuxiliaryButtonText = "Cancel",
      };
      _verificationWindow = new VerificationDialogViewModel();
      
/*
      _verificationWindow.ButtonCancel.Click += ButtonCancelOnClick;
      _verificationWindow.ButtonLogin.Click += ButtonLoginOnClick;*/
      _customDialog.Content = _verificationWindow;
      _customDialog.ShowDialogExternally();

      await metroWindow.ShowMetroDialogAsync(_customDialog);
    }

    public void ShowAboutDialog()
    {
      new AboutDialog().ShowAboutDialog();
    }

    public  void ShowCustomDialog()
    {
      new VerificationDialogViewModel().ShowVerificationDialog();
    }

    public void ShowLoginDialogPasswordPreview()
    {
      new AuthenticationDialog().ShowLoginDialogPasswordPreview();
    }  
    public void ShowAreYouSure()
    {
      new AreYouSureDialog().ShowMessageDialog();
    }

    public void ShowProgressDialog()
    {
      new FlashLoadScreen().ShowProgressDialog();
    }
   
  }
}
