using System.Runtime.InteropServices;

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
