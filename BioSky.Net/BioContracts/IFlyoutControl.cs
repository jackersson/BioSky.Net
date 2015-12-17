using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IFlyoutControl
  {
    ShellFlyoutControl FlyoutControl { get; }

    void ShowPage(Type flyoutPage, object[] args);
  }
}
