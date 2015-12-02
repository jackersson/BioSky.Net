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

namespace BioModule
{
  public class BioModuleInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container          
          .Register(Component.For<UsersViewModel>())
          .Register(Component.For<TabViewModel>())
          .Register(Component.For<VisitorsViewModel>())
          .Register(Component.For<SettingsViewModel>())
          .Register(Component.For<IBioModule>().ImplementedBy<BioModuleImpl>());
    }
  }
}

