using BioUITest.ViewModels;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BioUITest
{
  public class AppBootstrapper : BootstrapperBase
  {
   
    public AppBootstrapper()
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
      Initialize();
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
    
      DisplayRootViewFor<VideoStreamViewModel>();
    }
   
  }
}
