using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;

namespace BioGRPC
{
  public class ServiceConfiguration : IServiceConfiguration
  {
    public ServiceConfiguration(string database_service, string facial_service )
    {

    }
    public ServiceConfiguration()
    {

    }

    private string _facialService;
    public string FacialService
    {
      get { return _facialService; }
      set
      {
        if (_facialService != value)
          _facialService = value;

      }
    }

    private string _databaseService;
    public string DatabaseService
    {
      get { return _databaseService; }
      set
      {
        if ( _databaseService != value)        
          _databaseService = value;
        
      }
    }

    //string database = "169.254.14.74:50051"

  }
  

  public class BioServiceManager : IServiceManager
  {
    public BioServiceManager( IProcessorLocator locator )
    {
      _locator = locator;    
    }

    public void Start(IServiceConfiguration configuration)
    {      
      IBioEngine bioEngine = _locator.GetProcessor<IBioEngine>();

      _databaseClientChannel = new Channel(configuration.DatabaseService, ChannelCredentials.Insecure);

      _facialClientChannel   = new Channel(configuration.FacialService  , ChannelCredentials.Insecure);
       
      BioService.BiometricFacialSevice.IBiometricFacialSeviceClient   facialClient   = BiometricFacialSevice.NewClient(_facialClientChannel);
      BioService.BiometricDatabaseSevice.IBiometricDatabaseSeviceClient databaseClient = BiometricDatabaseSevice.NewClient(_databaseClientChannel);

      _faceService     = new BioFacialService  (_locator, facialClient  );
      _databaseService = new BioDatabaseService(_locator, databaseClient);

     
    }
    public void Stop()
    {
      if (_databaseClientChannel != null)
        _databaseClientChannel.ShutdownAsync().Wait();

      if (_facialClientChannel != null)
        _facialClientChannel.ShutdownAsync().Wait();
    }


    private IFaceService _faceService;
    public IFaceService FaceService
    {
      get { return _faceService; }
    }

    private IDatabaseService _databaseService;
    public IDatabaseService DatabaseService
    {
      get { return _databaseService; }
    }  

    private Channel _databaseClientChannel;
    private Channel _facialClientChannel  ;

    private readonly IProcessorLocator _locator;

  }
}
