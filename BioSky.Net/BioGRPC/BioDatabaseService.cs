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
    
    private IDataClient<Visitor, CommandVisitors> _visitorDataClient;
    public IDataClient<Visitor, CommandVisitors> VisitorDataClient
    {
      get { return _visitorDataClient; }     
    }

    private IDataClient<Photo, CommandPhoto> _photoDataClient;
    public IDataClient<Photo, CommandPhoto> PhotoDataClient
    {
      get { return _photoDataClient; }
    }

    private IDataClient<Person, CommandPersons> _personDataClient;
    public IDataClient<Person, CommandPersons> PersonDataClient
    {
      get { return _personDataClient; }
    }

    private IDataClient<Location, CommandLocations> _locationDataClient;
    public IDataClient<Location, CommandLocations> LocationDataClient
    {
      get { return _locationDataClient; }
    }    

  }
}
