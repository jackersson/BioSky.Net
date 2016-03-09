using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
    void Notify(int progress, bool status, double pointX, double pointY);
    
    object LoadingViewModel { get; }
  }
}
