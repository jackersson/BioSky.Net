using BioContracts;
using BioModule.Utils;
using BioModule.ViewModels;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using WPFLocalizeExtension.Extensions;

namespace BioModule.ViewModels
{
  public interface IFilterable : INotifyPropertyChanged
  {
    void Apply(QueryVisitors query);
    void Reset();
    bool CanRevert { get; }    
  }

  #region locationFilter
  public class LocationFilter : PropertyChangedBase, IFilterable
  {
    private readonly IProcessorLocator    _locator ;
    private readonly IBioSkyNetRepository _database;    
    
    public LocationFilter(IProcessorLocator locator)
    {
      _locator  = locator;
      _database = locator.GetProcessor<IBioSkyNetRepository>();

      SelectedLocations = new ObservableCollection<Location>(); 
       
    }
     
    public void OnLocationsAllClick()
    {
      SelectedLocations.Clear();

      foreach (Location location in Locations)
        SelectedLocations.Add(location);

      RefreshUI();
    }

    public void OnLocationsNoneClick()
    {
      SelectedLocations.Clear();
      RefreshUI();
    }

    public void OnSelectionLocationChanged(SelectionChangedEventArgs e)
    {
      IList selectedRecords   = e.AddedItems   as IList;
      IList unselectedRecords = e.RemovedItems as IList;

      if (selectedRecords == null || unselectedRecords == null)
        return;

      foreach (Location location in selectedRecords)
      {
        if(!SelectedLocations.Contains(location))
          SelectedLocations.Add(location);
      }


      foreach (Location location in unselectedRecords)
        SelectedLocations.Remove(location);

      RefreshUI();
    }  

    public void Apply(QueryVisitors query)
    {
      if (SelectedLocations == null || SelectedLocations.Count <= 0)
        return; 
      
      foreach (Location trackLocation in SelectedLocations)
        query.Locations.Add(trackLocation.Id);      
    }

    public void Reset()
    {
      SelectedLocations.Clear();
      NotifyOfPropertyChange(() => SelectedLocations);
    }

    private void RefreshUI()
    {
      NotifyOfPropertyChange(() => SelectedLocations);
      NotifyOfPropertyChange(() => LocationFilterText);
      NotifyOfPropertyChange(() => LocationButtonText);
      NotifyOfPropertyChange(() => CanRevert);
    }


    public ObservableCollection<Location> Locations {
      get { return _database.Locations.Data; }    
    }

    public string LocationFilterText
    {
      get {
        string text = "Locations";
        if ( SelectedLocations.Count > 1 )
          text = string.Format("{0} ({1})", text, SelectedLocations.Count);
        else if (SelectedLocations.Count == Locations.Count && Locations.Count != 0)
          text = "Locations All";
        return text;
      }     
    }

    private ObservableCollection<Location> _selectedLocations;
    public ObservableCollection<Location> SelectedLocations
    {
      get { return _selectedLocations; }
      set
      {
        if (_selectedLocations != value)
        {
          _selectedLocations = value;
          RefreshUI();
        }
      }
    }

    public string LocationButtonText {
      get { return SelectedLocations.Count > 0 ? string.Empty : "Select location..."; }    
    }

    public bool CanRevert  {  get { return SelectedLocations.Count > 0; } }   
  }
  #endregion

  #region countryFilter
  public class CountryFilter : PropertyChangedBase, IFilterable
  {
    private readonly IProcessorLocator _locator;
    private readonly IBioSkyNetRepository _database;

    public CountryFilter(IProcessorLocator locator)
    {
      _locator = locator;
      _database = locator.GetProcessor<IBioSkyNetRepository>();          
    }

    public string[] CountryNames {
      get { return _database.BioCultureSources.CountriesNames; }
    }

    private string _selectedCountry;
    public string SelectedCountry
    {
      get { return _selectedCountry; }
      set
      {
        if (_selectedCountry != value)
        {
          _selectedCountry = value;

          NotifyOfPropertyChange(() => SelectedCountry);
          NotifyOfPropertyChange(() => CountryFilterText);
        }
      }
    }

    public string CountryFilterText
    {
      get {      
        string count = SelectedCountry == null ? "" : "(1)";
        return "Country" + count;
      }
    }

    public void Apply(QueryVisitors query)
    {
      if (SelectedCountry != null)
        query.Countries.Add(SelectedCountry);
    }

    public void Reset() { SelectedCountry = null; }
    public bool CanRevert { get { return SelectedCountry != null; } }
  }
  #endregion

