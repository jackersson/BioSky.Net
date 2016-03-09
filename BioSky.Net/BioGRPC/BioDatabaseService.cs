using BioContracts;
using BioService;
using BioContracts.Services;
using BioGRPC.DatabaseClient;

namespace BioGRPC
{
  public class BioDatabaseService : IDatabaseService
  { 
    public BioDatabaseService( IProcessorLocator locator
                             , BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client)
    {    
      _visitorDataClient   = new VisitorDataClient (locator, client);
      _photoDataClient     = new PhotoDataClient   (locator, client);
      _personDataClient    = new PersonDataClient  (locator, client);
      _locationDataClient  = new LocationDataClient(locator, client);
    }
    
    private IDataClient<Visitor, QueryVisitors> _visitorDataClient;
    public IDataClient<Visitor, QueryVisitors> VisitorDataClient
    {
      get { return _visitorDataClient; }     
    }

    private IDataClient<Photo, QueryPhoto> _photoDataClient;
    public IDataClient<Photo, QueryPhoto> PhotoDataClient
    {
      get { return _photoDataClient; }
    }

    private IDataClient<Person, QueryPersons> _personDataClient;
    public IDataClient<Person, QueryPersons> PersonDataClient
    {
      get { return _personDataClient; }
    }

    private IDataClient<Location, QueryLocations> _locationDataClient;
    public IDataClient<Location, QueryLocations> LocationDataClient
    {
      get { return _locationDataClient; }
    }    

  }
}
