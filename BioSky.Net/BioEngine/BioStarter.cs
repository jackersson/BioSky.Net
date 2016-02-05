using BioContracts;
using BioGRPC;
using Grpc.Core;
using System;

namespace BioEngine
{
  public class BioStarter : IBioStarter
  {
    public BioStarter(IProcessorLocator locator)
    {
      _bioEngine      = locator.GetProcessor<IBioEngine>();
      _serviceManager = locator.GetProcessor<IServiceManager>();
    }

    public async void Run()
    {
      ServiceConfiguration configuration = new ServiceConfiguration();
      configuration.FacialService   = "192.168.1.127:50051";
      configuration.DatabaseService = "192.168.1.178:50051";

      _serviceManager.Start(configuration);
      
      RequestData();

      try
      {
        await _serviceManager.FaceService.Configurate(configuration);
      }
      catch (RpcException e)
      {
        Console.WriteLine("RPC failed " + e);
        throw;
      }
    }

    public void Stop()
    {
      _serviceManager.Stop();
      _bioEngine.Stop();
    }

 

    public async void RequestData()
    {
      try
      {
        
        BioService.CommandPerson commandPerson = new BioService.CommandPerson();
        await _serviceManager.DatabaseService.PersonRequest(commandPerson);

        BioService.CommandVisitor commandVisitor = new BioService.CommandVisitor();
        await _serviceManager.DatabaseService.VisitorRequest(commandVisitor);

        BioService.CommandAccessDevice commandAccessDevice = new BioService.CommandAccessDevice();
        await _serviceManager.DatabaseService.AccessDeviceRequest(commandAccessDevice);

        BioService.CommandCaptureDevice commandCaptureDevice = new BioService.CommandCaptureDevice();
        await _serviceManager.DatabaseService.CaptureDeviceRequest(commandCaptureDevice);

        BioService.CommandCard commandCard = new BioService.CommandCard();
        await _serviceManager.DatabaseService.CardRequest(commandCard);

        BioService.CommandLocation commandLocation = new BioService.CommandLocation();
        await _serviceManager.DatabaseService.LocationRequest(commandLocation);
        
        BioService.CommandPhoto commandPhoto = new BioService.CommandPhoto();
        await _serviceManager.DatabaseService.PhotoRequest(commandPhoto);
        
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private IServiceManager _serviceManager;
    private IBioEngine      _bioEngine     ;
  }
}