  #region timeFilterItem
  public class TimeFilterItem : PropertyChangedBase
  {
    public TimeFilterItem(string name, DateInterval interval, TimeEnum enumState)
    {
      _name      = name     ;
      Interval   = interval ;
      _enumState = enumState;

      TimeFilter = new DateTimePeriod();
    }

    public DateTimePeriod GetInterval()
    {
      DateTime from = DateTime.Now;
      from = from.AddYears  (Interval.Year   );
      from = from.AddMonths (Interval.Month  );
      from = from.AddDays   (Interval.Day    );
      from = from.AddHours  (Interval.Hour   );
      from = from.AddMinutes(Interval.Minutes);
      from = from.AddSeconds(Interval.Seconds);

      TimeFilter.DateTimeFrom = from;
      TimeFilter.DateTimeTo   = new DateTime();

      return TimeFilter;
    }

    public void Reset()
    {
      IsActive = false;
    }

    public DateInterval Interval { get; set; }

    private string _name;
    public string Name
    {
      get { return _name; }
    }

    private DateTimePeriod _timeFilter;
    public DateTimePeriod TimeFilter
    {
      get { return _timeFilter; }
      set
      {
        if (_timeFilter != value)
        {
          _timeFilter = value;
        }
      }
    }

    private TimeEnum _enumState;
    public TimeEnum EnumState
    {
      get { return _enumState; }
    }

    private bool _isActive;
    public bool IsActive
    {
      get { return _isActive; }
      set
      {
        if (_isActive != value)
        {
          _isActive = value;
          NotifyOfPropertyChange(() => IsActive);
        }
      }
    }
  }
  #endregion

#region dateinterval
  public class DateInterval
  {
    public DateInterval(int year, int month, int day, int hour, int minutes, int seconds)
    {
      Year    = year   ;
      Month   = month  ;
      Day     = day    ;
      Hour    = hour   ;
      Minutes = minutes;
      Seconds = seconds;
    }
    public int Year    { get; set; }
    public int Month   { get; set; }
    public int Day     { get; set; }
    public int Hour    { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }

  }

  #endregion


  #region datetimePeriod
  public class DateTimePeriod
  {
    public void Reset()
    {
      DateTimeFrom = new DateTime();
      DateTimeTo   = new DateTime();
    }

    public bool IsEmpty()
    {
      return DateTimeFrom.Ticks == 0 && DateTimeTo.Ticks == 0;
    }
    public DateTime DateTimeFrom { get; set; }
    public DateTime DateTimeTo   { get; set; }

  }
  #endregion

  #region timefilter
  public class TimeFilter : PropertyChangedBase, IFilterable
  {
    private readonly IProcessorLocator    _locator      ;
    private readonly IBioSkyNetRepository _database     ;
    private          DialogsHolder        _dialogsHolder;

    private DateTimePeriod _period = new DateTimePeriod();

    private ObservableCollection<TimeFilterItem> _timeFilters;
    public ObservableCollection<TimeFilterItem> TimeFilters
    {
      get { return _timeFilters; }
      set
      {
        if (_timeFilters != value)
        {
          _timeFilters = value;
          NotifyOfPropertyChange(() => TimeFilters);
        }
      }
    }

    public TimeFilter(IProcessorLocator locator)
    {
      _locator       = locator;
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();

      TimeFilters   = new ObservableCollection<TimeFilterItem>();

      TimeFilterText = DefaultName;

      InitializeIntervals();
    }

    public DateTimePeriod GetPeriod()
    {
      return _period;
    }

    private void InitializeIntervals()
    {
      DateInterval intervalYear      = new DateInterval(-1, 0, 0, 0,  0, 0);
      DateInterval intervalMonth     = new DateInterval(0, -1, 0, 0,  0, 0);
      DateInterval intervalDay       = new DateInterval(0, 0, -1, 0,  0, 0);
      DateInterval intervalWeek      = new DateInterval(0, 0, -7, 0,  0, 0);
      DateInterval intervalHour      = new DateInterval(0, 0, 0, -1,  0, 0);
      DateInterval interval10Minutes = new DateInterval(0, 0, 0,  0,-10, 0);


      TimeFilterItem lastYear      = new TimeFilterItem("Last Year"      , intervalYear     , TimeEnum.LastYear     );
      TimeFilterItem lastMonth     = new TimeFilterItem("Last Month"     , intervalMonth    , TimeEnum.LastMonth    );
      TimeFilterItem last24Hours   = new TimeFilterItem("Last 24 Hours"  , intervalDay      , TimeEnum.Last24Hours  );
      TimeFilterItem lastWeek      = new TimeFilterItem("Last Week"      , intervalWeek     , TimeEnum.LastWeek     );
      TimeFilterItem lastHour      = new TimeFilterItem("Last Hour"      , intervalHour     , TimeEnum.LastHour     );
      TimeFilterItem last10Minutes = new TimeFilterItem("Last 10 Minutes", interval10Minutes, TimeEnum.Last10Minutes);


      TimeFilters.Add(lastYear     );
      TimeFilters.Add(lastMonth    );
      TimeFilters.Add(lastWeek     );
      TimeFilters.Add(last24Hours  );
      TimeFilters.Add(lastHour     );
      TimeFilters.Add(last10Minutes);
    }

