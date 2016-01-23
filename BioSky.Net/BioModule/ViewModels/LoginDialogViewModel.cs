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

namespace BioModule.ViewModels
{
  public class LoginDialogViewModel : Screen
  {
    public LoginDialogViewModel(string title = "Login Form")
    {
      Update(title);
    }

    public void Update(string title = "Login Form")
    {
      DisplayName = title;
    }

    public void Register(SecureString password)
    {
      //register logic
      PasswordBox pb = new PasswordBox();
      
      Console.WriteLine(password);
      UserPassword = password;
      DialogResult = true;      
      this.TryClose(DialogResult);      
    }
    public void Login(SecureString password)
    {
      //login logic
      Console.WriteLine(password);
      UserPassword = password;
      DialogResult = true;
      this.TryClose(DialogResult);
    }
    public void Cancel()
    {      
      DialogResult = false;
      this.TryClose(DialogResult);
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
  }
}
