
using Caliburn.Micro;
using System.Security;
using System.Reflection;

namespace BioModule.ViewModels
{
  public enum LoginDialogStatus
  {
      SignIn
    , Register 
  }
  public class LoginDialogResult
  {
    public SecureString      password;
    public string            userName;
    public LoginDialogStatus status  ;
  }

  public class LoginDialogViewModel : Screen
  {
    public LoginDialogViewModel(IWindowManager windowManager, string title = "Login Form")
    {
      _windowManager = windowManager;
      Update(title);
    }

    public void Update(string title = "Login Form")
    {
      DisplayName = title;
    }

    public void Register(SecureString password)
    {
      UserPassword = password;
      DialogResult = new LoginDialogResult() { status = LoginDialogStatus.Register
                                             , userName = UserName
                                             , password = UserPassword }; ;
      this.TryClose(true);      
    }
    public void Login(SecureString password)
    {
      UserPassword = password;
      DialogResult = new LoginDialogResult() { status = LoginDialogStatus.SignIn
                                             , userName = UserName
                                             , password = UserPassword};

      this.TryClose(true);
    }
    public void Cancel()
    {
      DialogResult = null;
      this.TryClose(false);
    }
    public void Show()
    {
      _windowManager.ShowDialog(this);
    }

    public LoginDialogResult GetDialogResult()
    {
      return DialogResult;
    }

    private LoginDialogResult _dialogResult;
    public LoginDialogResult DialogResult
    {
      get { return _dialogResult; }
      set
      {
        if (_dialogResult != value)
        {
          _dialogResult = value;
          NotifyOfPropertyChange(() => DialogResult);
        }
      }
    }

    private SecureString _userPassword;
    public SecureString UserPassword
    {
      get { return _userPassword; }
      set
      {
        if (_userPassword != value)
        {
          _userPassword = value;
          NotifyOfPropertyChange(() => UserPassword);
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

    private IWindowManager _windowManager;
  }
}
