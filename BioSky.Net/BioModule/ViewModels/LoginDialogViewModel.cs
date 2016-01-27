using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.Security;
using System.Windows.Controls;
using System.Reflection;

namespace BioModule.ViewModels
{
  public class LoginDialogViewModel : Screen
  {
    public LoginDialogViewModel(IScreen screen , string title = "Login Form")
    {
      Update(title);
      _screen = screen;
    }

    public void Update(string title = "Login Form")
    {
      DisplayName = title;
    }

    public void Register(SecureString password)
    {
      //register logic     

      UserPassword = password;
      DialogResult = true;

      MethodInfo method = _screen.GetType().GetMethod("UpdateUserPassword");
      if (method != null)
        method.Invoke(_screen, new object[] { DialogResult, UserName, UserPassword });

      this.TryClose(true);      
    }
    public void Login(SecureString password)
    {
      //login logic

      UserPassword = password;
      DialogResult = false;

      MethodInfo method = _screen.GetType().GetMethod("UpdateUserPassword");
      if (method != null)
        method.Invoke(_screen, new object[] { DialogResult, UserName, UserPassword });

      this.TryClose(true);
    }
    public void Cancel()
    {      
      
      this.TryClose(false);
    }

    private bool _dialogResult;
    public bool DialogResult
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

    private readonly IScreen _screen;
  }
}
