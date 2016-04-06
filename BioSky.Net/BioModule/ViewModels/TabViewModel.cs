using System;

using Caliburn.Micro;
using BioContracts;
using BioModule.Utils;
using BioService;
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
      Console.WriteLine(ActiveItem);

      
      if (ActiveItem != null)
      {
        if (!ActiveItem.IsActive)
        {
          ActivateItem(ActiveItem);
          ActiveItem.Activate();
        }
      }
      
    }

    public void ShowContent(Type tabType, object[] args = null)
    {
      object scr = _locator.GetProcessor(tabType);
      if (!(scr is IScreen))
        return;

      IScreen currentScreen = (IScreen)scr;

      int currentHashCode = currentScreen.GetHashCode();
/*
      if (args != null)
      {
        foreach (object item in args)
        {
          if (item is Person)
            currentHashCode = (item as Person).Id.GetHashCode();
        }
      }

      foreach(IScreen screen in Items)
      {
        Console.WriteLine(screen.GetHashCode() + " " + currentHashCode);
        if (screen.GetHashCode() == currentHashCode)
        {
          //ActiveItem = screen;
          screen.Activate();
          return;
        }
      }
      */
      Items.Add(currentScreen);

      //this.

      //ActiveItem = currentScreen;
      ActivateItem(currentScreen);
      currentScreen.Activate();
      _methodInvoker.InvokeMethod(tabType, "Update", ActiveItem, args);      
    }

    
    public void OnSelectionChanged(object activeItem)
    {
      /*
      //ChangeActiveItem()
      if (ActiveItem != null)
      {
        if (!ActiveItem.IsActive)
        {
          ActivateItem(ActiveItem);
          ActiveItem.Activate();
        }
      }
      */
    }
    
  

    private FastMethodInvoker _methodInvoker;
    private IProcessorLocator _locator ;
  }
}
