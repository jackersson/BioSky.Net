using System;
using System.Collections;
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

using System.Windows.Input;

using System.Windows.Data;

using BioModule.Utils;


namespace BioModule.ViewModels
{ 
  public class UsersViewModel : Screen
  {    
    public UsersViewModel(IBioEngine bioEngine, ViewModelSelector selector)
    {
      _bioEngine = bioEngine;
      _selector  = selector;

      DisplayName = "Users";

      _users = new ObservableCollection<User>();

      FilteredUsers = new ObservableCollection<User>(); 

      Users         = _bioEngine.Database().GetAllUsers();
      FilteredUsers = _bioEngine.Database().GetAllUsers();       
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

    private ObservableCollection<User> _filteredUsers;
    public ObservableCollection<User> FilteredUsers
    {
      get { return _filteredUsers; }
      set
      {
        if (_filteredUsers != value)
        {
          _filteredUsers = value;
          NotifyOfPropertyChange(() => FilteredUsers);
        }
      }
    }
      

    public void Update()
    { }
     
        
    //*************************************************************Context Menu******************************************\
   
    private User _selectedItem;
    public User SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem != value)
        {
          _selectedItem = value;
          NotifyOfPropertyChange(() => SelectedItem);
        }          
      }
    }

    private bool _canOpenInNewTab;
    public bool CanOpenInNewTab
    {
      get  { return _canOpenInNewTab; }
      set
      {
        if (_canOpenInNewTab != value)
        {
          _canOpenInNewTab = value;
          NotifyOfPropertyChange(() => CanOpenInNewTab);
        }          
      }
    }    

    public void OnMouseRightButtonDown( User user )
    {
      CanOpenInNewTab = (user != null);
      SelectedItem = user;
    }

    public void ShowUserPage( bool isExistingUser )
    {   
      _selector.ShowContent( ShowableContentControl.TabControlContent
                           , ViewModelsID.UserPage, new object[] { isExistingUser ? SelectedItem : null } );
    }

    //************************************************************SearchBox***************************************************

    public void OnSearchTextChanged(string s)
    {
      FilteredUsers.Clear();
      if (s == "")
      {
        foreach (User user in _users)
          FilteredUsers.Add(user);        
      }
      else
      {
       
        IEnumerable<User> filter = Users.Where(x => (x.First_Name_.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0));
        foreach (User user in filter)
          FilteredUsers.Add(user);
      }
      NotifyOfPropertyChange(() => FilteredUsers);          
    }

    private readonly ViewModelSelector _selector;
    private readonly IBioEngine        _bioEngine;

    //************************************************************** UI *****************************************8
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
