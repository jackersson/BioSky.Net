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
    ShellFlyoutControl FlyoutControl { get; }
    ShellTabControl TabControl { get; }

    ShellToolBar ToolBar { get; }

    ShellMainMenu MainMenu { get; }
    ShellLoginInformation LoginInformation { get; }
    
    IBioModule LoadModule(Assembly assembly);
  }
}
