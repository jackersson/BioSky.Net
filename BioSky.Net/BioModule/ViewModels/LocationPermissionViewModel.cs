using BioData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using BioService;
using BioModule.Utils;
using System.Collections;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using static BioService.Location.Types;
using Google.Protobuf.Collections;

namespace BioModule.ViewModels
{

  public class LocationPermissionViewModel : Screen, IUpdatable
  {
    public LocationPermissionViewModel(IProcessorLocator locator)
    {
      DisplayName = "UsersNotification";

      _locator = locator;
      _database   = _locator.GetProcessor<IBioSkyNetRepository>();

      SelectedPersons = new ObservableCollection<Person>();
      PageController  = new PageControllerViewModel();

      AccessedPersons = new HashSet<long>();


      _database.Persons.DataChanged += RefreshData;
    }

    #region Update
    protected override void OnActivate()
    {
      base.OnActivate();
      RefreshData();
    }

    public void RefreshData()
    {
      if (!IsActive)
        return;

      Users = null;
      Users = _database.Persons.Data;

      if (Users == null || Users.Count <= 0)
        return;

      UsersCollectionView = null;
      UsersCollectionView = new PagingCollectionView(Users, PAGES_COUNT);

      PageController.UpdateData(UsersCollectionView);

      CheckOnState();
    }

    public void Update(Location location)
    {
      CurrentLocation = location;

      AccessedPersons.Clear();
      foreach (Person person in CurrentLocation.Persons)
        AccessedPersons.Add(person.Id);

      SelectedState = location.AccessType;
      RefreshData();
    }

    public void OnMouseRightButtonDown(string deviceItem) { MenuRemoveStatus = false; SelectedPerson = null; }

    #endregion

