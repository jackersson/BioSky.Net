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
    public event PersonUpdateHandler   PersonUpdated ;
    public event CardUpdateHandler     CardUpdated   ;
    public event VisitorUpdateHandler  VisitorUpdated;

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

    private void OnCardUpdated(CardList list, Result result)
    {
      if (CardUpdated != null)
        CardUpdated(list, result);
    }

    private void OnVisitorUpdated(VisitorList list, Result result)
    {
      if (VisitorUpdated != null)
        VisitorUpdated(list, result);
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
        OnPersonUpdated(list, call);       
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
        OnCardUpdated(list, call);
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
        //_database.Update<>()

        OnVisitorUpdated(list, call);
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