    public void OnFilterClick(TimeEnum timeFilter)
    {
      foreach (TimeFilterItem item in TimeFilters)
      {
        if (timeFilter == item.EnumState)
        {
          bool flag = !item.IsActive;
          item.IsActive = flag;

          if(flag)
          {
            _period = item.GetInterval();
            TimeFilterText = item.Name;
          }
          else
          {
            _period.Reset();
            TimeFilterText = DefaultName;
          }
        }
        else
          item.IsActive = false;
      }
    }

    public void OnAllTimeClick()
    {
      Reset();
    }
    public void OnPeriodClick()
    {
      _dialogsHolder.PeriodTimePicker.Show();
      PeriodTimePickerResult result = _dialogsHolder.PeriodTimePicker.GetResult();

      if (result == null)
        return;
      
      TimeFilterText = result.FromDateString + "-" + result.ToDateString;

      _period.DateTimeFrom = result.FromDateLong;
      _period.DateTimeTo   = result.ToDateLong  ; 
    }

    private void RemoveAllFilters()
    {
      foreach (TimeFilterItem item in TimeFilters)
        item.Reset();

      TimeFilterText = DefaultName;
    }

    public void Apply(QueryVisitors query)
    {
      if (_period != null)
      {
        query.DatetimeFrom = _period.DateTimeFrom.Ticks;
        query.DatetimeTo   = _period.DateTimeTo.Ticks;
      }
    }

    public void Reset()
    {
      _period.Reset();
      RemoveAllFilters();
    }
    public bool CanRevert {
      get{ return (TimeFilterText == DefaultName) ? false : true;}
    }  

    private string DefaultName { get { return "Time"; } }

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
          NotifyOfPropertyChange(() => CanRevert);
        }
      }
    } 

  }
  #endregion 
  public class VisitorsFilterMenuViewModel : Screen
  {
    private ObservableCollection<IFilterable> _filters;
    public ObservableCollection<IFilterable> Filters
    {
      get { return _filters; }
      set
      {
        if (_filters != value)
        {
          _filters = value;
          NotifyOfPropertyChange(() => Filters);
        }
      }
    }

    public VisitorsFilterMenuViewModel(IProcessorLocator locator)
    {
      _locator       = locator;

      Filters = new ObservableCollection<IFilterable>();
      Filters.Add(new LocationFilter(locator));
      Filters.Add(new CountryFilter (locator));
      Filters.Add(new TimeFilter    (locator));

      _query = new QueryVisitors();
    }

    protected override void OnActivate()
    {
      foreach (IFilterable filter in Filters)
        Filters[0].PropertyChanged += OnFiltersChanged;

      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      foreach (IFilterable filter in Filters)
        Filters[0].PropertyChanged -= OnFiltersChanged;

      base.OnDeactivate(close);
    }

    public void ResetSettings()
    {
      foreach (IFilterable filter in Filters)
        filter.Reset();     
    }

   
    public void UpdateQuery()
    {
      foreach (IFilterable filter in Filters)
        filter.Apply(_query);     
    }

    public QueryVisitors GetQuery()
    {
      UpdateQuery();
      return _query;
    }

    public void OnFiltersChanged(object sender, PropertyChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => IsResetButtonEnabled);
    }
     

 
    public bool IsResetButtonEnabled
    {
      get
      {
        foreach (IFilterable filter in Filters)
        {
          if (filter.CanRevert)
            return true;
        }
        return false;
      }
    }

                     QueryVisitors     _query  ;
    private readonly IProcessorLocator _locator; 

  }

  public enum TimeEnum
  {
     None
   , AllTime
   , LastHour
   , Last24Hours
   , LastWeek
   , LastMonth
   , LastYear
   , Last10Minutes
   , Period
  }
}
