using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.ViewModels
{
  public class FingerBarViewModel : Screen
  {
    public FingerBarViewModel()
    {
      Fingers = new ObservableCollection<string>();
      for(int i = 1; i <= 10; ++i)
      {
        Fingers.Add(i.ToString());
      }

      Fingers.Add("Test");
    }

    private ObservableCollection<string> _fingers;
    public ObservableCollection<string> Fingers
    {
      get { return _fingers; }
      set
      {
        if (_fingers != value)
        {
          _fingers = value;
          NotifyOfPropertyChange(() => Fingers);
        }
      }
    }
  }
}
