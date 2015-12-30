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
  class AreYouSureDialog : PropertyChangedBase
  {
    public async void ShowMessageDialog()
    {
      var metroWindow = (Application.Current.MainWindow as MetroWindow);
      var mySettings = new MetroDialogSettings()
      {
        AffirmativeButtonText = "Yes",
        NegativeButtonText = "No",        
        ColorScheme = MetroDialogColorScheme.Accented
      };

      MessageDialogResult result = await metroWindow.ShowMessageAsync("Are you sure?", "Are you sure?",
          MessageDialogStyle.AffirmativeAndNegative, mySettings);

      if (result == MessageDialogResult.Affirmative)
      {
        await metroWindow.ShowMessageAsync("Result", "Deleted");
        ValidationAnswer = true;
      }

      if (result == MessageDialogResult.Negative)
      {
        await metroWindow.ShowMessageAsync("Result", "Canceled");
        ValidationAnswer = false;
      }
    }

    private bool _validationAnswer;

    public bool ValidationAnswer
    {
      get { return _validationAnswer; }
      set
      {
        if (_validationAnswer == value)
          return;

        _validationAnswer = value;
        NotifyOfPropertyChange(() => ValidationAnswer);
      }
    }
  }
}
