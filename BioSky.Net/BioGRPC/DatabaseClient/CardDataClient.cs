using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BioGRPC.DatabaseClient
{
  public class CardDataClient :  IOwnerDataClient<Person, Card>
  {
    public CardDataClient(  IProcessorLocator locator  )
    {
      _locator = locator;

      _database = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier = _locator.GetProcessor<INotifier>();
    }

    public async Task Add(long ownerId, Card request)
    {
      if (request == null)
        return;

      try
      {
        Card response = await _client.AddCardAsync(request);
        Console.WriteLine(request);
        
        _database.Persons.CardDataHolder.UpdateFromResponse( _database.Persons.GetValue(ownerId)
                                                           , request
                                                           , response);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Remove(long ownerId, Card item)
    {
      if (item == null)
        return;

      Card request = new Card() { Id = item.Id };

      try  {

        var response = await _client.RemoveCardAsync(request);
        Console.WriteLine(response);
        
        _database.Persons.CardDataHolder.UpdateFromResponse(_database.Persons.GetValue(ownerId)
                                                           , request
                                                           , response);      
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }
    }

    public async Task Remove(long ownerId, IList<Card> targeIds)
    {
      if (targeIds == null || targeIds.Count <= 0 )
        return;

      CardList request = new CardList();
      request.Cards.Add(targeIds.Select(x => new Card() { Id = x.Id }));
        

      try {
        var response = await _client.RemoveCardsAsync(request);
        Console.WriteLine(response);
                
        if (response == null)
          return;
        _database.Persons.CardDataHolder.UpdateFromResponse(_database.Persons.GetValue(ownerId)
                                                           , request.Cards
                                                           , response.Cards);
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }
    }
     

    public Task Update(Person owner, Card item)
    {
      throw new NotImplementedException();
    }

    public void Update(BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client)
    {
      _client = client;
    }

    private readonly IProcessorLocator _locator;
    private readonly IBioSkyNetRepository _database;
    private readonly INotifier _notifier;
    private BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
