using System;

using Caliburn.Micro;
using System.Reflection;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace BioShell
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

        _mainContainer.Install(moduleInstaller);

        if (!AssemblySource.Instance.Contains(assembly))
          AssemblySource.Instance.Add(assembly);

        return true;
      }
      catch (Exception)
      {        
        return false;
      }
    }
  }
}
