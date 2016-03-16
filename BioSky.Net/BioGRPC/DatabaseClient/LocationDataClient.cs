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
       // LocationList call = await _client.LocationUpdateAsync(list);
        //_database.Locations.Update(list, call);       
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
        //LocationList call = await _client.LocationSelectAsync(command);
        //_database.Locations.Init(call);        
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
      //item.Dbresult    = ResultStatus.Failed;
     // item.EntityState = EntityState.Added  ;
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
      //item.Dbresult    = ResultStatus.Failed;
      //item.EntityState = EntityState.Modified;
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



    public async Task Remove( IList<Location> targeIds)
    {
      if (targeIds == null || targeIds.Count <=0 )
        return;


      _list.Locations.Clear();

      //Dictionary<long, Location> dictionary = _database.LocationHolder.DataSet;
      foreach (Location item in targeIds)
      {               
        Location newItem = new Location()
        {
            Id = item.Id
          //, EntityState = EntityState.Deleted
          //, Dbresult    = ResultStatus.Failed          
        };
        _list.Locations.Add(newItem);
        
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

    public Task Remove(Location targetItem)
    {
      throw new NotImplementedException();
    }


    /*
   public async Task LocationDeletePerformer(EntityState state)
   {
     LocationList locationList = new LocationList();

     if (SelectedTrackLocation == null)
       return;

     Location location = new Location() { Id = SelectedTrackLocation.LocationID
                                        , EntityState = EntityState.Deleted };
     locationList.Locations.Add(location);      

     try
     {
      // _database.Locations.DataUpdated += UpdateData;
      // await _bioService.DatabaseService.LocationUpdate(locationList);
     }
     catch (RpcException e)
     {
       Console.WriteLine(e.Message);
     }
   }

   private void UpdateData(LocationList list)
   {
     _database.Locations.DataUpdated -= UpdateData;

     if (list != null)
     {
       Location location = list.Locations.FirstOrDefault();
       if (location != null)
       {
         if (location.EntityState == EntityState.Deleted)
         {
           if (list.Locations.Count > 1)
             MessageBox.Show(list.Locations.Count + " locations successfully Deleted");
           else
             MessageBox.Show("Location successfully Deleted");
         }
       }
     }
   }   
  

    public void UpdateData(LocationList list)
    {
      _database.Locations.DataUpdated -= UpdateData;

      if (list != null)
      {
        Location location = list.Locations.FirstOrDefault();
        if (location != null)
        {
          if (location.EntityState == EntityState.Deleted)
          {
            location = null;
            MessageBox.Show("Location successfully Deleted");
          }
          else if (location.EntityState == EntityState.Added)
            MessageBox.Show("Location successfully Added");
          else if (location.EntityState == EntityState.Unchanged)
          {
            location.LocationName = RevertLocation.LocationName;
            location.Description = RevertLocation.Description;
            MessageBox.Show("Location successfully Updated");
          }
          else
            MessageBox.Show("Location successfully Updated");

          Update(location);
        }
      }
    }
     */


    private LocationList _list;

    private readonly IProcessorLocator    _locator   ;
    private readonly IBioSkyNetRepository _database  ;    
    private readonly INotifier            _notifier  ;
    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
