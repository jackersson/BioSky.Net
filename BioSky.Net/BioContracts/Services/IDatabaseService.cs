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
    IDataClient<Visitor , QueryVisitors>  VisitorDataClient  { get; }
    IDataClient<Person  , QueryPersons>   PersonDataClient   { get; }
    IDataClient<Location, QueryLocations> LocationDataClient { get; }
    //IDataClient<Photo   , QueryPhoto>     PhotoDataClient    { get; }
    IOwnerDataClient<Person, Card>        CardsDataClient    { get; }
    IOwnerDataClient<Person, Photo>       PhotosDataClient { get; }
  }
}
