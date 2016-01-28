using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using BioFaceService;
using Grpc.Core;
using BioContracts.Services;

namespace BioGRPC
{
  public class BioDatabaseService : IDatabaseService
  {      
    public BioDatabaseService(IProcessorLocator locator, BioFaceDetector.IBioFaceDetectorClient client)
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
        _database.CaptureDevices = call;
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
        _database.AccessDevices = call;
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
        _database.Locations = call;
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
        _database.Photos = call;
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
        _database.Cards = call;
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
        _database.Visitors = call;
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
        _database.Persons = call;
        
        Console.WriteLine(call.ToString());

        //call.Persons[0].Country = "Ucrania";
        //call.Persons[0].Dbstate = DbState.Update;

        //BioFaceService.CommandPerson cmd1 = new BioFaceService.CommandPerson();
        //await PersonUpdateRequest(call);

      }
      catch (RpcException e)
      {
        Log("RPC failed " + e);
        throw;
      }
    }

    public async Task PersonUpdateRequest(PersonList command)
    {
      try
      {
        Result call = await _client.PersonUpdateAsync(command);        
        Console.WriteLine(call.ToString());

        foreach (ResultPair ss in call.Status)
        {
          Console.WriteLine(ss.Status);
         
        }
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

    private readonly IProcessorLocator                      _locator;
    private readonly BioFaceDetector.IBioFaceDetectorClient _client;
    private readonly IBioSkyNetRepository                   _database; 
  }
}
