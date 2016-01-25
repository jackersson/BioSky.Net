using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.Utils
{
  public class FastMethodInvoker
  {

    //TODO all methods should be as interface
    public object InvokeMethod(Type objectType, string methodName, object source, object[] args = null)
    {
      MethodInfo method = objectType.GetMethod(methodName);
      if (method != null)
        return method.Invoke(source, args);
      return null;
    }
  }
}
