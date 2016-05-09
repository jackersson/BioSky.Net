using System;
using System.Collections.Generic;
using System.Linq;

using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Windows.Controls;

using BioModule.Utils;
using BioContracts;
using BioService;
using System.Collections;
using System.ComponentModel;
using WPFLocalizeExtension.Extensions;
using BioContracts.Services;
using Grpc.Core;
using System.Diagnostics;

namespace BioModule.ViewModels
{

  //TODO filter by location
  public class VisitorsViewModel : Screen
  {
    public VisitorsViewModel(IProcessorLocator locator)
    {
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Visitors_");

      _locator       =  locator;         
      _selector      = _locator.GetProcessor<ViewModelSelector>();
      _bioService    = _locator.GetProcessor<IServiceManager>().DatabaseService;
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier      = _locator.GetProcessor<INotifier>();
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();
      
      _selectedVisitors      = new ObservableCollection<Visitor>();
      PageController         = new PageControllerViewModel();
      _sortDescriptionByTime = new SortDescription("Time", ListSortDirection.Descending);

      int count = 0;
      string s = _database.LocalStorage.GetParametr(ConfigurationParametrs.ItemsCountPerPage);
      if (Int32.TryParse(s, out count))
        PAGES_COUNT = count;

      _database.Visitors.DataChanged+= RefreshData;

      IsDeleteButtonEnabled = false;


      RefreshData();
    }

    #region Update

    public void UpdateLocation(Location location)
    {
      if (location == null)
        return;

      _location = location;

      Query.Locations.Clear();
      Query.Locations.Add(_location.Id);

      UpdateFilterMenu();
      RefreshData();
    }

    private void UpdateFilterMenu()
    {
      VisitorsFilterMenu.SetQuery(Query);
    }
    #endregion

    #region Database

    public async void Select()
    {
      QueryVisitors query = GetQuery();      
      try
      {        
        //await _bioService.VisitorDataClient.Select(query);
      }
      catch (RpcException ex) {
        _notifier.Notify(ex);
      }
    }
    public QueryVisitors GetQuery()
    {
      return VisitorsFilterMenu.GetQuery();
    }



    private void RefreshData()
    {
      if (!IsActive)
        return;

      AsyncObservableCollection<Visitor> Visitors = _database.Visitors.Data;

      if (Visitors == null || Visitors.Count <= 0)
        return;

      UpdateFilterMenu();
      VisitorsFiltering(Visitors);
      GetLastVisitor();

      VisitorsCollectionView = null;
      VisitorsCollectionView = new PagingCollectionView(FilteredVisitors, PAGES_COUNT);
      VisitorsCollectionView.Sort(SortDescriptionByTime);

      PageController.UpdateData(VisitorsCollectionView);     
    }

    #region Filtering
    public void VisitorsFiltering(AsyncObservableCollection<Visitor> visitors)
    {
      Query       = GetQuery();
      _predicates = GetPredicates(Query);

      FilteredVisitors.Clear();
      foreach (Visitor vs in visitors)
      {
        if (CheckQueryRequirements(vs, _predicates))
          FilteredVisitors.Add(vs);
      }
      FilteredVisitors = FilteredVisitors.OrderByDescending(x => x.Time).ToList();
    }
    private List<Predicate<Visitor>> GetPredicates(QueryVisitors query)
    {
      _predicates = new List<Predicate<Visitor>>();

      if (query.Locations.Count > 0)
        _predicates.Add(item => query.Locations.Contains(item.Locationid));

      /*if (query.Countries.Count > 0)
        predicates.Add(item => item.Person != null && query.Countries.Contains(item.Person.Country));*/

      /*if (query.Persons.Count > 0)
        predicates.Add(item => item.Person != null && query.Persons.Contains(item.Person.Id));*/

      if (query.DatetimeFrom >= 0 && query.DatetimeTo != 0)
      {
        long to = (query.DatetimeTo == 0) ? DateTime.Now.Ticks : query.DatetimeTo;

        _predicates.Add(item => item.Time >= query.DatetimeFrom
                            && item.Time <= query.DatetimeTo);
      }
      return _predicates;
    }
    private bool CheckQueryRequirements(Visitor item, List<Predicate<Visitor>> predicates)
    {
      foreach (Predicate<Visitor> predicate in predicates)
      {
        if (!predicate(item))
          return false;
      }
      return true;
    }

