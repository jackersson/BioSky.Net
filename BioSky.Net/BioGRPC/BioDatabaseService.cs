using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using BioFaceService;
using Grpc.Core;
using BioContracts.Services;

using System.Collections.ObjectModel;

namespace BioGRPC
{
  public class BioDatabaseService : IDatabaseService
  {
    public event PersonUpdateHandler PersonUpdated;

    public BioDatabaseService(IProcessorLocator locator, BioFaceDetector.IBioFaceDetectorClient client)
    {
      _client    = client;
      _locator   = locator;

      _database = _locator.GetProcessor<IBioSkyNetRepository>();
    }
    
    private void OnPersonUpdated(PersonList list, Result result)
    {
      if (PersonUpdated != null)
        PersonUpdated(list, result);
    }
    
    public async Task CaptureDeviceRequest(CommandCaptureDevice command)
    {
      try
      {
        CaptureDeviceList call = await _client.CaptureDeviceSelectAsync(command);
        _database.UpdateCaptureDeviceSet(call);
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
        _database.UpdateAccessDeviceSet(call);
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
        _database.UpdateLocationSet(call);
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
        _database.UpdatePhotoSet(call);
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
        _database.UpdateCardSet(call);
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
        _database.UpdateVisitorSet(call);
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
        _database.UpdatePersonSet(call);
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
        //Console.WriteLine(call.ToString());

        OnPersonUpdated(list, call);

        //foreach (ResultPair ss in call.Status)        
          //Console.WriteLine(ss.Status);         
        
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
