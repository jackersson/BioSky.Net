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

      //return;

     
      container.Register(Component.For<IEntityFrameworkConnectionBuilder>()
               .ImplementedBy<EntityFrameworkConnectionBuilder>()
               .DependsOn(new
               {
                 dbConnectionstring = @"F:\C#\BioSkyNetSuccess\BioSky.Net\BioSky.Net\database\BioSkyNet.mdf"
               })
                 .LifestyleSingleton()
               );

      container.AddFacility<TypedFactoryFacility>()
               .Register(Component.For<IEntityFrameworkContextFactory>().AsFactory());

      container.Register(Component.For<BioSkyNetDataModel>()
               .DependsOn(new
               {
                 modelName = "BioSkyNetDataModel"
               })
               .LifestyleTransient());


      container.Register(Component.For<IBioSkyNetRepository>()
                        .ImplementedBy<BioSkyNetRepository>());


      //container.Register(Component.For<IBioModule>().ImplementedBy<BioDataImpl>());
      //container.Resolve<EntityFrameworkConnectionBuilder>();

      //System.Console.WriteLine("et");
  
    }
  }
}
