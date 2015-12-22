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
  class AboutDialog : PropertyChangedBase
  {
    public async void ShowAboutDialog()
    {
      var metroWindow = (Application.Current.MainWindow as MetroWindow);
      var mySettings = new MetroDialogSettings()
      {        
        AffirmativeButtonText = "Ok",        
        ColorScheme = MetroDialogColorScheme.Accented
      };

      MessageDialogResult result = await metroWindow.ShowMessageAsync("ABOUT", "BioSkyNet" 
      + Environment.NewLine + Environment.NewLine + "Jam" 
      + Environment.NewLine + Environment.NewLine + "Taras and Sasha :)))",
      MessageDialogStyle.Affirmative, mySettings);     
    }
  }
}
