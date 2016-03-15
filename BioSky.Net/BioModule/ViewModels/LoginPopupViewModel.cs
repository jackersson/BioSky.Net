using BioContracts;
using Caliburn.Micro;
using System;
using System.Windows;
using System.Windows.Input;

namespace BioModule.ViewModels
{
  public class LoginPopupViewModel : Screen
  {    
    public void Show()
    {      
      IsPopupOpen = true;   
    }

    public void Hide()
    {      
      IsPopupOpen = false;
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
  }
}
