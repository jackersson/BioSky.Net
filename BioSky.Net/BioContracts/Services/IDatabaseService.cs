using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{

  public interface IDatabaseService
  {   
    Task CaptureDeviceRequest(CommandCaptureDevice command);
    Task CaptureDeviceUpdateRequest(CaptureDeviceList list);

    Task AccessDeviceRequest(CommandAccessDevice command);
    Task AccessDeviceUpdateRequest(AccessDeviceList list);

    Task PhotoRequest       (CommandPhoto command);
    Task PhotoUpdateRequest(PhotoList list);

    Task CardRequest        (CommandCard command);
    Task CardUpdateRequest  (CardList list);

    Task VisitorRequest     (CommandVisitor command);
    Task VisitorUpdateRequest(VisitorList list);

    Task PersonRequest      (CommandPerson command);
    Task PersonUpdateRequest(PersonList command);

    Task LocationRequest(CommandLocation command);

    Task LocationUpdateRequest(LocationList list);
  }
}
