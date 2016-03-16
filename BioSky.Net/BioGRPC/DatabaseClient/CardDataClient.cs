using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BioGRPC.DatabaseClient
{
  public class CardDataClient :  IOwnerDataClient<Person, Card>
  {
    public CardDataClient(  IProcessorLocator locator
                          , BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client)
    {
      _client = client;
      _locator = locator;

      _database = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier = _locator.GetProcessor<INotifier>();

      _rawIndexes = new RawIndexes();
    }

    public async Task Add(Person owner, Card item)
    {
      if (item == null || owner == null)
        return;

      try
      {
        Card newCard = await _client.AddCardAsync(item);
        Console.WriteLine(item);
        _database.Persons.AddCard(owner, item, newCard);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Remove(Person owner, Card item)
    {
      if (item == null || owner == null)
        return;

      _rawIndexes.Indexes.Clear();
      _rawIndexes.Indexes.Add(item.Id);

      try  {
        await RemovePerformer(owner, _rawIndexes);
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }
    }

    public async Task Remove(Person owner, IList<Card> targeIds)
    {
      if (targeIds == null || targeIds.Count <= 0 || owner == null)
        return;


      _rawIndexes.Indexes.Clear();   

      foreach (Card item in targeIds)      
        _rawIndexes.Indexes.Add(item.Id);      

      try {
        await RemovePerformer(owner, _rawIndexes);
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }
    }

    private async Task RemovePerformer(Person owner, RawIndexes rawIndexes)
    {
      if (rawIndexes.Indexes.Count <= 0 || owner == null)
        return;

      try
      {
        RawIndexes result = await _client.RemoveCardsAsync(rawIndexes);
        Console.WriteLine(result);
        _database.Persons.RemoveCards(owner, rawIndexes.Indexes, result.Indexes);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }


    private RawIndexes _rawIndexes;

    private readonly IProcessorLocator _locator;
    private readonly IBioSkyNetRepository _database;
    private readonly INotifier _notifier;
    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
