using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public enum WarningLevel
  {
      Information = 0
    , Warning     = 1
    , Error       = 2
  }
  public interface INotifier
  {
    void Notify(_Exception exception);
    void Notify(string message, WarningLevel level);
  }
}
