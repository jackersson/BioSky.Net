using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IPermissionController
  {
    Rights CurrentPermissionRights { get;}

    bool isActivityAllowed(Activity activity);

    void UpdateAuthenticatedPersonRights(Rights rights);
  }
}
