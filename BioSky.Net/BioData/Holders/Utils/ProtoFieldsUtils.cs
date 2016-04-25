using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders.Utils
{
  public class ProtoFieldsUtils
  {
    public const string FIELD_DELETE_STATE = "delete";

    public bool IsDeleteState(string field)
    {
      if (string.IsNullOrEmpty(field))
        return false;

      string deleteState = FIELD_DELETE_STATE;
      return deleteState.Equals(field);
    }
  }
}
