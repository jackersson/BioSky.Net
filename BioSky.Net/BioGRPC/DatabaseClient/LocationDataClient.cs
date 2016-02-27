using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioGRPC.DatabaseClient
{
  public class LocationDataClient : IDataClient<Location, CommandLocations>
  {
    public LocationDataClient( IProcessorLocator locator
                             , BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client )
    {
      _client   = client;
      _locator  = locator;

      _database = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier = _locator.GetProcessor<INotifier>();

      _list = new LocationList();
    }

    private async Task Update(LocationList list)
    {
      try
      {
        LocationList call = await _client.LocationUpdateAsync(list);
        _database.Locations.Update(list, call);       
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Select(CommandLocations command)
    {
      try
      {
        LocationList call = await _client.LocationSelectAsync(command);
        _database.Locations.Init(call);        
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Add(Location item)
    {
      if (item == null)
        return;

      _list.Locations.Clear();

      //TODO ResultStatus None 
      item.Dbresult    = ResultStatus.Failed;
      item.EntityState = EntityState.Added  ;
      _list.Locations.Add(item);    

      try
      {
        await Update(_list);
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

      _list.Locations.Clear();

      //TODO ResultStatus None 
      item.Dbresult    = ResultStatus.Failed;
      item.EntityState = EntityState.Modified;
      _list.Locations.Add(item);

      try
      {
        await Update(_list);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }



    public async Task Delete( IList<long> targeIds)
    {
      if (targeIds == null || targeIds.Count <=0 )
        return;


      _list.Locations.Clear();

      Dictionary<long, Location> dictionary = _database.LocationHolder.DataSet;
      foreach (long id in targeIds)
      {       
        Location item = null;      
        if (dictionary.TryGetValue(id, out item))
        {
          Location newItem = new Location()
          {
              Id = id
            , EntityState = EntityState.Deleted
            , Dbresult    = ResultStatus.Failed          
          };
          _list.Locations.Add(newItem);
        }
      }

      if (_list.Locations.Count <= 0)
        return;

      try
      {
        await Update(_list);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    private LocationList _list;

    private readonly IProcessorLocator    _locator   ;
    private readonly IBioSkyNetRepository _database  ;    
    private readonly INotifier            _notifier  ;
    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
