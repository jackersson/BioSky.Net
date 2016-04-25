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
using Google.Protobuf.Collections;
using static BioService.AccessInfo.Types;

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

      DesiredAccessedPersons = new HashSet<long>();
      ActiveAccessedPersons  = new HashSet<long>();

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
      if (CurrentLocation == location)
        return;

      CurrentLocation = location;
      DesiredAccessedPersons.Clear();
      ActiveAccessedPersons .Clear();

      AccessInfo currentAccessInfo = CurrentLocation.AccessInfo;

      if (currentAccessInfo == null)
      {
        SelectedAccessType = AccessType.None;
        return;
      }

      AccessType currentState  = currentAccessInfo.AccessType;

      SelectedAccessType = currentState;

      if (currentState == AccessType.Custom)
      {
        ActiveAccessedPersons  = new HashSet<long>(currentAccessInfo.Persons.Select(x => x.Id));
        DesiredAccessedPersons = new HashSet<long>(ActiveAccessedPersons);
      }      
      
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
        
    public AccessInfo GetResult()
    {
      if (!IsAccessChanged(false))
        return null;

      AccessInfo result  = new AccessInfo();
      result.AccessType  = SelectedAccessType;
      result.EntityState = EntityState.Modified;

      if (SelectedAccessType != AccessType.Custom)
        return result;

      IEnumerable<long> itemsToAdd    = DesiredAccessedPersons.Except(ActiveAccessedPersons);
      IEnumerable<long> itemsToRemove = ActiveAccessedPersons.Except(DesiredAccessedPersons);

      result.Persons.Add(itemsToAdd.Select   (x => new Person() { Id = x, EntityState = EntityState.Added }));
      result.Persons.Add(itemsToRemove.Select(x => new Person() { Id = x, EntityState = EntityState.Deleted }));         

      return result;
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
    
    private void CheckOnState()
    {
      if (DesiredAccessedPersons.Count >= Users.Count)
        SelectedAccessType = AccessType.All;
      else if (DesiredAccessedPersons.Count <= 0)
        SelectedAccessType = AccessType.None;
      else
        SelectedAccessType = AccessType.Custom;    
    }

    private void ActiveDeactiveItem(Person item, bool activate)
    {
      if (activate)
        DesiredAccessedPersons.Add(item.Id);
      else
        DesiredAccessedPersons.Remove(item.Id);      
    }

    #endregion
       
    private bool _isAccessChanged;
    public bool IsAccessChanged( bool useActive = true )
    {     
      if (useActive && !IsActive)
        return _isAccessChanged;

      bool flag = false;
      AccessInfo currentAccessInfo = CurrentLocation.AccessInfo;
      if (currentAccessInfo == null)
        _isAccessChanged = true;
      else
      {
        AccessType currentAccessType = currentAccessInfo.AccessType;

        if (currentAccessType != SelectedAccessType)
          _isAccessChanged = true;
        else if (SelectedAccessType != AccessType.Custom)
          _isAccessChanged = false;
        else
          _isAccessChanged = DesiredAccessedPersons.SequenceEqual(ActiveAccessedPersons);
      }
      return _isAccessChanged;             
    }

    #region UI
    private HashSet<long> _desiredAccessedPersons;
    public HashSet<long> DesiredAccessedPersons
    {
      get { return _desiredAccessedPersons; }
      set
      {
        if (_desiredAccessedPersons != value)
        {
          _desiredAccessedPersons = value;        
          NotifyOfPropertyChange(() => DesiredAccessedPersons);
        }
      }
    }

    private HashSet<long> _activeAccessedPersons;
    public HashSet<long> ActiveAccessedPersons
    {
      get { return _activeAccessedPersons; }
      set
      {
        if (_activeAccessedPersons != value)
        {
          _activeAccessedPersons = value;
          NotifyOfPropertyChange(() => ActiveAccessedPersons);
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


    private AccessType _selectedState;
    public AccessType SelectedAccessType
    {
      get { return _selectedState; }
      set
      {        
        _selectedState = value;
       
        NotifyOfPropertyChange(() => SelectedAccessType);

        if (SelectedAccessType != AccessType.Custom)
        {
          DesiredAccessedPersons.Clear();
          NotifyOfPropertyChange(() => DesiredAccessedPersons);
          return;
        }
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
    #endregion
  }  

}
