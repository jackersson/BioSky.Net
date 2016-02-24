using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface ILogger
  {
    void LogException(_Exception exception);
    void LogMessage  ( string message );
  }
}
