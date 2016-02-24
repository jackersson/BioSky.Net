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
  public class AboutDialogViewModel : Screen
  {
    public AboutDialogViewModel(string title = "About")
    {
      Update(title);
    }

    public void Update(string title = "About")
    {
      DisplayName = title;      
    }

    public void Apply()
    {
      DialogResult = true;
      TryClose(DialogResult);
    }

    private bool _dialogResult;
    public bool DialogResult
    {
      get { return _dialogResult; }
      set
      {
        if (_dialogResult != value)
        {
          _dialogResult = value;
          NotifyOfPropertyChange(() => DialogResult);
        }
      }
    }
  }
}
