using BioContracts;
using System.Runtime.InteropServices;
using System.Windows;

namespace BioShell.Utils
{
  public class BioNotifier : INotifier
  {
    
    public void Notify(_Exception exception)
    {
      if (exception == null)
        return;

      Notify(exception.Message, WarningLevel.Error);
    }

    public void Notify(string message, WarningLevel level)
    {
      MessageBox.Show(message, "Exception");
    }

  }
}
