using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using BioContracts;
using BioModule.ViewModels;

using BioModule.Utils;
using System.Globalization;
using System.Threading;
using BioModule.ResourcesLoader;

using Caliburn.Micro;


namespace BioModule
{
  public class BioModuleImpl : IBioModule
  {
    private readonly IProcessorLocator _locator;

    public BioModuleImpl( IProcessorLocator locator )     
    {
      _locator = locator;
    }

    public void Init()
    {
      
      IBioStarter starter = _locator.GetProcessor<IBioStarter>();
      starter.Run();

      ConverterInitializer initializer = new ConverterInitializer(_locator);     
      
      ViewModelSelector selector = _locator.GetProcessor<ViewModelSelector>();
      selector.ShowContent( ShowableContentControl.TabControlContent,  ViewModelsID.HomePage);
      
      IBioShell bioShell = _locator.GetProcessor<IBioShell>();
       

      bioShell.TabControl       = _locator.GetProcessor<TabViewModel>();
      bioShell.FlyoutControl    = _locator.GetProcessor<FlyoutControlViewModel>();
      bioShell.ToolBar          = _locator.GetProcessor<ToolBarViewModel>();
      bioShell.MainMenu         = _locator.GetProcessor<MainMenuViewModel>();
      bioShell.ProgressRing     = _locator.GetProcessor<INotifier>().LoadingViewModel;
      bioShell.LoginInformation = _locator.GetProcessor<ILoginInformation>().LoginInformation;
    }
  }
}
