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
    IDataClient<Visitor , CommandVisitors>  VisitorDataClient  { get; }
    IDataClient<Person  , CommandPersons>   PersonDataClient   { get; }
    IDataClient<Location, CommandLocations> LocationDataClient { get; }
    IDataClient<Photo   , CommandPhoto>     PhotoDataClient    { get; }
  }
}
