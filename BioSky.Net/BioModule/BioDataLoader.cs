using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Reflection;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace BioModule
{
  class BioDataLoader
  {
    private readonly IWindsorContainer _mainContainer;

    public BioDataLoader(IWindsorContainer mainContainer)
    {
      _mainContainer = mainContainer;
    }

    public bool LoadData(Assembly assembly)
    {
      try
      {
        var moduleInstaller = FromAssembly.Instance(assembly);

        //var modulecontainer = new WindsorContainer();

        //_mainContainer.AddChildContainer(modulecontainer);

        _mainContainer.Install(moduleInstaller);
        
        if (!AssemblySource.Instance.Contains(assembly))
          AssemblySource.Instance.Add(assembly);

       

        return true;
      }
      catch (Exception)
      {
        //TODO: good exception handling 
        return false;
      }
    }
  }
}
