using BioContracts;
using BioContracts.Services;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioGRPC
{
  public class BioGrpcInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      try
      { 
        container.Register(Component.For<IServiceManager>().ImplementedBy<BioServiceManager>());       
      }
      catch (Exception ex)
      {
        Console.WriteLine("BioGrpc.dll" + ex.Message);
      }
}
  }
}
