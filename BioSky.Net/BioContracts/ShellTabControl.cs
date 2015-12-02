using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public class ShellTabControl
  {
    private ObservableCollection<ShellTabPage> _tabPages;
    public ShellTabControl()
    {
      _tabPages = new ObservableCollection<ShellTabPage>();
    }
    public ObservableCollection<ShellTabPage> TabPages { get { return _tabPages;  } }
    public object ScreenViewModel { get; set; }
 
       
  }
}
