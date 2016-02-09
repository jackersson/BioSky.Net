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
using BioService;
using BioContracts;
using Google.Protobuf.Collections;

namespace BioModule.ViewModels
{
  public class UsersViewModel : Screen
  {
    public UsersViewModel(IProcessorLocator locator, IWindowManager windowManager)
    {
      DisplayName = "Users";

      _locator       = locator;
      _windowManager = windowManager;
      _bioEngine     = locator.GetProcessor<IBioEngine>();
      _selector      = locator.GetProcessor<ViewModelSelector>();
      _bioService    = _locator.GetProcessor<IServiceManager>();
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();

      _selectedItemIds = new ObservableCollection<long>();
      
      _database.Persons.DataChanged     += RefreshData;
      _database.PhotoHolder.DataChanged += RefreshData;

      IsDeleteButtonEnabled = false;
    }

    private void RefreshData()
    {
      if (!IsActive)
        return;

      Users = null;
      Users = _database.PersonHolder.Data;
    }   

    
    protected override void OnActivate()
    {          
      base.OnActivate();
      RefreshData();
    }
  
    private AsyncObservableCollection<Person> _users;
    public AsyncObservableCollection<Person> Users
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
        IsDeleteButtonEnabled = true;      
      else      
        IsDeleteButtonEnabled = false;            
    }

    public void OnDeleteUsers()
    {
      //await UserUpdatePerformer(DbState.Remove);      
    }

   /*

    private void DatabaseService_PersonsUpdated(PersonList list, Result result)
    {
      //PersonUpdateResultProcessing(list, result);
    }
   
    */
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

    public void OnMouseDoubleClick(Person user)
    {
      if(user != null)
      {
        _selector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.UserPage
                             , new object[] { user });
      }
    }


    public void ShowUserPage(bool isExistingUser)
    {
      //TODO refactor
      if (!isExistingUser)
        _selector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.UserPage, new object[] { null });
      else
      {
        foreach (long id in SelectedItemIds)
        {
          Person person = null;
          bool personFound = _bioEngine.Database().PersonHolder.DataSet.TryGetValue(id, out person);

          if (personFound)
          {
            _selector.ShowContent( ShowableContentControl.TabControlContent
                                 , ViewModelsID.UserPage
                                 , new object[] { person });
          }
        }
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
    private readonly IWindowManager       _windowManager;
    private readonly IProcessorLocator    _locator      ;
    private readonly ViewModelSelector    _selector     ;
    private readonly IBioEngine           _bioEngine    ;    
    private readonly IServiceManager      _bioService   ;
    private readonly IBioSkyNetRepository _database     ;
  }
}
