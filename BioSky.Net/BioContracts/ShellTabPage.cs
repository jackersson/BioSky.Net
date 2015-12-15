using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public class ShellTabPage
  {
    public string Caption { get; set; }
    public object ScreenViewModel { get; set; }

    public bool CanClose { get; set; }
  }
}
