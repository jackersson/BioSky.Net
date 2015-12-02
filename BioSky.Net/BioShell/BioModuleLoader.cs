using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Reflection;
using Castle.Windsor;
using Castle.Windsor.Installer;


using BioContracts;


namespace BioShell
{
  class BioModuleLoader
  {
    private readonly IWindsorContainer _mainContainer;

    public BioModuleLoader(IWindsorContainer mainContainer)
    {
      _mainContainer = mainContainer;
    }

    public IBioModule LoadModule(Assembly assembly)
    {
      try
      {
        var moduleInstaller = FromAssembly.Instance(assembly);

        var modulecontainer = new WindsorContainer();

        _mainContainer.AddChildContainer(modulecontainer);

        modulecontainer.Install(moduleInstaller);

        var module = modulecontainer.Resolve<IBioModule>();

        if (!AssemblySource.Instance.Contains(assembly))
          AssemblySource.Instance.Add(assembly);

        return module;
      }
      catch (Exception ex)
      {
        //TODO: good exception handling 
        return null;
      }
    }
  }
}
