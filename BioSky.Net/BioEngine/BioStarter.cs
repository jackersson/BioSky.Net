using BioContracts;
using BioGRPC;
using Grpc.Core;
using System;

using WPFLocalizeExtension.Engine;
using System.Globalization;
using BioContracts.Services;
using System.Threading.Tasks;

namespace BioEngine
{
  public class BioStarter : IBioStarter
  {
    public BioStarter(IProcessorLocator locator)
    {
      _bioEngine      = locator.GetProcessor<IBioEngine>();
      _notifier       = locator.GetProcessor<INotifier>();
      _serviceManager = locator.GetProcessor<IServiceManager>();

      _localStorage   = _bioEngine.Database().LocalStorage;
    }

    public async void Run()
    {
      ServiceConfiguration configuration = new ServiceConfiguration();

      configuration.FacialService   = _localStorage.FaceServiceStoragePath;
      configuration.DatabaseService = _localStorage.DatabaseServiceStoragePath;
      
      _serviceManager.Start(configuration);
      
      Task ta = new Task(RequestData);
      ta.Start();
      Setlanguage();
      try
      {
        await _serviceManager.FaceService.Configurate(configuration);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public void Setlanguage()
    {
      LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
      LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(_localStorage.Language);
    }

    public void Stop()
    {
      _serviceManager.Stop();
      _bioEngine.Stop();
    } 

    public void RequestData()
    {
      try
      {
       // return;
        IDatabaseService service = _serviceManager.DatabaseService;
        BioService.QueryPersons commandPerson = new BioService.QueryPersons();
        service.PersonDataClient.Select(commandPerson);

        //BioService.QueryPersons commandPerson2 = new BioService.QueryPersons();
        //service.PersonDataClient.Select(commandPerson2);
        //return;
       // BioService.QueryVisitors commandVisitor = new BioService.QueryVisitors();
       // service.VisitorDataClient.Select(commandVisitor);

         BioService.QueryLocations commandLocation = new BioService.QueryLocations();
         service.LocationDataClient.Select(commandLocation);

        /* not here
        BioService.CommandPhoto commandPhoto = new BioService.CommandPhoto();
        await service.PhotoDataClient.Select(commandPhoto);
        */
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }
    }

    private IServiceManager _serviceManager;
    private IBioEngine      _bioEngine     ;
    private ILocalStorage   _localStorage  ;

    private readonly INotifier _notifier;
  }
}
