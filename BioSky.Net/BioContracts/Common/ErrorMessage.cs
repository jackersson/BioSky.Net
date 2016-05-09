using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Common
{
  public class ErrorMessage
  {
    public ErrorMessage(Exception exception, Enum exceptionEnum )
    {
      ExceptionMessage = exception    ;
      ExceptionEnum    = exceptionEnum;
    }

    private Exception _exceptionMessage;
    public Exception ExceptionMessage
    {
      get { return _exceptionMessage; }
      set
      {
        if (_exceptionMessage != value)
          _exceptionMessage = value;

      }
    }

    private Enum _exceptionEnum;
    public Enum ExceptionEnum
    {
      get { return _exceptionEnum; }
      set
      {
        if (_exceptionEnum != value)
          _exceptionEnum = value;
      }
    }
  }
}
