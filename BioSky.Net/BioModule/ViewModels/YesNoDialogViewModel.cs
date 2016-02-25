using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;

namespace BioModule.ViewModels
{
  public class YesNoDialogViewModel : Screen
  {
    public YesNoDialogViewModel(string title = "Custom Dialog", string dialogMessage = "AreYouSure")
    {
      Update(title, dialogMessage);      
    }

    public void Update(string title = "Custom Dialog", string dialogMessage = "AreYouSure")
    {
      DisplayName = title;
      DialogMessage = dialogMessage;
    }

    public void Apply()
    {      
      this.TryClose(true);
    }
    public void Cancel()
    {      
      this.TryClose(false);
    }

    private string _dialogMessage;
    public string DialogMessage
    {
      get { return _dialogMessage; }
      set
      {
        if (_dialogMessage != value)
        {
          _dialogMessage = value;
          NotifyOfPropertyChange(() => DialogMessage);
        }
      }
    }   
  }
}
