using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public delegate void PersonUpdateHandler (PersonList list , Result result);

  public delegate void CardUpdateHandler   (CardList list   , Result result);

  public delegate void VisitorUpdateHandler(VisitorList list, Result result);

  public interface IDatabaseService
  {
    event PersonUpdateHandler  PersonUpdated ;
    event CardUpdateHandler    CardUpdated   ;
    event VisitorUpdateHandler VisitorUpdated;

    Task CaptureDeviceRequest(CommandCaptureDevice command);
    Task AccessDeviceRequest(CommandAccessDevice command);   
    Task PhotoRequest       (CommandPhoto command);

    Task CardRequest        (CommandCard command);
    Task CardUpdateRequest  (CardList list);

    Task VisitorRequest     (CommandVisitor command);
    Task VisitorUpdateRequest(VisitorList list);

    Task PersonRequest      (CommandPerson command);
    Task PersonUpdateRequest(PersonList command);

    Task LocationRequest(CommandLocation command);

  }
}
