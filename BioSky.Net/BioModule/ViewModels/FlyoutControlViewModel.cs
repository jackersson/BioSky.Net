﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using Castle.Windsor;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class FlyoutControlViewModel : Conductor<IScreen>.Collection.OneActive, IShowableContent
  {
    public FlyoutControlViewModel( IProcessorLocator locator, string locationId = "")
    {
      _locator = locator;

      Items.Add(_locator.GetProcessor<LocationPageViewModel>());

      _methodInvoker = new FastMethodInvoker();

      FlyoutOpenState = false;
    }
  
    public void ShowContent(Type flyoutPage, object[] args = null)
    {
      ActiveItem = Items.Where(x => x.GetType() == flyoutPage).FirstOrDefault();
      ActiveItem.Activate();
      FlyoutOpenState = true;
      _methodInvoker.InvokeMethod(flyoutPage, "Update", ActiveItem, args);
    }

    private bool _flyoutOpenState;
    public bool FlyoutOpenState
    {
      get { return _flyoutOpenState; }
      set
      {
        if (_flyoutOpenState == value)
          return;

        _flyoutOpenState = value;
        NotifyOfPropertyChange(() => FlyoutOpenState);
      }
    }

    private FastMethodInvoker _methodInvoker;
    private readonly IProcessorLocator _locator;
  }
}
