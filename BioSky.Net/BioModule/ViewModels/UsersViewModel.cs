using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Drawing;
using System.Reflection;

using BioModule.ResourcesLoader;
using System.ComponentModel;
using System.Collections.ObjectModel;

using BioData;
using BioModule.Model;

namespace BioModule.ViewModels
{
 
  public class UsersViewModel : PropertyChangedBase
  {
    IBioEngine _bioEngine;
    public UsersViewModel(IBioEngine bioEngine)
    {
      _bioEngine = bioEngine;
      _users = new ObservableCollection<User>();

      List<User> users = (List<User>)_bioEngine.Database().getAllUsers();
      foreach (User user in users)
        _users.Add(user);

      NotifyOfPropertyChange(() => Users);     
    }
  
    private ObservableCollection<User> _users;
    public ObservableCollection<User> Users
    {
      get { return _users; }
      set
      {
        if ( _users != value )
        {
          _users = value;
          NotifyOfPropertyChange(() => Users);
        }
      }
    }


    public BitmapSource AddIconSource
    {
      get { return ResourceLoader.AddIconSource; }
    }

    public BitmapSource RemoveIconSource
    {
      get { return ResourceLoader.RemoveIconSource; }
    }

    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }  

  }
}
