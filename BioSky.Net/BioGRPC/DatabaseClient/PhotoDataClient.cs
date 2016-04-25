using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BioGRPC.DatabaseClient
{
  public class PhotoDataClient : IOwnerDataClient<Person, Photo>
  {
    public PhotoDataClient(  IProcessorLocator locator
                            , BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client)
    {
      _client  = client;
      _locator = locator;

      _database = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier = _locator.GetProcessor<INotifier>();

      _database.Photos.RequestPhoto += PhotoHolder_RequestPhoto;

      _rawIndexes = new RawIndexes();      
    }

    private async void PhotoHolder_RequestPhoto(QueryPhoto query)
    {
      try
      {
        await Select(query);       
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Select(QueryPhoto query)
    {
      try
      {
        PhotoList call = await _client.SelectPhotosAsync(query);
        Console.WriteLine(call);
        _database.Photos.UpdateFromQuery(query, call.Photos);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
    
    public async Task Add(long ownerId, Photo requested)
    {
      if (requested == null)
        return;

      try
      {
        Photo responded = await _client.AddPhotoAsync(requested);
        Console.WriteLine(responded);
        _database.Photos.UpdateFromResponse(requested, responded);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
    
    
    public async Task Remove(long ownerId, IList<Photo> targeIds)
    {
      if (targeIds == null || targeIds.Count <= 0 )
        return;
      
      _rawIndexes.Indexes.Clear();
      foreach (Photo item in targeIds)
        _rawIndexes.Indexes.Add(item.Id);

      try {
       // await RemovePerformer(owner, _rawIndexes);
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }
    }

    public async Task Remove(long ownerId, Photo item)
    {
      if (item == null )
        return;

      _rawIndexes.Indexes.Clear();
      _rawIndexes.Indexes.Add(item.Id);

      try {
        //await RemovePerformer(owner, _rawIndexes);
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }
    }

    private async Task RemovePerformer(Person owner, RawIndexes rawIndexes)
    {
      if (rawIndexes.Indexes.Count <= 0 || owner == null)
        return;

      try {
        RawIndexes result = await _client.RemovePhotosAsync(rawIndexes);
        Console.WriteLine(result);
        //_database.Persons.RemovePhotos(owner, rawIndexes.Indexes, result.Indexes);
      }
      catch (RpcException e) {
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
