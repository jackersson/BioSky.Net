using BioContracts;
using BioContracts.Services;
using BioGRPC.Utils;
using BioService;
using Grpc.Core;
using System.Collections.Generic;

namespace BioGRPC
{
  public class ServiceConfiguration : IServiceConfiguration
  {     
    public string FacialService { get; set;  }
       
    public string DatabaseService { get; set; }

    public string FingerprintService { get; set; }    
  }
  

  public class BioServiceManager : IServiceManager
  {
    public BioServiceManager( IProcessorLocator locator )
    {
      _locator      = locator;
      _services     = new List<IService>();
      _networkUtils = new NetworkUtils();
    }

    public void Start(IServiceConfiguration configuration)
    {      
      _faceService        = new BioFacialService     (_locator, configuration.FacialService     );
      _databaseService    = new BioDatabaseService   (_locator, configuration.DatabaseService   );
      _fingerprintService = new BioFingerprintService(_locator, configuration.FingerprintService);
      _services.Add(_faceService);
      _services.Add(_databaseService);
      _services.Add(_fingerprintService);

      foreach (IService service in _services)
        service.Start();
    }

    public void Stop()
    {
      foreach (IService service in _services)
        service.Stop();
    }
    
    private string _macAddress;
    public string MacAddress
    {
      get
      {
        if (string.IsNullOrEmpty(_macAddress))
          _macAddress = _networkUtils.GetMACAddress();
        return _macAddress;
      }
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

    private IFingerprintService _fingerprintService;
    public IFingerprintService FingerprintService
    {
      get { return _fingerprintService; }
    }

    private List<IService> _services;
    private readonly IProcessorLocator _locator     ;
    private readonly NetworkUtils      _networkUtils;

  }
}
