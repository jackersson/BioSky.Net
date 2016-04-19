using BioContracts;
using BioGRPC;
using Grpc.Core;
using System;

using WPFLocalizeExtension.Engine;
using System.Globalization;
using BioContracts.Services;


namespace BioEngine
{
  public class BioStarter : IBioStarter
  {
    public BioStarter(IProcessorLocator locator)
    {
      _locator = locator;
       _bioEngine      = locator.GetProcessor<IBioEngine>();
      _notifier       = locator.GetProcessor<INotifier>();
      _serviceManager = locator.GetProcessor<IServiceManager>();

      _localStorage   = _bioEngine.Database().LocalStorage;
    }

    public async void Run()
    {
      ServiceConfiguration configuration = new ServiceConfiguration();

      configuration.FacialService   = _localStorage.GetParametr(ConfigurationParametrs.FaceServiceAddress);
      configuration.DatabaseService = _localStorage.GetParametr(ConfigurationParametrs.DatabaseServiceAddress);
      
      _serviceManager.Start(configuration);
      
      RequestData();
      Setlanguage();
      /*
      try
      {
        await _serviceManager.FaceService.Configurate(configuration);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
      */
    }

    public void Setlanguage()
    {
      LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
      LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(_localStorage.GetParametr(ConfigurationParametrs.Language));
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

        IDatabaseService service = _serviceManager.DatabaseService;
        service.Subscribe();
          /*
        BioService.QueryPersons commandPerson = new BioService.QueryPersons();
        await service.PersonDataClient.Select(commandPerson);
        
        BioService.QueryVisitors commandVisitor = new BioService.QueryVisitors();
        await service.VisitorDataClient.Select(commandVisitor);
        */

        BioService.QueryLocations commandLocation = new BioService.QueryLocations();
        await service.LocationDataClient.Select(commandLocation);
        
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
    private readonly IProcessorLocator _locator;
    private readonly INotifier _notifier;
  }
}
