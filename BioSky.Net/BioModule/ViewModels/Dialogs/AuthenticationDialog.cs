using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using MahApps.Metro.Controls;
using Caliburn.Micro;

namespace BioModule.ViewModels.Dialogs
{
  class AuthenticationDialog : PropertyChangedBase
  {
    public async void ShowLoginDialogPasswordPreview()
    {
      var metroWindow = (Application.Current.MainWindow as MetroWindow);

      var mySettings = new LoginDialogSettings { UsernameWatermark = "Username", ColorScheme = metroWindow.MetroDialogOptions.ColorScheme 
                                  , InitialUsername = "admin"
                                  , EnablePasswordPreview = true 
                                  , NegativeButtonText = "Cancel"
                                  , FirstAuxiliaryButtonText = "Register"
                                  , SecondAuxiliaryButtonText = "Register"
                                  , AffirmativeButtonText = "Login"
                                  , NegativeButtonVisibility = Visibility.Visible                                   
                                  };

      LoginDialogData result = await metroWindow.ShowLoginAsync("Authentication", "Enter your Username and Password" 
                                                               , mySettings);
      if (result == null)
      {
        //User pressed cancel
        
      }
      else
      {
        UserLogin = result.Username;
        UserPassword = result.Password;
        MessageDialogResult messageResult = await metroWindow.ShowMessageAsync("Authentication Information", String.Format("Login: {0}\nPassword: {1}", result.Username, result.Password));
      }
    }

    private string _userLogin;

    public string UserLogin
    {
      get { return _userLogin; }
      set
      {
        if (_userLogin == value)
          return;

        _userLogin = value;
        NotifyOfPropertyChange(() => UserLogin);
      }
    }

    private string _userPassword;

    public string UserPassword
    {
      get { return _userPassword; }
      set
      {
        if (_userPassword == value)
          return;

        _userPassword = value;
        NotifyOfPropertyChange(() => UserPassword);
      }
    }

  }
}
