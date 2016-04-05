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
      _bioEngine         = _locator.GetProcessor<IBioEngine>();
      
      PhotoImage = new BioImageViewModel(_locator);
      PhotoImage.SetVisibility(false, false, true, false, false);
    }
    public void Show()
    {
      LoginData = "";
      ShowCloseButton = (_bioEngine.AuthenticatedPerson != null) ? true : false;
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
      _bioEngine.AuthenticatedPerson = new BioService.Person { Firstname = LoginData
                                                             , Rights = BioService.Person.Types.Rights.Supervisor };
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


    private BioImageViewModel _photoImage;
    public BioImageViewModel PhotoImage
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

    private bool _showCloseButton;
    public bool ShowCloseButton
    {
      get { return _showCloseButton; }
      set
      {
        if (_showCloseButton != value)
        {
          _showCloseButton = value;
          NotifyOfPropertyChange(() => ShowCloseButton);
        }
      }
    }

    private string _loginData;
    public string LoginData
    {
      get { return _loginData; }
      set
      {
        if (_loginData != value)
        {
          _loginData = value;
          NotifyOfPropertyChange(() => LoginData);
        }
      }
    }

    #region Global Variables

    private IProcessorLocator _locator          ;
    private IWindowManager    _windowManager    ;
    private ViewModelSelector _viewModelSelector;
    private IBioEngine        _bioEngine        ;

    #endregion



  }
}
