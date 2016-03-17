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

namespace BioModule.ViewModels
{

  //TODO filter by location
  public class VisitorsViewModel : Screen
  {
    public VisitorsViewModel(IProcessorLocator locator)
    {
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Visitors_");

      _locator       = locator;
         
      _selector      = _locator.GetProcessor<ViewModelSelector>();
      _bioService    = _locator.GetProcessor<IServiceManager>().DatabaseService;
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier      = _locator.GetProcessor<INotifier>();
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();


      _selectedVisitors = new ObservableCollection<Visitor>();
      PageController        = new PageControllerViewModel();

      _sortDescriptionByTime = new SortDescription("Time", ListSortDirection.Descending);
      
     // _database.PhotoHolder.DataChanged   += RefreshData;
      _database.Visitors.DataChanged      += RefreshData;

      IsMenuItemChecked = true;
      TimeFilterText = "Time";      
    } 
        
    #region Database

    private void RefreshData()
    {
      if (!IsActive)
        return;

      Visitors = null;
      Visitors = _database.Visitors.Data;

      if (Visitors == null || Visitors.Count <= 0)
        return;
           
      GetLastVisitor();

      VisitorsCollectionView = null;
      VisitorsCollectionView = new PagingCollectionView(Visitors, PAGES_COUNT);
      VisitorsCollectionView.Sort(SortDescriptionByTime);

      PageController.UpdateData(VisitorsCollectionView);
    }

    private void GetLastVisitor()
    {
      LastVisitor = null;     
      LastVisitor = Visitors.LastOrDefault();
    }

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
      //if (person == null)
        //person = new Person(){ Thumbnailid = visitor.Photoid };
      
     
      _selector.ShowContent( ShowableContentControl.TabControlContent
                           , ViewModelsID.UserPage
                           , new object[] { person });
      
    }
     

