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

      _database.PhotoHolderByPerson.FullPhotoRequested += PhotoHolderByPerson_FullPhotoRequested;
    }

    private async void PhotoHolderByPerson_FullPhotoRequested(PhotoList list)
    {
      CommandPhoto cmd = new CommandPhoto();
      cmd.Description = true;

      foreach (Photo ph in list.Photos)      
        cmd.TargetPhoto.Add(new Photo() { Id = ph.Id });

      await PhotosSelect(cmd);
    }

    public async Task PersonsSelect(CommandPersons command)
    {
      try
      {
        PersonList call = await _client.PersonSelectAsync(command);
        _database.Persons.Init(call);

        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task VisitorsSelect(CommandVisitors command)
    {
      try
      {
        VisitorList call = await _client.VisitorSelectAsync(command);
        _database.Visitors.Init(call);

        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task LocationsSelect(CommandLocations command)
    {
      try
      {
        LocationList call = await _client.LocationSelectAsync(command);
        _database.Locations.Init(call);

        Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task PhotosSelect(CommandPhoto command)
    {
      try
      {
        PhotoList call = await _client.PhotoSelectAsync(command);

        _database.PhotoHolder.Update(call.Photos);

        Console.WriteLine("Photos count = " + call.Photos.Count);
        //_database.CaptureDeviceHolder.Update(call.CaptureDevices);

        // Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task PersonUpdate(PersonList persons)
    {
      try
      {
        PersonList call = await _client.PersonUpdateAsync(persons);
        _database.Persons.Update(persons, call);

      //  Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task VisitorUpdate(VisitorList visitors)
    {
      try
      {
        VisitorList call = await _client.VisitorUpdateAsync(visitors);
        _database.Visitors.Update(visitors, call);
        //_database.CaptureDeviceHolder.Update(call.CaptureDevices);

      //  Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task LocationUpdate(LocationList locations)
    {
      try
      {
        LocationList call = await _client.LocationUpdateAsync(locations);
        //_database.CaptureDeviceHolder.Update(call.CaptureDevices);

      //  Console.WriteLine(call.ToString());
      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task AddSocket(SocketConfiguration config)
    {
      try
      {
        Response call = await _client.AddSocketAsync(config);
        //_database.CaptureDeviceHolder.Update(call.CaptureDevices);

        Console.WriteLine(call.ToString());
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
