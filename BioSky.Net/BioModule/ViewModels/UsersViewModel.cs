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


using System.Windows.Input;

using System.Windows.Data;

using BioModule.Utils;
using BioFaceService;
using BioContracts;
using Google.Protobuf.Collections;

namespace BioModule.ViewModels
{
  public class UsersViewModel : Screen
  {
    public UsersViewModel(IProcessorLocator locator)
    {
      _locator   = locator;
      _bioEngine = locator.GetProcessor<IBioEngine>();
      _selector  = locator.GetProcessor<ViewModelSelector>();

      _users = new RepeatedField<Person>();
      _selectedItemIds = new ObservableCollection<long>();

      //FilteredUsers = new RepeatedField<Person>();

      Users = _bioEngine.Database().Persons.Persons;
      //FilteredUsers = _bioEngine.Database().GetAllUsers();

      IsDeleteButtonEnabled = false;

      DisplayName = "Users";
    }

    private RepeatedField<Person> _users;
    public RepeatedField<Person> Users
    {
      get { return _users; }
      set
      {
        if (_users != value)
        {
          _users = value;
          NotifyOfPropertyChange(() => Users);
        }
      }
    }

    /*
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
    */
    private bool _isSelected;
    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected != value)
        {
          _isSelected = value;
          NotifyOfPropertyChange(() => IsSelected);
        }
      }
    }

    private bool _isDeleteButtonEnabled;
    public bool IsDeleteButtonEnabled
    {
      get { return _isDeleteButtonEnabled; }
      set
      {
        if (_isDeleteButtonEnabled != value)
        {
          _isDeleteButtonEnabled = value;
          NotifyOfPropertyChange(() => IsDeleteButtonEnabled);
        }
      }
    }
    public void Update()
    { }

    private bool? _isAllItemsSelected;
    public bool? IsAllItemsSelected
    {
      get { return _isAllItemsSelected; }
      set
      {
        if (_isAllItemsSelected == value) return;

        _isAllItemsSelected = value;

        if (_isAllItemsSelected.HasValue)
          //SelectAll(_isAllItemsSelected.Value, FilteredVisitors);

          NotifyOfPropertyChange(() => IsAllItemsSelected);
      }
    }

    private ObservableCollection<long> _selectedItemIds;
    public ObservableCollection<long> SelectedItemIds
    {
      get { return _selectedItemIds; }
      set
      {
        if (_selectedItemIds != value)
        {
          _selectedItemIds = value;
          NotifyOfPropertyChange(() => SelectedItemIds);
        }
      }
    }

    public void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      /*
      IList selectedRecords = e.AddedItems as IList;
      IList unselectedRecords = e.RemovedItems as IList;

      foreach (User currentUser in selectedRecords)
      {
        SelectedItemIds.Add(currentUser.UID);
      }

      foreach (User currentUser in unselectedRecords)
      {
        SelectedItemIds.Remove(currentUser.UID);
      }

      if (SelectedItemIds.Count >= 1)
      {
        Console.WriteLine("Delete Records (" + SelectedItemIds.Count + ")");
        IsDeleteButtonEnabled = true;
      }
      else
      {
        Console.WriteLine("Delete Record");
        IsDeleteButtonEnabled = false;
      }
      foreach (long item in SelectedItemIds)
      {
        Console.WriteLine(item);
      }
      */
    }

    //*************************************************************Context Menu******************************************\

    private Person _selectedItem;
    public Person SelectedItem
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
      get { return _canOpenInNewTab; }
      set
      {
        if (_canOpenInNewTab != value)
        {
          _canOpenInNewTab = value;
          NotifyOfPropertyChange(() => CanOpenInNewTab);
        }
      }
    }

    public void OnMouseRightButtonDown(Person user)
    {
      CanOpenInNewTab = (user != null);
      SelectedItem = user;
    }

    public void ShowUserPage(bool isExistingUser)
    {
      foreach (long item in SelectedItemIds)
      {
        _selector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.UserPage, new object[] { Users[(int)item] });
      }
    }

    //************************************************************SearchBox***************************************************

    public void OnSearchTextChanged(string s)
    {
      /*
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
      */
    }
    private readonly IProcessorLocator _locator;
    private readonly ViewModelSelector _selector;
    private readonly IBioEngine        _bioEngine;
  }
}
