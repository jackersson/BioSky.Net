using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public class ShellFlyoutControl
  {
    private ObservableCollection<ShellFlyoutPage> _flyoutPages;

    public ShellFlyoutControl()
    {
      _flyoutPages = new ObservableCollection<ShellFlyoutPage>();
    }

    public ObservableCollection<ShellFlyoutPage> FlyoutPages { get { return _flyoutPages;  } }
  
  }
}
