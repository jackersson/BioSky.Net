using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public delegate void PersonUpdateHandler(PersonList list, Result result);
  public interface IDatabaseService
  {
    event PersonUpdateHandler PersonUpdated;

    Task CaptureDeviceRequest(CommandCaptureDevice command);
    Task AccessDeviceRequest(CommandAccessDevice command);   
    Task PhotoRequest       (CommandPhoto command);
    Task CardRequest        (CommandCard command);
    Task VisitorRequest     (CommandVisitor command);
    Task PersonRequest      (CommandPerson command);
    Task PersonUpdateRequest(PersonList command);

    Task LocationRequest(CommandLocation command);

  }
}
