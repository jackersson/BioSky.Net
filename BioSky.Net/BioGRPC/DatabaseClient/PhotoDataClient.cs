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

      _rawIndexes = new RawIndexes();      
    }
      /*
    public async Task Select(QueryPhoto command)
    {
      try
      {
        PhotoList call = await _client.SelectPhotosAsync(command);
        //_database.PhotoHolder.Update(call.Photos);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
    */
    public async Task Add(Person owner, Photo requested)
    {
      if (requested == null || owner == null)
        return;

      try
      {
        Photo responded = await _client.AddPhotoAsync(requested);
        Console.WriteLine(responded);
        _database.Persons.AddPhoto(owner, requested, responded);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public Task Update(Photo item)
    {
      return null;
    }

    public async Task Remove(Person owner, IList<Photo> targeIds)
    {
      if (targeIds == null || targeIds.Count <= 0 || owner == null)
        return;
      
      _rawIndexes.Indexes.Clear();
      foreach (Photo item in targeIds)
        _rawIndexes.Indexes.Add(item.Id);

      try {
        await RemovePerformer(owner, _rawIndexes);
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }
    }

    public async Task Remove(Person owner, Photo item)
    {
      if (item == null || owner == null)
        return;

      _rawIndexes.Indexes.Clear();
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

      try {
        RawIndexes result = await _client.RemovePhotosAsync(rawIndexes);
        Console.WriteLine(result);
        _database.Persons.RemovePhotos(owner, rawIndexes.Indexes, result.Indexes);
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
