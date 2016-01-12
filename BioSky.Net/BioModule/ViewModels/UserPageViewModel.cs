using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioContracts;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

using BioModule.Model;
using BioModule.Utils;
using BioData;
using System.Reflection;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace BioModule.ViewModels
{
  enum UserPageMode
  {
     NewUser
   , ExistingUser
  }
  public class UserPageViewModel : Conductor<IScreen>.Collection.OneActive
  {   
    public UserPageViewModel(IBioEngine bioEngine, IWindowManager windowManager) : base()
    {
      _bioEngine = bioEngine;
      _windowManager = windowManager;

      Items.Add(new UserInformationViewModel    ());
      Items.Add(new UserContactlessCardViewModel(_bioEngine));
     
      ActiveItem = Items[0];
      OpenTab();

      CurrentImageView = new ImageViewModel();

      DisplayName = "Add New User";
    }

    public void Update(User user)
    {
      if (user != null)
      {
        _user = user;
        _userPageMode = UserPageMode.ExistingUser;

        DisplayName = (_user.First_Name_ + " " + _user.Last_Name_);
      }
      else
      {
        _user = new User()
        {
            First_Name_ = ""
          , Last_Name_ = ""
          , Photo = ""
          , Gender = Gender.Male.ToString()
          , Rights = Rights.Operator.ToString()
        };

        _userPageMode = UserPageMode.NewUser;
        DisplayName = "Add New User";
      }      

      CurrentImageView.Update( _user);
      
      foreach (IScreen scrn in Items)
      {
        MethodInfo method = scrn.GetType().GetMethod("Update");
        if (method != null)
          method.Invoke(scrn, new object[] { _user } );        
      }
      
    }
      

    public void OpenTab()
    {      
      ActiveItem.Activate();     
    }

    private ImageViewModel _currentImageView;
    public ImageViewModel CurrentImageView
    {
      get { return _currentImageView; }
      private set
      {
        if (_currentImageView != value)
        {
          _currentImageView = value;
          NotifyOfPropertyChange(() => CurrentImageView);
        }
      }
    }

    public void Apply()
    {     
      if (_userPageMode == UserPageMode.NewUser)
        _bioEngine.Database().AddUser(_user);
      else
        _bioEngine.Database().UpdateUser(_user);

      MessageBox.Show(_userPageMode == UserPageMode.NewUser
                      ? "User Successfully Added"
                      : "Successfully updated");

      foreach (IScreen scrn in Items)
      {
        MethodInfo method = scrn.GetType().GetMethod("Apply");
        if (method != null)
          method.Invoke(scrn, null);
      }      
    }


    private User                _user         ;
    private UserPageMode        _userPageMode ;
    private IWindowManager      _windowManager;
    private readonly IBioEngine _bioEngine    ;
    
  }
}
