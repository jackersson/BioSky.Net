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
        BioService.CommandPersons commandPerson = new BioService.CommandPersons();
        await _serviceManager.DatabaseService.PersonsSelect(commandPerson);

        BioService.CommandVisitors commandVisitor = new BioService.CommandVisitors();
        await _serviceManager.DatabaseService.VisitorsSelect(commandVisitor);

        BioService.CommandLocations commandLocation = new BioService.CommandLocations();
        await _serviceManager.DatabaseService.LocationsSelect(commandLocation);

        BioService.CommandPhoto commandPhoto = new BioService.CommandPhoto();
        await _serviceManager.DatabaseService.PhotosSelect(commandPhoto);

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