    private void GetLastVisitor()
    {
      LastVisitor = null;
      LastVisitor = FilteredVisitors.FirstOrDefault();
    }
    #endregion

    #endregion

    #region Interface
    public async void OnDeleteVisitors()
    {
      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (result == false)
        return;
      
      try
      {
        await _bioService.VisitorDataClient.Remove(SelectedVisitors);          
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }     
    }

    public void OnMouseDoubleClick(Visitor visitor)
    {          
      ShowPerson(visitor);    
    }

    public void OnAddVisitorAsUser()
    {
      ShowPerson(SelectedItem);
    }

    private void ShowPerson(Visitor visitor)
    {     
      if (visitor == null)
        return;
      
      Person person = _database.Persons.GetValue(visitor.Personid);
     
      _selector.ShowContent( ShowableContentControl.TabControlContent
                           , ViewModelsID.UserPage
                           , new object[] { person });      
    }
     

    public void OnDataContextChanged()
    {
      if (PhotoImage == null)      
        PhotoImage = new BioImageViewModel(_locator);      
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      VisitorsFilterMenu.ActivateWith(this);
      RefreshData();
    }

    protected override void OnDeactivate(bool close)
    {
      VisitorsFilterMenu.DeactivateWith(this);
      base.OnDeactivate(close);
    }

    public void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      IList selectedRecords   = e.AddedItems   as IList;
      IList unselectedRecords = e.RemovedItems as IList;

      if (selectedRecords == null || unselectedRecords == null)
        return;

      foreach (Visitor currentVisitor in selectedRecords)      
        SelectedVisitors.Add(currentVisitor);      

      foreach (Visitor currentVisitor in unselectedRecords)
        SelectedVisitors.Remove(currentVisitor);
     
      IsDeleteButtonEnabled = SelectedVisitors.Count >= 1 ? true : false;     
      
      if (SelectedVisitors.Count == 1 && PhotoImage != null)
      {        
        Visitor visitor = SelectedVisitors.FirstOrDefault();
        if (visitor != null)
        {
          //Photo photo = _database.PhotoHolder.GetValue(visitor.Photoid);
          //ImageView.UpdateImage(photo, _database.LocalStorage.LocalStoragePath);
        }
      }
    }

