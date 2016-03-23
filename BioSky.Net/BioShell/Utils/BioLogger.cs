using BioContracts;
using System;
using System.Runtime.InteropServices;

namespace BioShell.Utils
{
  public class BioLogger : ILogger
  {
    public void LogException(_Exception exception)
    {
      if (exception == null)
        return;

      LogMessage(exception.Message);
    }

    public void LogMessage(string message)
    {
      Console.WriteLine(message);
    }
  }
}
