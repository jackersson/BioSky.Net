using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;


namespace BioModule.ViewModels.Dialogs
{
  public class VerificationDialogViewModel: PropertyChangedBase
  {
    public VerificationDialogViewModel() 
    {
    }

    public async void ShowVerificationDialog()
    {
      var metroWindow = (Application.Current.MainWindow as MetroWindow);
      
      var dialog = new CustomDialog();
      var verificationWindow = new VerificationDialogViewModel();
      dialog.Content = verificationWindow;

      await metroWindow.ShowMetroDialogAsync(dialog);
    }

    public VerificationDialogViewModel _verViewModel;
    public VerificationDialogViewModel VerViewModel
    {
      get { return _verViewModel; }
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

    public void ButtonYesOnClick()
    {
      ValidationAnswer = true;
    }
    public void ButtonNolOnClick()
    {
      ValidationAnswer = false;
    }

    public Button _buttonYes;

    public Button ButtonYes
    {
      get { return _buttonYes; }
      set
      {
        if (_buttonYes == value)
          return;

        _buttonYes = value;
        NotifyOfPropertyChange(() => ButtonYes);
      }
    }

    public Button _buttonNo;

    public Button ButtonNo
    {
      get { return _buttonNo; }
      set
      {
        if (_buttonNo == value)
          return;

        _buttonNo = value;
        NotifyOfPropertyChange(() => ButtonNo);
      }
    }
  }
}
