using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface ITabControl
  {
    ShellTabControl TabControl { get; }

    void Init();

    void OpenTab(Type tabType);
  }
}
