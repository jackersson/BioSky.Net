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
using Grpc.Core;
using WPFLocalizeExtension.Extensions;


namespace BioModule.ViewModels
{
  public class UsersViewModel : Screen
  {
    public UsersViewModel(IProcessorLocator locator, IWindowManager windowManager)
    {
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Users_");

      _locator       = locator;
      _windowManager = windowManager;
      _bioEngine     = locator.GetProcessor<IBioEngine>();
      _selector      = locator.GetProcessor<ViewModelSelector>();
      _bioService    = _locator.GetProcessor<IServiceManager>();
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();

      _selectedItemIds = new ObservableCollection<long>();
      PageController   = new PageControllerViewModel(this);


      _database.Persons.DataChanged += RefreshData;      
      _database.PhotoHolder.DataChanged  += RefreshData;

      IsDeleteButtonEnabled = false;   
    }
    #region Database
    private void RefreshData()
    {
      Users = null;
      Users = _database.PersonHolder.Data;
      UsersCollectionView = null;

      if (Users.Count > 0)
      {
        UsersCollectionView = new PagingCollectionView(Users, 10);
        PageController.UpdateData(UsersCollectionView.GetPagingData());
      }
      else
        PageController.UpdateData(new PagingData() { startIndex = 0, endIndex = 0, count = 0, itemsPerPage = 0 });
    }  

    #endregion

    #region PageController
    public void MovePage(bool isRightSide)
    {
      if (Users.Count <= 0)
        return;

      if (isRightSide)
        UsersCollectionView.MoveToNextPage();
      else
        UsersCollectionView.MoveToPreviousPage();

      PageController.UpdateData(UsersCollectionView.GetPagingData());
    }

    #endregion

    #region BioService

    public async Task UsersDeletePerformer(EntityState state)
    {
      PersonList personList = new PersonList();


      foreach (long id in SelectedItemIds)
      {
        Person person = new Person() { Id = id, EntityState = EntityState.Deleted };
        personList.Persons.Add(person);
      }

      try
      {
        _database.Persons.DataUpdated += UpdateData;
        await _bioService.DatabaseService.PersonUpdate(personList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }
    }

    private void UpdateData(PersonList list)
    {
      _database.Persons.DataUpdated -= UpdateData;

      if (list != null)
      {
        Person person = list.Persons.FirstOrDefault();
        if (person != null)
        {
          if (person.EntityState == EntityState.Deleted)
          {
            if(list.Persons.Count > 1)
              MessageBox.Show( list.Persons.Count + " users successfully Deleted");
            else
              MessageBox.Show("User successfully Deleted");
          }
        }
      }
    }   

    #endregion

    #region Interface
    public async void OnDeleteUsers()
    {
      var result = _windowManager.ShowDialog(DialogsHolder.AreYouSureDialog);

      if (result == true)
      {
         try
        {
          await UsersDeletePerformer(EntityState.Deleted);
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        } 
      }
    } 



    public void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      IList selectedRecords = e.AddedItems as IList;
      IList unselectedRecords = e.RemovedItems as IList;

      foreach (Person currentUser in selectedRecords)      
        SelectedItemIds.Add(currentUser.Id);
      

      foreach (Person currentUser in unselectedRecords)      
        SelectedItemIds.Remove(currentUser.Id);
      

      if (SelectedItemIds.Count >= 1)
        IsDeleteButtonEnabled = true;
      else
        IsDeleteButtonEnabled = false;
    }
    protected override void OnActivate()
    {
      base.OnActivate();
      RefreshData();
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
          Person person = _bioEngine.Database().PersonHolder.GetValue(id);

          if (person != null)
          {
            _selector.ShowContent(ShowableContentControl.TabControlContent
                                 , ViewModelsID.UserPage
                                 , new object[] { person });
          }
        }
      }
    }
    public void OnSearchTextChanged(string s)
    {
      SearchText = s;

      UsersCollectionView.Filtering = item =>
      {
        Person vitem = item as Person;
        if (vitem == null) return false;

        if (String.IsNullOrEmpty(SearchText))
          return true;

        if ((item as Person).Firstname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
            (item as Person).Lastname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
        {
          return true;
        }
        return false;
      };
    }
    public void OnMouseRightButtonDown(Person user)
    {
      CanOpenInNewTab = (user != null);
      SelectedItem = user;
    }

    public void OnMouseDoubleClick(Person user)
    {
      if (user != null)
      {
        _selector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.UserPage
                             , new object[] { user });
      }
    }
    #endregion

    #region UI
    private string _searchText;
    public string SearchText
    {
      get { return _searchText; }
      set
      {
        if (_searchText != value)
        {
          _searchText = value;

          NotifyOfPropertyChange(() => SearchText);
        }
      }
    }

    private IPagingCollectionView _usersCollectionView;
    public IPagingCollectionView UsersCollectionView
    {
      get { return _usersCollectionView; }
      set
      {
        if (_usersCollectionView != value)
        {
          _usersCollectionView = value;

          NotifyOfPropertyChange(() => UsersCollectionView);
        }
      }
    }

    private PageControllerViewModel _pageController;
    public PageControllerViewModel PageController
    {
      get { return _pageController; }
      set
      {
        if (_pageController != value)
        {
          _pageController = value;
          NotifyOfPropertyChange(() => PageController);
        }
      }
    }

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


    #endregion

    #region Global Variables
    private readonly IWindowManager       _windowManager;
    private readonly IProcessorLocator    _locator      ;
    private readonly ViewModelSelector    _selector     ;
    private readonly IBioEngine           _bioEngine    ;    
    private readonly IServiceManager      _bioService   ;
    private readonly IBioSkyNetRepository _database;
    #endregion
  } 
}
