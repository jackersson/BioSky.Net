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

    private static ConvertPhotoIdToImage _photoIDConverter;
    public static ConvertPhotoIdToImage PhotoIDConverter
    {
      get  { return _photoIDConverter; }
      set
      {
        if (_photoIDConverter != value)
        {
          _photoIDConverter = value;         
        }
      }
    }

    public UsersViewModel(IProcessorLocator locator)
    {
      DisplayName = "Users";

      _locator    = locator;
      _bioEngine  = locator.GetProcessor<IBioEngine>();
      _selector   = locator.GetProcessor<ViewModelSelector>();
      _bioService = _locator.GetProcessor<IServiceManager>();

      _users           = new RepeatedField<Person>();
      _selectedItemIds = new ObservableCollection<long>();

      //FilteredUsers = new RepeatedField<Person>();

      //FilteredUsers = _bioEngine.Database().GetAllUsers();

      IsDeleteButtonEnabled = false;
      PhotoIDConverter = new ConvertPhotoIdToImage(_bioEngine.Database());


      _bioEngine.Database().DataChanged += UsersViewModel_DataChanged;     

    }

    protected async override void OnActivate()
    {
      await _bioService.DatabaseService.PhotoRequest(new CommandPhoto());
      await _bioService.DatabaseService.PersonRequest(new CommandPerson());
    }

    public void UsersViewModel_DataChanged(object sender, EventArgs args)
    {
      OnPersonsChanged(_bioEngine.Database().Persons);
    }

    private void OnPersonsChanged(PersonList persons)
    {
      foreach (Person item in persons.Persons)
      {
        if (Users.Contains(item))
          return;

        Users.Add(item);
      }
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
      
      IList selectedRecords = e.AddedItems as IList;
      IList unselectedRecords = e.RemovedItems as IList;

      foreach (Person currentUser in selectedRecords)
      {
        SelectedItemIds.Add(currentUser.Id);
      }

      foreach (Person currentUser in unselectedRecords)
      {
        SelectedItemIds.Remove(currentUser.Id);
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
        _selector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.UserPage, new object[] { Users.Where( x => x.Id == (long)item).FirstOrDefault() });
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
    private readonly IProcessorLocator _locator   ;
    private readonly ViewModelSelector _selector  ;
    private readonly IBioEngine        _bioEngine ;    
    private readonly IServiceManager   _bioService;
  }
}
