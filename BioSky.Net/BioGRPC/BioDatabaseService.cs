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
    }
    
    
    public async Task CaptureDeviceRequest(CommandCaptureDevice command)
    {
      try
      {
        CaptureDeviceList call = await _client.CaptureDeviceSelectAsync(command);
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

    private readonly IProcessorLocator                      _locator;
    private readonly BioFaceDetector.IBioFaceDetectorClient _client;
    
  }
}
