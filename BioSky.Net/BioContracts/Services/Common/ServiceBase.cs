using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services.Common
{
  public abstract class ServiceBase : IService
  {
    public void Start()
    {
      Start(Address);
    }

    public void Start(string address)
    {
      if (string.IsNullOrEmpty(address))
        return;

      Address = address;

      Stop();

      Channel = new Channel(Address, ChannelCredentials.Insecure);

      CreateClient();
    }

    protected abstract void CreateClient();   

    public void Stop()
    {
      if (Channel != null)
        Channel.ShutdownAsync().Wait();
    }

    protected string  Address { get; set; }
    protected Channel Channel;
  }
}
