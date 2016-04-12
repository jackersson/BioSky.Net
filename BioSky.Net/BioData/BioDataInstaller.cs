using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using BioContracts;

namespace BioData
{
  public class BioDataInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      try  {     
        container.Register(Component.For<IBioSkyNetRepository>().ImplementedBy<BioSkyNetRepository>());        
      }
      catch (Exception ex)  {
        Console.WriteLine("BioDataInstaller.dll" + ex.Message);
      }
    }
  }
}
