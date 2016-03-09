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

namespace BioModule.ViewModels
{
  public enum PermissionState
  {
        All
      , None
      , Custom
  }
  public class LocationPermissionViewModel : Screen, IUpdatable
  {
    List<Person>  _persons      = new List<Person>() ;
    HashSet<long> activePersons = new HashSet<long>();

    public LocationPermissionViewModel(IProcessorLocator locator)
    {
      DisplayName = "UsersNotification";

      _locator = locator;
      _bioService = _locator.GetProcessor<IServiceManager>();
      _bioEngine  = _locator.GetProcessor<IBioEngine>();

      SelectedPersons = new ObservableCollection<PermissionItem>();
      PersonsList     = new AsyncObservableCollection<PermissionItem>();

      for(int i = 1; i < 20; i++)
      {
        Person p = new Person() {Id = i, Firstname = "Sasha" + i, Lastname = "Iskra" + i };
        _persons.Add(p);
      }

      Person p2 = new Person() { Id = 100, Firstname = "Taras", Lastname = "Lishenko" };
      _persons.Add(p2);


    }

    #region Update


    public void RefreshData()
    {
      PersonsList.Clear();

      foreach (Person item in _persons) //_bioEngine.Database().PersonHolder.Data)
      {

        PermissionItem permissionItem = new PermissionItem()
        {
          ItemContext = item
        , ItemActive = false
        };

        PersonsList.Add(permissionItem);
      }

      PersonsCollectionView = null;
      PersonsCollectionView = CollectionViewSource.GetDefaultView(PersonsList);
    }

    public void Update(Location location)
    {
      _location = location;

      RefreshData();
    }

    #endregion

    #region Interface
    public void OnSearchTextChanged(string SearchText)
    {
      PersonsCollectionView.Filter = item =>
      {
        PermissionItem permItem = item as PermissionItem;
        if (permItem == null) return false;

        if (String.IsNullOrEmpty(SearchText))
          return true;

        if (permItem.ItemContext.Firstname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
            permItem.ItemContext.Lastname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
          return true;

        return false;
      };
    }

    public void OnStateChange()
    {
      SetPermissionState();
    }
    private void SetPermissionState()
    {
      if (SelectedState == PermissionState.Custom)
        return;

      bool state = (SelectedState == PermissionState.All) ? true : false;

      foreach (PermissionItem item in PersonsList)
        ActiveDeactiveItem(item, state);
    }

    public void OnMouseRightButtonDown(PermissionItem permissionItem)
    {
      MenuRemoveStatus = (SelectedPerson != null);
      SelectedPerson = permissionItem;
    }
    public void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      IList selectedRecords = e.AddedItems as IList;
      IList unselectedRecords = e.RemovedItems as IList;

      if (selectedRecords == null || unselectedRecords == null)
        return;

      foreach (PermissionItem currentItem in selectedRecords)
        SelectedPersons.Add(currentItem);

      foreach (PermissionItem currentItem in unselectedRecords)
        SelectedPersons.Remove(currentItem);

      MenuRemoveStatus = (SelectedPersons.Count >= 1) ? true : false;
    }

    public void Apply()
    {

    }

    public void OnAllow(bool state)
    {
      if (SelectedPersons.Count >= 1)
      {
        foreach (PermissionItem item in SelectedPersons)
        {
          ActiveDeactiveItem(item, state);
          CheckOnState(state);
        }
      }
    }

    private void CheckOnState(bool active)
    {
      if (active)
        SelectedState = (activePersons.Count >= PersonsList.Count) ? PermissionState.All : PermissionState.Custom;
      else
        SelectedState = (activePersons.Count <= 0) ? PermissionState.None : PermissionState.Custom;
    }

    private void ActiveDeactiveItem(PermissionItem item, bool activate)
    {
      item.ItemActive = activate;
      item.ItemEnabled = !activate;

      if (activate)
        activePersons.Add(item.ItemContext.Id);
      else
        activePersons.Remove(item.ItemContext.Id);
    }

    #endregion

    #region UI

    private ICollectionView _personsCollectionView;
    public ICollectionView PersonsCollectionView
    {
      get { return _personsCollectionView; }
      set
      {
        if (_personsCollectionView != value)
        {
          _personsCollectionView = value;

          NotifyOfPropertyChange(() => PersonsCollectionView);
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

    private AsyncObservableCollection<PermissionItem> _personsList;
    public AsyncObservableCollection<PermissionItem> PersonsList
    {
      get { return _personsList; }
      set
      {
        if (_personsList != value)
        {
          _personsList = value;
          NotifyOfPropertyChange(() => PersonsList);
        }
      }
    }

    private PermissionItem _selectedPerson;
    public PermissionItem SelectedPerson
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

    private ObservableCollection<PermissionItem> _selectedPersons;
    public ObservableCollection<PermissionItem> SelectedPersons
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

    private PermissionState _selectedState;
    public PermissionState SelectedState
    {
      get { return _selectedState; }
      set
      {
        if (_selectedState != value)
        {
          _selectedState = value;
          NotifyOfPropertyChange(() => SelectedState);
        }
      }
    }

    public List<string> StateSources
    {
      get { return new List<string>() { "All" , "None", "Custom"}; }
    }

    #endregion

    #region Global Variables

    private Location                   _location  ;
    private readonly IProcessorLocator _locator   ;
    private readonly IBioEngine        _bioEngine ;
    private readonly IServiceManager   _bioService;

    #endregion
  }

  public class PermissionItem : DeviceItemBase<Person> { }

}
