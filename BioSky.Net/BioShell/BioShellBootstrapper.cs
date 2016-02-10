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
using System.Windows.Input;

using System.Threading;
using System.Globalization;
using BioContracts;

namespace BioShell
{
  public class BioShellBootstrapper : BootstrapperBase
  {
    private readonly IWindsorContainer _container = new WindsorContainer();

    public BioShellBootstrapper()
    {
      Initialize();
    }


    protected override void OnExit(object sender, EventArgs e)
    {
      Console.WriteLine("Exit");
      IBioStarter starter = _container.Resolve<IBioStarter>();
      if (starter != null)
      {
       // Thread.Sleep(1000);
        starter.Stop();
      }

      Thread.Sleep(5000);
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
      
      var loader     = _container.Resolve<BioModuleLoader>();
      var dataloader = _container.Resolve<BioDataLoader>();

      var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      try
      {
        dataloader.LoadData(Assembly.LoadFile(exeDir + @"\BioData.dll"));
        dataloader.LoadData(Assembly.LoadFile(exeDir + @"\BioAccessDevice.dll"));
        dataloader.LoadData(Assembly.LoadFile(exeDir + @"\BioGRPC.dll"));
        dataloader.LoadData(Assembly.LoadFile(exeDir + @"\BioEngine.dll"));
       
        var pattern = "BioModule.dll";

        Directory
            .GetFiles(exeDir, pattern)
            .Select(Assembly.LoadFrom)
            .Select(loader.LoadModule)
            .Where(module => module != null)
            .ForEach(module => module.Init());
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }

      DisplayRootViewFor<BioShellViewModel>();
    }

    protected override void Configure()
    {

      Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU"); 


      MessageBinder.SpecialValues.Add("$mouseselecteditem", (context) =>
      {
        if (context.EventArgs is MouseButtonEventArgs)
        {
          var eventArg = context.EventArgs as MouseButtonEventArgs;
          var frameworkElement = eventArg.OriginalSource as FrameworkElement;

          if (frameworkElement != null)
            return frameworkElement.DataContext;
        }

        return null;
      });

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
