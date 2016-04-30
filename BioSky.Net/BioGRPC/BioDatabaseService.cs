using BioContracts;
using BioService;
using BioContracts.Services;
using BioGRPC.DatabaseClient;
using Grpc.Core;
using System;
using BioGRPC.Utils;
using BioContracts.Services.Common;
using System.Collections.Generic;

namespace BioGRPC
{
  public class BioDatabaseService : ServiceBase, IDatabaseService
  { 
    public BioDatabaseService( IProcessorLocator locator )
    {
      _locator = locator;
      Initialize();    
    }

    public BioDatabaseService(IProcessorLocator locator, string address)
    {
      _locator = locator;
       Address = address;
      Initialize();
    }

    private void Initialize()
    {
      _utils       = new NetworkUtils();
      _dataClients = new List<IDataClientUpdateAble>();

      _visitorDataClient   = new VisitorDataClient (_locator);
      _photosDataClient    = new PhotoDataClient   (_locator);
      _personDataClient    = new PersonDataClient  (_locator);
      _locationDataClient  = new LocationDataClient(_locator);
      _cardsDataClient     = new CardDataClient    (_locator);

      _dataClients.Add(_visitorDataClient );
      _dataClients.Add(_photosDataClient  );
      _dataClients.Add(_personDataClient  );
      _dataClients.Add(_locationDataClient);
      _dataClients.Add(_cardsDataClient   );
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

    protected override void CreateClient()
    {
      _client = BiometricDatabaseSevice.NewClient(Channel);

      foreach (IDataClientUpdateAble dataClient in _dataClients)
        dataClient.Update(_client);
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

    private List<IDataClientUpdateAble> _dataClients;

    private BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
    private NetworkUtils _utils;
    private readonly IProcessorLocator _locator;
  }
}
