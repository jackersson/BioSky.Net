using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using BioContracts;
using BioModule.ViewModels;
using BioModule.Model;
using BioData;
using System.Collections;
using Castle.Facilities.TypedFactory;
using System.IO;
using System.Reflection;

namespace BioModule
{
  public class BioModuleInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      try
      {
      

        /*
        //string dbPath = @"F:\C#\BioSkyNetSuccess\BioSky.Net\BioSky.Net\database\BioSkyNet.mdf";
        container.Register(Component.For<IEntityFrameworkConnectionBuilder>()
                 .ImplementedBy<EntityFrameworkConnectionBuilder>()
                 .DependsOn(new
                 {
                   dbConnectionstring = @"F:\C#\BioSkyNetSuccess\BioSky.Net\BioSky.Net\database\BioSkyNet.mdf"
                 })
                 .LifestyleSingleton());

        container.AddFacility<TypedFactoryFacility>()
                 .Register( Component.For<IEntityFrameworkContextFactory>().AsFactory() );

        container.Register(Component.For<BioSkyNetEntities>()
                 .DependsOn(new
                 {
                   modelName = "BioSkyNetDataModel"
                 })
                 .LifestyleTransient());
                 */
        container.Register(Component.For<IBioEngine>()
                 .ImplementedBy<BioEngine>() );

        //IBioSkyNetRepository repo = container.Resolve<IBioSkyNetRepository>();

        container
            .Register(Component.For<UsersViewModel>())
            .Register(Component.For<TabViewModel>())
            .Register(Component.For<VisitorsViewModel>())
            .Register(Component.For<SettingsViewModel>())
            .Register(Component.For<UserPageViewModel>())
            .Register(Component.For<TrackControlViewModel>())               
            .Register(Component.For<IBioModule>().ImplementedBy<BioModuleImpl>());

        System.Console.WriteLine("sdfsdf");
      }
      catch
      {
        System.Console.Write("S");
      }
    }
  }
}

