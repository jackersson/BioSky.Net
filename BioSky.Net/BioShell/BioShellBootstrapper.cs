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
using BioShell.Utils;

namespace BioShell
{
  public class BioShellBootstrapper : BootstrapperBase
  {
    public BioShellBootstrapper()
    {
      Initialize();
    }
    
    protected override void OnExit(object sender, EventArgs e)
    {
      try
      {        
         IBioStarter starter = _container.Resolve<IBioStarter>();
         if (starter != null)                       
             starter.Stop();          

         Thread.Sleep(5000);
      }
      catch (Exception ex)
      {
        Notifier.Notify(ex);
      }
    }
    
    protected override void OnStartup(object sender, StartupEventArgs e)
    {
      try
      {
       

        var loader     = _container.Resolve<BioModuleLoader>();
        var dataloader = _container.Resolve<BioDataLoader>();

        var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

      
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
      catch (Exception ex) {
        Notifier.Notify(ex);
      }      

      DisplayRootViewFor<BioShellViewModel>();
      _container.Register(Castle.MicroKernel.Registration.Component.For<Window>().Instance(Application.Current.MainWindow));
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

    private readonly IWindsorContainer _container = new WindsorContainer();

    private INotifier _notifier;
    private INotifier Notifier
    {
      get
      {
        if (_notifier == null)
          _notifier = _container.Resolve<INotifier>();

        return _notifier == null ? new BioNotifier() : _notifier;
      }
      set
      {
        if (_notifier != value && value != null)
          _notifier = value;

      }
    }
  }
}
