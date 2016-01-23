using BioFaceService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioGRPC
{
  public class BioFaceServiceManager
  {
    
    public void Start()
    {
      _clientChannel = new Channel("127.0.0.1:50051", Credentials.Insecure);
      _bioFaceClient = new BioFaceClient(BioFaceDetector.NewClient(_clientChannel));  
    }

    public BioFaceClient FaceClient
    {
      get { return _bioFaceClient; }
    }

    public void Stop()
    {
      _clientChannel.ShutdownAsync().Wait();
    }

    private BioFaceClient _bioFaceClient;
    private Channel       _clientChannel;

  }
}
