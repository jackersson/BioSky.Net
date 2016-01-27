using BioContracts;
using BioContracts.Services;
using BioFaceService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioGRPC
{
  public class BioServiceManager : IServiceManager
  {
    public BioServiceManager( IProcessorLocator locator )
    {
      _locator = locator;
    }

    public void Start(string server_address = "127.0.0.1:50051")
    {
      IBioEngine bioEngine = _locator.GetProcessor<IBioEngine>();
      //string server_address2 = "169.254.14.74:50051";
      Channel _clientChannel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

      var client = BioFaceDetector.NewClient(_clientChannel);

      _bioFaceService     = new BioFaceService    (_locator, client);
      _bioDatabaseService = new BioDatabaseService(_locator, client);
    }
    public void Stop()
    {
     // _clientChannel.ShutdownAsync().Wait();
    }


    private IFaceService _bioFaceService;
    public IFaceService FaceService
    {
      get { return _bioFaceService; }
    }

    private IDatabaseService _bioDatabaseService;
    public IDatabaseService DatabaseService
    {
      get { return _bioDatabaseService; }
    }  

   // private Channel       _clientChannel;

    private readonly IProcessorLocator _locator;

  }
}
