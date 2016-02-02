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
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class TabViewModel : Conductor<IScreen>.Collection.OneActive, IShowableContent
  {       
    public TabViewModel(IProcessorLocator locator )
    {
      _locator = locator;

      _methodInvoker = new FastMethodInvoker();
    }      

    public void ShowContent(Type tabType, object[] args = null)
    {
      object scr = _locator.GetProcessor(tabType);
      if (!(scr is IScreen))
        return;

      IScreen currentScreen = (IScreen)scr;
      Items.Add(currentScreen);

      ActiveItem = currentScreen;
      currentScreen.Activate();

      _methodInvoker.InvokeMethod(tabType, "Update", ActiveItem, args);      
    }

    public void OnSelectionChanged()
    {
      if (ActiveItem != null)
        ActiveItem.Activate();
    }




    private FastMethodInvoker _methodInvoker;
    private IProcessorLocator _locator ;
  }
}