    private bool ApplyTextFilter(Visitor item, string SearchText, Dictionary<long, Person> dictionary)
    {
      if (String.IsNullOrEmpty(SearchText))
        return true;

      if (item != null)
      {
        Person person = null;
        if (dictionary.TryGetValue(item.Personid, out person))
        {
          if (person.Firstname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
              person.Lastname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
            return true;
        }
      }
      return false;
    }

    public void OnSearchTextChanged(string SearchText)
    {        
      Dictionary<long, Person> dictionary = _database.Persons.DataSet;

      if (VisitorsCollectionView == null)
        return;

      VisitorsCollectionView.Filtering = item =>
      {
        return ApplyTextFilter(item as Visitor, SearchText, dictionary);
      };

      PageController.UpdateMove();     
    }

    public void ApplyQuery(QueryVisitors query)
    {
      if (VisitorsCollectionView == null)
        return;

      VisitorsCollectionView.Filtering = item =>
      {
        return ApplyQuery(item as Visitor, query);
      };
    }

    private bool ApplyQuery(Visitor item, QueryVisitors query)    {

      if (item != null && query.Locations.Contains(item.Locationid))
        return true;

      return false;
    }

    public void OnMouseRightButtonDown(Visitor visitor)
    {
      MenuOpenStatus = (visitor != null);
      SelectedItem = visitor;
    }
    
    public void ShowUserPage()
    {
      foreach (Visitor visitor in SelectedVisitors)          
          ShowPerson(visitor);                
    }
   
    #endregion

    #region UI   

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

    private BioImageViewModel _photoImage;
    public BioImageViewModel PhotoImage
    {
      get { return _photoImage; }
      set
      {
        if (_photoImage != value)
        {
          _photoImage = value;
          NotifyOfPropertyChange(() => PhotoImage);
        }
      }
    }

    private static Visitor _defaultVisitor;
    private Visitor DefaultVisitor
    {
      get {
        if (_defaultVisitor == null)
          _defaultVisitor = new Visitor() { Id = 0, Personid = 0 };

        return _defaultVisitor;
      }
    }

    private static VisitorsFilterMenuViewModel _visitorsFilterMenu;
    public VisitorsFilterMenuViewModel VisitorsFilterMenu
    {
      get {
        if (_visitorsFilterMenu == null)
          _visitorsFilterMenu = new VisitorsFilterMenuViewModel(_locator);

        return _visitorsFilterMenu; }
      set
      {
        if (_visitorsFilterMenu != value)
        {
          _visitorsFilterMenu = value;
          NotifyOfPropertyChange(() => VisitorsFilterMenu);
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

    private SortDescription _sortDescriptionByTime;
    public SortDescription SortDescriptionByTime
    {
      get { return _sortDescriptionByTime; }     
    }

    private List<Visitor> _filteredVisitors;
    public List<Visitor> FilteredVisitors
    {
      get { return (_filteredVisitors == null) ? _filteredVisitors = new List<Visitor>()
                                               : _filteredVisitors; }
      set
      {
        if (_filteredVisitors != value)
        {
          _filteredVisitors = value;
          NotifyOfPropertyChange(() => FilteredVisitors);
        }
      }
    }

    private Visitor _lastVisitor;
    public Visitor LastVisitor
    {
      get {

        if (_lastVisitor == null)
          return DefaultVisitor;
        return _lastVisitor; }
      set
      {
        if (_lastVisitor != value)
        {
          _lastVisitor = value;
          NotifyOfPropertyChange(() => LastVisitor);
        }
      }
    }      

    private Visitor _selectedItem;
    public Visitor SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem != value)
          _selectedItem = value;
        NotifyOfPropertyChange(() => SelectedItem);
      }
    }

    private bool _menuOpenStatus;
    public bool MenuOpenStatus
    {
      get { return _menuOpenStatus;  }
      set
      {
        if (_menuOpenStatus != value)
          _menuOpenStatus = value;

        NotifyOfPropertyChange(() => MenuOpenStatus);
      }
    }

    private ObservableCollection<Visitor> _selectedVisitors;
    public ObservableCollection<Visitor> SelectedVisitors
    {
      get { return _selectedVisitors; }
      set
      {
        if (_selectedVisitors != value)
        {
          _selectedVisitors = value;
          NotifyOfPropertyChange(() => SelectedVisitors);
        }
      }
    }

    private IPagingCollectionView _visitorsCollectionView;
    public IPagingCollectionView VisitorsCollectionView
    {
      get { return _visitorsCollectionView; }
      set
      {
        if (_visitorsCollectionView != value)
        {
          _visitorsCollectionView = value;

          NotifyOfPropertyChange(() => VisitorsCollectionView);
        }
      }
    }

    private QueryVisitors _query;
    public QueryVisitors Query
    {
      get { return (_query == null) ? _query = new QueryVisitors() 
                                    : _query; }
      set
      {
        if (_query != value)        
          _query = value;        
      }
    }

    #endregion

    #region Global Variables  
    private readonly IProcessorLocator        _locator      ;
    private readonly ViewModelSelector        _selector     ;
    private readonly IDatabaseService         _bioService   ;
    private readonly IBioSkyNetRepository     _database     ;
    private readonly INotifier                _notifier     ;
    private readonly DialogsHolder            _dialogsHolder;
    private          Location                 _location     ;
    private          List<Predicate<Visitor>> _predicates   ;

    private int PAGES_COUNT = 10;
    #endregion
  } 
}