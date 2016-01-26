using BioContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioGRPC;



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
      //_serviceManager.Start("192.168.1.127:50051");
      _serviceManager.Start("192.168.1.178:50051");

    }

    private IServiceManager _serviceManager;
    private IBioEngine      _bioEngine     ;
  }
}
