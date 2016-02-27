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
//using BioModule.Model;
using BioModule.Utils;
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
        container.Register(Component.For<ITrackLocationEngine>()
                 .ImplementedBy<TrackLocationEngine>());


        container.Register(Component.For<IBioEngine>()
                 .ImplementedBy<BioEngine>() );
         
        */

        //container.Resolve();

        container
            .Register(Component.For<VisitorsViewModel>())
            .Register(Component.For<LocationPageViewModel>())
            .Register(Component.For<GeneralSettingsPageViewModel>())
            .Register(Component.For<TrackControlViewModel>())
            .Register(Component.For<UserPageViewModel>().LifestyleTransient())
            .Register(Component.For<DialogsHolder>());



        //container.Register(Component.For<IWindsorContainer>().Instance(container));
        container.Register(Component.For<TabViewModel>());
        container.Register(Component.For<FlyoutControlViewModel>());
        container.Register(Component.For<ViewModelSelector>().LifeStyle.Singleton);        

        container.Register(Component.For<UsersViewModel>().LifeStyle.Singleton);

        container.Register(Component.For<MainMenuViewModel>().LifeStyle.Singleton);
        container.Register(Component.For<ToolBarViewModel>().LifeStyle.Singleton);
        container.Register(Component.For<LoginInformationViewModel>().LifeStyle.Singleton);

       
        

        container.Resolve<IProcessorLocator>().Init(container);

       // IBioStarter starter = container.Resolve<IBioStarter>();
        //starter.Run();

        //var test =  container.Resolve<IBioEngine>();
        //IBioStarter starter = container.Resolve<IBioStarter>();
        //container.Register(Component.For<IWindsorContainer>().Instance(container));
       // container.Register(Component.For<IProcessorLocator>().ImplementedBy<ProcessorLocator>());



        container.Register(Component.For<IBioModule>().ImplementedBy<BioModuleImpl>());
      }
      catch ( Exception ex )
      {
        Console.WriteLine("BioGrpc.dll" + ex.Message);
      }
    }
  }
}

