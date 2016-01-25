using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using BioContracts;
using Castle.Facilities.TypedFactory;

namespace BioData
{
  public class BioDataInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      try
      {     
        container.Register(Component.For<IBioSkyNetRepository>().ImplementedBy<BioSkyNetRepository>());        
      }
      catch (Exception ex)
      {
        Console.WriteLine("BioDataInstaller.dll" + ex.Message);
      }
}
  }
}
