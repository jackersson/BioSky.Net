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
        container.Register(Component.For<ITrackLocationEngine>()
                 .ImplementedBy<TrackLocationEngine>());


        container.Register(Component.For<IBioEngine>()
                 .ImplementedBy<BioEngine>() );

        container            
            .Register(Component.For<VisitorsViewModel>())
            .Register(Component.For<SettingsViewModel>())           
            .Register(Component.For<TrackControlViewModel>())
            .Register(Component.For<UserPageViewModel>().LifestyleTransient());


        container.Register(Component.For<IWindsorContainer>().Instance(container));
        container.Register(Component.For<ITabControl>().ImplementedBy<TabViewModel>());
        container.Register(Component.For<IFlyoutControl>().ImplementedBy<FlyoutControlViewModel>());
        container.Register(Component.For<ViewModelSelector>().LifeStyle.Singleton);        

        container.Register(Component.For<UsersViewModel>().LifeStyle.Singleton);

        container.Register(Component.For<MainMenuViewModel>().LifeStyle.Singleton);
        container.Register(Component.For<ToolBarViewModel>().LifeStyle.Singleton);

        container.Register(Component.For<IBioModule>().ImplementedBy<BioModuleImpl>());
      }
      catch
      {
       
      }
    }
  }
}

