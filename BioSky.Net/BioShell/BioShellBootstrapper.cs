using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Castle.Core.Internal;
using Castle.Windsor;

using BioShell.ViewModels;

namespace BioShell
{
  public class BioShellBootstrapper : BootstrapperBase
  {
    private readonly IWindsorContainer _container = new WindsorContainer();

    public BioShellBootstrapper()
    {
      Initialize();
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
      var loader     = _container.Resolve<BioModuleLoader>();
      var dataloader = _container.Resolve<BioDataLoader>();

      var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                
      dataloader.LoadData(Assembly.LoadFile(exeDir + @"\BioData.dll"));
      dataloader.LoadData(Assembly.LoadFile(exeDir + @"\BioAccessDevice.dll"));

      var pattern = "BioModule.dll";

      Directory
          .GetFiles(exeDir, pattern)          
          .Select(Assembly.LoadFrom)
          .Select(loader.LoadModule)
          .Where(module => module != null)
          .ForEach(module => module.Init());

      DisplayRootViewFor<BioShellViewModel>();
    }

    protected override void Configure()
    {
      _container.Install(new BioShellInstaller());
    }

    protected override object GetInstance(Type service, string key)
    {
      return string.IsNullOrWhiteSpace(key)

          ? _container.Kernel.HasComponent(service)
              ? _container.Resolve(service)
              : base.GetInstance(service, key)

          : _container.Kernel.HasComponent(key)
              ? _container.Resolve(key, service)
              : base.GetInstance(service, key);
    }
  }
}
