
using Caliburn.Micro;
using BioContracts;
using BioModule.Utils;
using WPFLocalizeExtension.Extensions;
using WPFLocalizeExtension.Providers;
using XAMLMarkupExtensions.Base;
using BioService;
using System.Windows;

namespace BioModule.ViewModels
{
  //Top right corner (User information)
  public class LoginInformationViewModel : Screen, ILoginInformation
  {
    public LoginInformationViewModel(IProcessorLocator locator)
    {
      _locator = locator;
      _dialogsHolder     = _locator.GetProcessor<DialogsHolder>();
      _windowManager     = _locator.GetProcessor<IWindowManager>();

      // TODO Check Error
      //_viewModelSelector = _locator.GetProcessor<ViewModelSelector>();


      LoginPopup = new LoginPopupViewModel();
    }
    
    protected override void OnDeactivate(bool close)
    {
      LoginPopup.Hide();
      base.OnDeactivate(close);
    }
    
    public void UpdateUser(Person user)
    {
      _user = user;

      if (_user == null)
      {
        UserName = LocExtension.GetLocalizedValue<string>("BioModule:lang:SignIn");
        Rights   = "";
        RightsVisibility = Visibility.Collapsed;
      }
      else
      {
        UserName = user.Firstname + " " + user.Lastname;
        Rights   = user.Rights.ToString();
        RightsVisibility = Visibility.Visible;
      }
    }

    public void OpenTabAuthentication()
    {  
      if(_user == null)
        _dialogsHolder.AuthenticationPage.Show();
      else
      {
        _viewModelSelector.ShowContent(ShowableContentControl.TabControlContent
                              , ViewModelsID.UserPage
                              , new object[] { _user });
      }
    }

    public void OpenPopup()
    {
      if (_user != null)
        LoginPopup.Show();
    }

    private Visibility _rightsVisibility;
    public Visibility RightsVisibility
    {
      get { return _rightsVisibility; }
      set
      {
        if (_rightsVisibility != value)
        {
          _rightsVisibility = value;
          NotifyOfPropertyChange(() => RightsVisibility);
        }
      }
    }

    private LoginPopupViewModel _loginPopup;
    public LoginPopupViewModel LoginPopup
    {
      get { return _loginPopup; }
      set
      {
        if (_loginPopup != value)
        {
          _loginPopup = value;
          NotifyOfPropertyChange(() => LoginPopup);
        }
      }
    }

    private string _userName;
    public string UserName
    {
      get { return _userName; }
      set
      {
        if (_userName != value)
        {
          _userName = value;
          NotifyOfPropertyChange(() => UserName);
        }
      }
    }

    private string _rights;
    public string Rights
    {
      get { return _rights; }
      set
      {
        if (_rights != value)
        {
          _rights = value;
          NotifyOfPropertyChange(() => Rights);
        }
      }
    }

    public object LoginInformation
    {
      get
      {
        return this; 
      }
    }

    private Person _user;

    private readonly IProcessorLocator _locator          ;
    private          DialogsHolder     _dialogsHolder    ; 
    private          IWindowManager    _windowManager    ;
    private          ViewModelSelector _viewModelSelector;
  }
}
