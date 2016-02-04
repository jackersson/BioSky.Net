using System;
using System.Threading.Tasks;

using BioContracts;
using BioService;
using Grpc.Core;
using BioContracts.Services;

namespace BioGRPC
{
  public class BioDatabaseService : IDatabaseService
  { 
    public BioDatabaseService(IProcessorLocator locator, BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client)
    {
      _client    = client;
      _locator   = locator;

      _database = _locator.GetProcessor<IBioSkyNetRepository>();
    }
    
   
    public async Task CaptureDeviceRequest(CommandCaptureDevice command)
    {
      try
      {
        CaptureDeviceList call = await _client.CaptureDeviceSelectAsync(command);
        _database.CaptureDeviceHolder.Update(call.CaptureDevices);

        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }
    
    public async Task AccessDeviceRequest(CommandAccessDevice command)
    {
      try
      {
        AccessDeviceList call = await _client.AccessDeviceSelectAsync(command);
        _database.AccessDeviceHolder.Update(call.AccessDevices);

        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task LocationRequest(CommandLocation command)
    {
      try
      {
        LocationList call = await _client.LocationSelectAsync(command);
        _database.LocationHolder.Update(call.Locations);

        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task PhotoRequest(CommandPhoto command)
    {
      try
      {
        PhotoList call = await _client.PhotoSelectAsync(command);
        _database.PhotoHolder.Update(call.Photos);

        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task CardRequest(CommandCard command)
    {
      try
      {
        CardList call = await _client.CardSelectAsync(command);
        _database.CardHolder.Update(call.Cards);

        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task VisitorRequest(CommandVisitor command)
    {
      try
      {
        VisitorList call = await _client.VisitorSelectAsync(command);
        _database.VisitorHolder.Update(call.Visitors);

        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task PersonRequest(CommandPerson command)
    {
      try
      {
        PersonList call = await _client.PersonSelectAsync(command);
        _database.PersonHolder.Update(call.Persons);

        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task PersonUpdateRequest(PersonList list)
    {
      try
      {
        Result call = await _client.PersonUpdateAsync(list);
        _database.PersonHolder.Update(list.Persons, call);      
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }


    public async Task CardUpdateRequest(CardList list)
    {
      try
      {
        Result call = await _client.CardUpdateAsync(list);
        _database.CardHolder.Update(list.Cards, call);        
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task VisitorUpdateRequest(VisitorList list)
    {
      try
      {
        Result call = await _client.VisitorUpdateAsync(list);
        _database.VisitorHolder.Update(list.Visitors, call);        
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task LocationUpdateRequest(LocationList list)
    {
      try
      {
        Result call = await _client.LocationUpdateAsync(list);
        _database.LocationHolder.Update(list.Locations, call);
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task AccessDeviceUpdateRequest(AccessDeviceList list)
    {
      try
      {
        Result call = await _client.AccessDeviceUpdateAsync(list);
        _database.AccessDeviceHolder.Update(list.AccessDevices, call);
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task CaptureDeviceUpdateRequest(CaptureDeviceList list)
    {
      try
      {
        Result call = await _client.CaptureDeviceUpdateAsync(list);
        _database.CaptureDeviceHolder.Update(list.CaptureDevices, call);
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task PhotoUpdateRequest(PhotoList list)
    {
      try
      {
        Result call = await _client.PhotoUpdateAsync(list);
        _database.PhotoHolder.Update(list.Photos, call);
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    private void Log(string s, params object[] args)
    {
      Console.WriteLine(string.Format(s, args));
    }

    private void Log(string s)
    {
      Console.WriteLine(s);
    }

    private readonly IProcessorLocator                                      _locator;
    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
    private readonly IBioSkyNetRepository                                   _database; 
  }
}
