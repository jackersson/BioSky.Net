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
    public AboutDialogViewModel(IWindowManager windowManager, string title = "About")
    {
      _windowManager = windowManager;
      Update(title);
    }

    public void Update(string title = "About")
    {
      DisplayName = title;      
    }
    public void Show()
    {
      _windowManager.ShowDialog(this);
    }

    public void Apply()
    {
      TryClose(true);
    }

    public bool GetDialogResult()
    {
      return true;
    }

    private IWindowManager _windowManager;
  }
}
