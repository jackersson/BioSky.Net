using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using Caliburn.Micro;

using BioContracts;
using BioShell.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace BioShell
{
  class BioShellInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container        
          .Register(Component.For<IWindowManager>().Instance(new WindowManager()))
          .Register(Component.For<IWindsorContainer>().Instance(container))
          .Register(Component.For<IProcessorLocator>().ImplementedBy<ProcessorLocator>());

      container
               .Register(Component.For<BioShellViewModel>())
               .Register(Component.For<BioModuleLoader>())
               .Register(Component.For<BioDataLoader>())
               .Register(Component.For<IBioShell>().ImplementedBy<BioShellImpl>());

    }
  }
}