    public void OnDataContextChanged()
    {
      if (PhotoImage == null)
      {
        PhotoImage = new PhotoImageViewModel(_locator);
        PhotoImage.SetVisibility(true, false, false);
      }
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      RefreshData();
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

    public void OnSearchTextChanged(string SearchText)
    {        
      Dictionary<long, Person> dictionary = _database.Persons.DataSet;
      VisitorsCollectionView.Filtering = item =>
      {
        if (String.IsNullOrEmpty(SearchText))
          return true;

        Visitor vitem = item as Visitor;  
        if (vitem != null)
        {
          Person person = null;
          if (dictionary.TryGetValue(vitem.Personid, out person) )
          {
            if (person.Firstname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                person.Lastname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)            
              return true;            
          }
        }      

        return false;
      };      
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

    //*********************************************Filters*************************************************

    public void ShowPeriodTimePicker()
    {
      _dialogsHolder.PeriodTimePicker.Show();
      PeriodTimePickerResult result = _dialogsHolder.PeriodTimePicker.GetResult();
      if (result != null)
      {
        TimeFilterText = result.FromDateString + "-" + result.ToDateString;

        long[] timeFilter = { result.FromDateLong, result.ToDateLong };
        periodTime = timeFilter;
        SetTimeFilterState(TimeEnum.Period);
      }
    }

    private long[] periodTime        = { 0 , 0};
    private long[] currentTimeFilter = { 0 , 0};


    public long[] GetTimeFilterInterval(TimeEnum state)
    {
      long fromTime = 0;
      long toTime   = 0;

      DateTime nullDate = DateTime.Now;
      DateTime now      = DateTime.Now;
      DateTime dateTo   = now;
      DateTime dateFrom = now;

      switch (state)
      {
        case TimeEnum.AllTime:
          dateTo   = nullDate;
          dateFrom = nullDate;
          break;

        case TimeEnum.LastHour:
          dateFrom = now.AddHours(-1);
          break;

        case TimeEnum.Last24Houts:
          dateFrom = now.AddHours(-24);         
          break;

        case TimeEnum.LastMonth:
          dateFrom = now.AddMonths(-1);
          break;

        case TimeEnum.LastWeek:
          dateFrom = now.AddDays(-7);
          break;

        case TimeEnum.LastYear:
          dateFrom = now.AddYears(-7);
          break;
      }

      fromTime = dateFrom.Ticks;
      toTime   = dateTo.Ticks  ;

      long [] timeFilter = { fromTime, toTime };

      return timeFilter;
    }
    public void SetTimeFilterState(TimeEnum state)
    {
      TimeFilterState = state;
      if (state != TimeEnum.Period)
        currentTimeFilter = GetTimeFilterInterval(TimeFilterState);
      else
        currentTimeFilter = periodTime;
    }
    #endregion

    #region UI

    public string[] CountryNames
    {
      get { return _database.BioCultureSources.CountriesNames; }
    }



    private TimeEnum _timeFilterState;
    public TimeEnum TimeFilterState
    {
      get { return _timeFilterState; }
      set
      {
        if (_timeFilterState != value)
        {
          _timeFilterState = value;
          if(_timeFilterState != TimeEnum.Period)
            TimeFilterText = _timeFilterState.ToString();
          if (_timeFilterState != TimeEnum.None)
            TimeFilterText = "Time";
        }
      }
    }

    private string _timeFilterText;
    public string TimeFilterText
    {
      get { return _timeFilterText; }
      set
      {
        if (_timeFilterText != value)
        {
          _timeFilterText = value;
          NotifyOfPropertyChange(() => TimeFilterText);
        }
      }
    }

    private bool _isYearChecked;
    public bool IsYearChecked
    {
      get { return _isYearChecked; }
      set
      {
        if (_isYearChecked != value)
        {
          _isYearChecked = value;
          SetTimeFilterState(TimeEnum.LastYear);
          NotifyOfPropertyChange(() => IsYearChecked);
        }
      }
    }

    private bool _isMonthChecked;
    public bool IsMonthChecked
    {
      get { return _isMonthChecked; }
      set
      {
        if (_isMonthChecked != value)
        {
          _isMonthChecked = value;
          SetTimeFilterState(TimeEnum.LastMonth);
          NotifyOfPropertyChange(() => IsMonthChecked);
        }
      }
    }

    private bool _isWeekChecked;
    public bool IsWeekChecked
    {
      get { return _isWeekChecked; }
      set
      {
        if (_isWeekChecked != value)
        {
          _isWeekChecked = value;
          SetTimeFilterState(TimeEnum.LastWeek);
          NotifyOfPropertyChange(() => IsWeekChecked);
        }
      }
    }

    private bool _is24HoursChecked;
    public bool Is24HoursChecked
    {
      get { return _is24HoursChecked; }
      set
      {
        if (_is24HoursChecked != value)
        {
          _is24HoursChecked = value;
          SetTimeFilterState(TimeEnum.Last24Houts);
          NotifyOfPropertyChange(() => Is24HoursChecked);
        }
      }
    }

    private bool _isLastHourChecked;
    public bool IsLastHourChecked
    {
      get { return _isLastHourChecked; }
      set
      {
        if (_isLastHourChecked != value)
        {
          _isLastHourChecked = value;
          SetTimeFilterState(TimeEnum.LastHour);
          NotifyOfPropertyChange(() => IsLastHourChecked);
        }
      }
    }

    private bool _isAllTimeChecked;
    public bool IsAllTimeChecked
    {
      get { return _isAllTimeChecked; }
      set
      {
        if (_isAllTimeChecked != value)
        {
          _isAllTimeChecked = value;
          SetTimeFilterState(TimeEnum.AllTime);
          NotifyOfPropertyChange(() => IsAllTimeChecked);
        }
      }
    }

    private bool _isMenuItemChecked;
    public bool IsMenuItemChecked
    {
      get { return _isMenuItemChecked; }
      set
      {
        if (_isMenuItemChecked != value)
        {
          _isMenuItemChecked = value;
          NotifyOfPropertyChange(() => IsMenuItemChecked);
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

    private PhotoImageViewModel _photoImage;
    public PhotoImageViewModel PhotoImage
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

    private AsyncObservableCollection<Visitor> _visitors;
    public AsyncObservableCollection<Visitor> Visitors
    {
      get { return _visitors; }
      set
      {
        if (_visitors != value)
        {
          _visitors = value;
          NotifyOfPropertyChange(() => Visitors);
        }
      }
    }

    private Visitor _lastVisitor;
    public Visitor LastVisitor
    {
      get { return _lastVisitor; }
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

    #endregion

    #region Global Variables  
    private readonly IProcessorLocator    _locator      ;
    private readonly ViewModelSelector    _selector     ;
    private readonly IDatabaseService     _bioService   ;
    private readonly IBioSkyNetRepository _database     ;
    private readonly INotifier            _notifier     ;
    private readonly DialogsHolder        _dialogsHolder;


    private int PAGES_COUNT = 10;
    #endregion
  } 

  public enum TimeEnum
  {
     None
   , AllTime
   , LastHour
   , Last24Houts
   , LastWeek
   , LastMonth
   , LastYear
   , Period
  }
}