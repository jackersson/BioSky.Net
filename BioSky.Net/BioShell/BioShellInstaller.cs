using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using BioContracts;
using BioShell.ViewModels;

namespace BioShell
{
  class BioShellInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container
          .Register(Component.For<IWindsorContainer>().Instance(container))
          .Register(Component.For<ShellTabControl>())
          .Register(Component.For<BioShellViewModel>() /*.LifeStyle.Singleton*/)          
          .Register(Component.For<BioModuleLoader>())
          .Register(Component.For<IBioShell>().ImplementedBy<BioShellImpl>());
        
    }
  }
}
