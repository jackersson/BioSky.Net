using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace BioContracts
{
  public interface IBioShell
  {
    object FlyoutControl { get; set; }
    object TabControl { get; set; }

    object ToolBar { get; set; }

    object MainMenu { get; set; } 

    object LoginInformation { get; set; }

    IBioModule LoadModule(Assembly assembly);
  }
}
