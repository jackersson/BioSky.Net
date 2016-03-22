using System;

using Caliburn.Micro;
using BioContracts;
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

      foreach(IScreen screen in Items)
      {

        /*
        Type t = typeof(UserPageViewModel);
        currentScreen.GetType();

        if (currentScreen.GetType() == t && screen.GetType() == t && screen.DisplayName != currentScreen.DisplayName)
        {
          UserPageViewModel screen1 = currentScreen as UserPageViewModel;
          UserPageViewModel screen2 = screen        as UserPageViewModel;

          UserPageMode mode1 = screen1.GetUserPageMode();
          UserPageMode mode2 = screen2.GetUserPageMode();

          if (mode1 == mode2 && mode2 == UserPageMode.NewUser)
          {
            ActiveItem = screen;
            screen.Activate();
            return;
          }
        }
          

        if (screen.DisplayName == currentScreen.DisplayName)
        {
          ActiveItem = screen;
          screen.Activate();
          return;
        }*/
        //Console.WriteLine(screen.GetHashCode() + " " + currentScreen.GetHashCode());
        if (screen.GetHashCode() == currentScreen.GetHashCode())
        {
          ActiveItem = screen;
          screen.Activate();
          return;
        }
      }

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
