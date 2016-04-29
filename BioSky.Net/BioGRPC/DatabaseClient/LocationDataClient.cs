using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BioGRPC.DatabaseClient
{
  public class LocationDataClient : IDataClient<Location, QueryLocations>
  {
    public LocationDataClient( IProcessorLocator locator )
    {
      _locator  = locator;

      _database = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier = _locator.GetProcessor<INotifier>();     
    }

    public async Task Add(Location item)
    {
      if (item == null)
        return;

      try
      {
        Location newLocation = await _client.AddLocationAsync(item);
        Console.WriteLine(newLocation);
        _database.Locations.Add(item, newLocation);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Update(Location item)
    {
      if (item == null)
        return;

      try
      {
        Location call = await _client.UpdateLocationAsync(item);
        Console.WriteLine(call);
        _database.Locations.Update(item, call);
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }      
    }

    public async Task Remove(Location targetItem)
    {
      if (targetItem == null || targetItem.Id <= 0)
        return;

      try
      {
        var result = await _client.RemoveLocationAsync(new Location() { Id = targetItem.Id });
        Console.WriteLine(result);
        _database.Locations.Remove(targetItem, result);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Select(QueryLocations command)
    {
      try
      {
        LocationList call = await _client.SelectLocationsAsync(command);
        _database.Locations.Init(call.Locations);    
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
    
    public Task Remove( IList<Location> targeIds)
    {
      throw new NotImplementedException();
    }

    public void Update(BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client)
    {
      _client = client;
    }

    private readonly IProcessorLocator    _locator   ;
    private readonly IBioSkyNetRepository _database  ;    
    private readonly INotifier            _notifier  ;
    private BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
