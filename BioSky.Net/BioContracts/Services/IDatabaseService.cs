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
    Task CaptureDeviceUpdateRequest(CaptureDevice captureDevice);

    Task AccessDeviceRequest(CommandAccessDevice command);
    Task AccessDeviceUpdateRequest(AccessDeviceList list);
    Task AccessDeviceUpdateRequest(AccessDevice accessDevice);

    Task PhotoRequest       (CommandPhoto command);
    Task PhotoUpdateRequest (PhotoList list);
    Task PhotoUpdateRequest (Photo photo);

    Task CardRequest        (CommandCard command);
    Task CardUpdateRequest  (CardList list);
    Task CardUpdateRequest  (Card card);

    Task VisitorRequest     (CommandVisitor command);
    Task VisitorUpdateRequest(VisitorList list);
    Task VisitorUpdateRequest(Visitor visitor);

    Task PersonRequest      (CommandPerson command);
    Task PersonUpdateRequest(PersonList list);
    Task PersonUpdateRequest(Person person);

    Task LocationRequest(CommandLocation command);
    Task LocationUpdateRequest(LocationList list);
    Task LocationUpdateRequest(Location location);
  }
}
