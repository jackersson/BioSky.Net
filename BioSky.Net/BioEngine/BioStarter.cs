using BioContracts;
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

    public void Run()
    {     
      _serviceManager.Start("192.168.1.178:50051");
      
      RequestData();    
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
        
        BioFaceService.CommandPerson commandPerson = new BioFaceService.CommandPerson();
        await _serviceManager.DatabaseService.PersonRequest(commandPerson);

        BioFaceService.CommandVisitor commandVisitor = new BioFaceService.CommandVisitor();
        await _serviceManager.DatabaseService.VisitorRequest(commandVisitor);

        BioFaceService.CommandAccessDevice commandAccessDevice = new BioFaceService.CommandAccessDevice();
        await _serviceManager.DatabaseService.AccessDeviceRequest(commandAccessDevice);

        BioFaceService.CommandCaptureDevice commandCaptureDevice = new BioFaceService.CommandCaptureDevice();
        await _serviceManager.DatabaseService.CaptureDeviceRequest(commandCaptureDevice);

        BioFaceService.CommandCard commandCard = new BioFaceService.CommandCard();
        await _serviceManager.DatabaseService.CardRequest(commandCard);

        BioFaceService.CommandLocation commandLocation = new BioFaceService.CommandLocation();
        await _serviceManager.DatabaseService.LocationRequest(commandLocation);
        
        BioFaceService.CommandPhoto commandPhoto = new BioFaceService.CommandPhoto();
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
