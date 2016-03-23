using BioContracts;
using Caliburn.Micro;
using System;
using System.Windows;
using System.Windows.Input;

namespace BioModule.ViewModels
{
  public class LoginPopupViewModel : Screen
  {   
    
    public LoginPopupViewModel(IProcessorLocator locator)
    {
      _locator   = locator;      
    }
    public void Show()
    {      
      IsPopupOpen = true;   
    }

    public void Hide()
    {      
      IsPopupOpen = false;
    }   
    
    public void OnSignOut()
    {
      if(_bioEngine == null)
        _bioEngine = _locator.GetProcessor<IBioEngine>();

      _bioEngine.AuthenticatedPerson = null;
    }

    private bool _isPopupOpen;
    public bool IsPopupOpen
    {
      get { return _isPopupOpen; }
      set
      {
        if (_isPopupOpen != value)
        {
          _isPopupOpen = value;
          NotifyOfPropertyChange(() => IsPopupOpen);
        }
      }
    }

    private readonly IProcessorLocator _locator  ;
    private          IBioEngine        _bioEngine;
  }
}
