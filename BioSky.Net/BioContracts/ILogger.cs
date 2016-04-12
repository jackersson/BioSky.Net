using System.Runtime.InteropServices;

namespace BioContracts
{
  public interface ILogger
  {
    void LogException(_Exception exception);
    void LogMessage  ( string message );
  }
}
