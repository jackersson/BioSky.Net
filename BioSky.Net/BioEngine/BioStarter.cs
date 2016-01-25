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
    BioStarter(IProcessorLocator locator)
    {
      _bioEngine      = locator.GetProcessor<IBioEngine>();
      _serviceManager = locator.GetProcessor<IServiceManager>();
    }

    public void Run()
    {
      _serviceManager.Start("127.0.0.1:50051");
    }

    private IServiceManager _serviceManager;
    private IBioEngine      _bioEngine     ;
  }
}
