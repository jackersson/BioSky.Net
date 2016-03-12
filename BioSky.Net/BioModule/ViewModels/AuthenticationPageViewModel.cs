using BioContracts;
using BioModule.Utils;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.ViewModels
{
  public class AuthenticationPageViewModel : Screen
  {
    public AuthenticationPageViewModel(IProcessorLocator locator)
    {
      DisplayName = "AuthenticationPage";

      _locator           = locator;
      _windowManager     = _locator.GetProcessor<IWindowManager>();
      _viewModelSelector = _locator.GetProcessor<ViewModelSelector>();


      PhotoImage = new PhotoImageViewModel(_locator);
      PhotoImage.SetVisibility(false, false, true, false, false);
    }

    public void Show()
    {
      _windowManager.ShowDialog(this);
    }
    public bool GetDialogResult()
    {
      return true;
    }

    public void OnRegister()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.TabControlContent
                              , ViewModelsID.UserPage
                              , new object[] { null });
      TryClose();
    }
    public void OnLogin()
    {
      TryClose(true);
    }
    protected override void OnActivate()
    {
      PhotoImage.ActivateWith(this);      
      base.OnActivate();    
    }

    protected override void OnDeactivate(bool close)
    {
      PhotoImage.DeactivateWith(this);      
      base.OnDeactivate(close);
    }


    private PhotoImageViewModel _photoImage;
    public PhotoImageViewModel PhotoImage
    {
      get { return _photoImage; }
      set
      {
        if (_photoImage != value)
        {
          _photoImage = value;
          NotifyOfPropertyChange(() => PhotoImage);
        }
      }
    }

    #region Global Variables

    private IProcessorLocator _locator          ;
    private IWindowManager    _windowManager    ;
    private ViewModelSelector _viewModelSelector;

    #endregion



  }
}
