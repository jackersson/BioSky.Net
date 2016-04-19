using BioContracts;
using BioService;
using BioContracts.Services;
using BioGRPC.DatabaseClient;
using Grpc.Core;
using System;
using BioGRPC.Utils;

namespace BioGRPC
{
  public class BioDatabaseService : IDatabaseService
  { 
    public BioDatabaseService( IProcessorLocator locator
                             , BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client)
    {
      _client = client;

      _utils = new NetworkUtils();

      _visitorDataClient   = new VisitorDataClient (locator, client);
      _photosDataClient    = new PhotoDataClient   (locator, client);
      _personDataClient    = new PersonDataClient  (locator, client);
      _locationDataClient  = new LocationDataClient(locator, client);
      _cardsDataClient     = new CardDataClient    (locator, client);
    }

    public async void Subscribe()
    {
      try
      {
        BioClient currentPCInfo = new BioClient();
        currentPCInfo.IpAddress  = _utils.GetLocalIPAddress();
        currentPCInfo.MacAddress = _utils.GetMACAddress();

        var call = await _client.AddClientAsync(currentPCInfo);
        Console.WriteLine(call);
      }
      catch (RpcException ex) {
        Console.WriteLine(ex.Message);
      }      
    }



    private PhotoDataClient _photosDataClient;
    public IOwnerDataClient<Person, Photo> PhotosDataClient
    {
      get { return _photosDataClient; }
    }
 
    public IThumbnailDataClient ThumbnailDataClient
    {
      get { return _personDataClient; }
    }

    private CardDataClient _cardsDataClient;
    public IOwnerDataClient<Person, Card> CardsDataClient
    {
      get { return _cardsDataClient; }
    }

    private VisitorDataClient _visitorDataClient;
    public IDataClient<Visitor, QueryVisitors> VisitorDataClient
    {
      get { return _visitorDataClient; }     
    }    

    private PersonDataClient _personDataClient;
    public IDataClient<Person, QueryPersons> PersonDataClient
    {
      get { return _personDataClient; }
    }

    private LocationDataClient _locationDataClient;
    public IDataClient<Location, QueryLocations> LocationDataClient
    {
      get { return _locationDataClient; }
    }

    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
    private NetworkUtils _utils;
  }
}