    #region Interface
    public void OnSearchTextChanged(string SearchText)
    {
      UsersCollectionView.Filtering = item =>
      {
        Person vitem = item as Person;
        if (vitem == null) return false;

        if (String.IsNullOrEmpty(SearchText))
          return true;

        if (vitem.Firstname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
            vitem.Lastname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
          return true;

        return false;
      };    
    }
    
    private void SetPermissionState()
    {
      if (SelectedState == AccessType.Custom)
        return;

      bool state = (SelectedState == AccessType.All) ? true : false;

      if (Users == null)
        Users = _database.Persons.Data;

      foreach (Person item in Users)
        ActiveDeactiveItem(item, state);

      NotifyChanges();
    }

    private void NotifyChanges()
    {
      NotifyOfPropertyChange(() => AccessedPersons);
      OnPermissionsChanged();
    }

    public RepeatedField<Person> GetResult()
    {
      if (SelectedState != AccessType.Custom)
        return null;
      
      RepeatedField<Person> persons = new RepeatedField<Person>();

      RepeatedField<Person> existingPersons = CurrentLocation.Persons;

      if (AccessedPersons.Count > 0)
      {
        for (int i = 0; i < existingPersons.Count; ++i)
        {
          Person pp = existingPersons[i];
          if (!AccessedPersons.Contains(pp.Id))
            persons.Add(new Person() { EntityState = EntityState.Deleted, Id = pp.Id });
        }

        List<long> personsToAdd = new List<long>(AccessedPersons);
        for (int i = 0; i < personsToAdd.Count; ++i)
        {
          Person pp = new Person { EntityState = EntityState.Added, Id = personsToAdd[i] };
          persons.Add(pp);
        }
      }

      return persons;
    }

    public void OnSelectionChanged(SelectionChangedEventArgs e)
    {      
      IList selectedRecords   = e.AddedItems as IList;
      IList unselectedRecords = e.RemovedItems as IList;

      if (selectedRecords == null || unselectedRecords == null)
        return;

      foreach (Person currentItem in selectedRecords)
        SelectedPersons.Add(currentItem);

      foreach (Person currentItem in unselectedRecords)
        SelectedPersons.Remove(currentItem);

      MenuRemoveStatus = (SelectedPersons.Count >= 1) ? true : false;      
    }
    
    public void Apply() {}

    public void OnAllow(bool state)
    {
      if (SelectedPersons.Count < 1)
        return;
      
      foreach (Person item in SelectedPersons)        
        ActiveDeactiveItem(item, state);
        
      CheckOnState();
      NotifyChanges();
    }

   

    private void OnPermissionsChanged()
    {
      if (PermissionsChanged != null)
        PermissionsChanged(null, EventArgs.Empty);
    }
    
    private void CheckOnState()
    {
      if (AccessedPersons.Count >= Users.Count)
        SelectedState = AccessType.All;
      else if (AccessedPersons.Count <= 0)
        SelectedState = AccessType.None;
      else
        SelectedState = AccessType.Custom;    
    }

    private void ActiveDeactiveItem(Person item, bool activate)
    {
      if (activate)
        AccessedPersons.Add(item.Id);
      else
        AccessedPersons.Remove(item.Id);      
    }

    #endregion

    #region UI
    private IPagingCollectionView _personsCollectionView;
    public IPagingCollectionView UsersCollectionView
    {
      get { return _personsCollectionView; }
      set
      {
        if (_personsCollectionView != value)
        {
          _personsCollectionView = value;
          NotifyOfPropertyChange(() => UsersCollectionView);
        }
      }
    }

    private bool _menuRemoveStatus;
    public bool MenuRemoveStatus
    {
      get { return _menuRemoveStatus; }
      set
      {
        if (_menuRemoveStatus != value)
        {
          _menuRemoveStatus = value;
          NotifyOfPropertyChange(() => MenuRemoveStatus);
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
    
    public bool IsAccessChanged
    {
      get {
        RepeatedField<Person> p = GetResult();

        bool flag = false;

        if (AccessedPersons.Count != CurrentLocation.Persons.Count)
          flag = true;
        else
        {
          foreach (Person person in CurrentLocation.Persons)
            if (!AccessedPersons.Contains(person.Id))
              flag = true;
        }

        return CurrentLocation.AccessType != SelectedState || flag;
      }
    }

    private HashSet<long> _accessedPersons;
    public HashSet<long> AccessedPersons
    {
      get { return _accessedPersons; }
      set
      {
        if (_accessedPersons != value)
        {
          _accessedPersons = value;        
          NotifyOfPropertyChange(() => AccessedPersons);
        }
      }
    }

    private ObservableCollection<Person> _selectedPersons;
    public ObservableCollection<Person> SelectedPersons
    {
      get { return _selectedPersons; }
      set
      {
        if (_selectedPersons != value)
        {
          _selectedPersons = value;
          NotifyOfPropertyChange(() => SelectedPersons);
        }
      }
    }

    private AccessType _selectedState;
    public AccessType SelectedState
    {
      get { return _selectedState; }
      set
      {
        //if (_selectedState != value)
        //{
          _selectedState = value;
          SetPermissionState();
          NotifyOfPropertyChange(() => SelectedState);
        //}
      }
    }

    private List<string> _permissionStateSources;
    public List<string> PermissionStateSources
    {
      get {
        if (_permissionStateSources == null)
          _permissionStateSources = Enum.GetNames(typeof(AccessType)).ToList();
        return _permissionStateSources;
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

    private Person _selectedPerson;
    public Person SelectedPerson
    {
      get { return _selectedPerson; }
      set
      {
        if (_selectedPerson != value)
        {
          _selectedPerson = value;
          NotifyOfPropertyChange(() => SelectedPerson);
        }
      }
    }

    private Location _currentLocation;
    public Location CurrentLocation
    {
      get { return _currentLocation; }
      set
      {
        if ( _currentLocation != value)
        {
          _currentLocation = value;
          NotifyOfPropertyChange(() => CurrentLocation);
        }
      }
    }
    #endregion

    #region Global Variables

    private readonly IProcessorLocator    _locator   ;
    private readonly IBioSkyNetRepository _database  ;
    private int PAGES_COUNT = 100;

    public event EventHandler PermissionsChanged;
    #endregion
  }  

}
