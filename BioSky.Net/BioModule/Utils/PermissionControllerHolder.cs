using BioContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.Utils
{
  public class PermissionControllerHolder
  {
    public PermissionControllerHolder(IProcessorLocator locator)
    {
      _permissionController = locator.GetProcessor<IPermissionController>();
    }

    private static IPermissionController _permissionController;
    public static IPermissionController PermissionController
    {
      get { return _permissionController; }
    }
  }
}
