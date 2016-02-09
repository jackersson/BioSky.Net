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
    Task PersonsSelect(CommandPersons command);


    Task VisitorsSelect(CommandVisitors command);


    Task LocationsSelect(CommandLocations command);


    Task PhotosSelect(CommandPhoto command);


    Task PersonUpdate(PersonList persons);


    Task VisitorUpdate(VisitorList visitors);

    Task LocationUpdate(LocationList locations);


    Task AddSocket(SocketConfiguration config);
   

  }
}
