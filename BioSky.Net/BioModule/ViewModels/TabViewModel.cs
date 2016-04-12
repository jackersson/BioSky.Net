using System;

using Caliburn.Micro;
using BioContracts;
using BioModule.Utils;
using System.Windows.Controls;

namespace BioModule.ViewModels
{
  public class TabViewModel : Conductor<IScreen>.Collection.OneActive, IShowableContent
  {       
    public TabViewModel(IProcessorLocator locator )
    {
      _locator = locator;

      _methodInvoker = new FastMethodInvoker();

      this.PropertyChanged += TabViewModel_PropertyChanged;
    }

    private void TabViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (ActiveItem != null && !ActiveItem.IsActive)
      {
         ActivateItem(ActiveItem);
         ActiveItem.Activate();
      }     
    }

    public void ShowContent(Type tabType, object[] args = null)
    {
      object scr = _locator.GetProcessor(tabType);
      if (!(scr is IScreen))
        return;

      /*IScreen currentScreen = (IScreen)scr;
      _methodInvoker.InvokeMethod(tabType, "Update", currentScreen, args);
      foreach (IScreen screen in Items)
      {
        if (screen.GetHashCode() == currentScreen.GetHashCode( ))
        {
          ActivateItem(screen);
          screen.Activate();
          return;
        }        
      }     
      
      ActivateItem(currentScreen);
      currentScreen.Activate();   */

      IScreen screen = scr as IScreen;


      Items.Add(screen);
      ActivateItem(screen);
      screen.Activate();

      _methodInvoker.InvokeMethod(tabType, "Update", screen, args);
    }


    private FastMethodInvoker _methodInvoker;
    private IProcessorLocator _locator ;
  }
}
