using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Collections.ObjectModel;
using BioContracts;
using System.Windows;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Reflection;

namespace BioModule.ViewModels
{
  public class TabViewModel : Conductor<IScreen>.Collection.OneActive, IShowableContent
  {       
    public TabViewModel( IWindsorContainer container, IBioShell shell)
    {
      _container  = container;
    }

    public void Init()
    {    

    }

    private object InvokeMethod ( Type objectType, string methodName, object source, object[] args = null )
    {
      MethodInfo method = objectType.GetMethod(methodName);
      if (method != null)
        return method.Invoke(source, args);
      return null;
    }

    public void ShowContent(Type tabType, object[] args = null)
    {
      IScreen currentScreen = (IScreen)_container.Resolve(tabType);
      Items.Add(currentScreen);

      ActiveItem = currentScreen;
      currentScreen.Activate();

      //InvokeMethod(tabType, "Update", newTabPage.ScreenViewModel, args);
    }

    private IWindsorContainer _container ;
  }
}
