using BioContracts;
using BioModule.Utils;
using BioModule.ViewModels;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using WPFLocalizeExtension.Extensions;

namespace BioModule.ViewModels
{
  public interface IFilterable
  {
    void Apply(QueryVisitors query);
    void Reset();
    bool CanRevert { get; }
  }

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


    public ObservableCollection<Location> Locations
    {
      get { return _database.Locations.Data; }
     /* get
      {
        //for test
        return new ObservableCollection<Location>() { new Location() { LocationName = "location 1" }
                                                     , new Location() { LocationName = "location 2" }
                                                     , new Location() { LocationName = "location 3" }
                                                     , new Location() { LocationName = "location 4" }
                                                     , new Location() { LocationName = "location 5" }
                                                     , new Location() { LocationName = "location 6" }
                                                     , new Location() { LocationName = "location 7" }        };
      }*/
    }

    public string LocationFilterText
    {
      get {
        string text = "Locations";
        if ( SelectedLocations.Count > 1 )
          text = string.Format("{0} ({1})", text, SelectedLocations.Count);
        else if (SelectedLocations.Count == Locations.Count)
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

    public string LocationButtonText
    {
      get { return SelectedLocations.Count > 0 ? string.Empty : "Select location..."; }    
    }

    public bool CanRevert  {  get { return SelectedLocations.Count > 0; } }   

  }

  public class CountryFilter : PropertyChangedBase, IFilterable
  {
    private readonly IProcessorLocator _locator;
    private readonly IBioSkyNetRepository _database;

    public CountryFilter(IProcessorLocator locator)
    {
      _locator = locator;
      _database = locator.GetProcessor<IBioSkyNetRepository>();      
    }

    public string[] CountryNames
    {
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
      get
      {
        string text = "Country";
        string count = SelectedCountry == null ? "" : "(1)";
        return text + count;
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

  public class TimeFilterItem
  {
    public TimeFilterItem(string name, DateInterval interval)
    {
      Name     = name    ;
      Interval = interval;
    }

    public long[] GetInterval(DateInterval interval)
    {
      long fromTime = 0;

      DateTime from = DateTime.Now;
      from.AddYears  (Interval.Year   );
      from.AddMonths (Interval.Month  );
      from.AddDays   (Interval.Day    );
      from.AddHours  (Interval.Hour   );
      from.AddMinutes(Interval.Minutes);
      from.AddSeconds(Interval.Seconds);

      fromTime = from.Ticks;

      long[] timeFilter = { fromTime, 0 };

      return timeFilter;
    }

    public DateInterval Interval { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }
  }

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

  public class TimeFilter : PropertyChangedBase, IFilterable
  {
    private readonly IProcessorLocator    _locator      ;
    private readonly IBioSkyNetRepository _database     ;
    private          DialogsHolder        _dialogsHolder;

    private long[] periodTime = { 0, 0 };
    private long[] currentTimeFilter = { 0, 0 };

    int[] l = { 0, 1, 1, 0 };


    public TimeFilter(IProcessorLocator locator)
    {
      _locator       = locator;
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();


    }

    private void InitializeIntervals()
    {
      DateInterval intervalYear  = new DateInterval(-1, 0, 0, 0, 0, 0);
      DateInterval intervalMonth = new DateInterval(0, -1, 0, 0, 0, 0);
      DateInterval intervalDay   = new DateInterval(0, 0, -1, 0, 0, 0);
      DateInterval intervalWeek  = new DateInterval(0, 0, -7, 0, 0, 0);
      DateInterval intervalHour  = new DateInterval(0, 0, 0, -1, 0, 0);


    }

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




    public long[] GetTimeFilterInterval(TimeEnum state)
    {
      long fromTime = 0;
      long toTime = 0;

      DateTime nullDate = new DateTime();
      DateTime now = DateTime.Now;
      DateTime dateTo = now;
      DateTime dateFrom = now;

      DateTime date1 = new DateTime(0, 0, 0, 1, 0, 0);
      DateTime date2 = new DateTime(0, 0, 1, 0, 0, 0);

      DateTime date3 = DateTime.Now;
      date3.AddHours(-1 * date1.Hour);

      switch (state)
      {
        case TimeEnum.AllTime:
          dateTo = now;
          dateFrom = nullDate;
          TimeFilterText = "All Time";
          break;

        case TimeEnum.LastHour:
          dateFrom = now.AddHours(-1);
          TimeFilterText = "Last Hour";
          break;

        case TimeEnum.Last24Hours:
          dateFrom = now.AddHours(-24);
          TimeFilterText = "Last 24 Hours";
          break;

        case TimeEnum.LastMonth:
          dateFrom = now.AddMonths(-1);
          TimeFilterText = "Last Month";
          break;

        case TimeEnum.LastWeek:
          dateFrom = now.AddDays(-7);
          TimeFilterText = "Last Week";
          break;

        case TimeEnum.LastYear:
          dateFrom = now.AddYears(-7);
          TimeFilterText = "Last Year";
          break;
      }

      fromTime = dateFrom.Ticks;
      toTime = dateTo.Ticks;

      long[] timeFilter = { fromTime, toTime };

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

    public void CheckOnTimeFilters()
    {
      if (TimeFilterState != TimeEnum.AllTime)
        IsAllTimeChecked = false;

      if (TimeFilterState != TimeEnum.Last24Hours)
        Is24HoursChecked = false;

      if (TimeFilterState != TimeEnum.LastHour)
        IsLastHourChecked = false;

      if (TimeFilterState != TimeEnum.LastMonth)
        IsMonthChecked = false;

      if (TimeFilterState != TimeEnum.LastWeek)
        IsWeekChecked = false;

      if (TimeFilterState != TimeEnum.LastYear)
        IsYearChecked = false;
    }

    public void Apply(QueryVisitors query)
    {
      if (currentTimeFilter != null)
      {
        query.DatetimeFrom = currentTimeFilter[0];
        query.DatetimeTo = currentTimeFilter[1];
      }
    }

    public void Reset()
    {
      TimeFilterState = TimeEnum.None;
      currentTimeFilter = null;
    }
    public bool CanRevert
    {
      get
      {
        throw new NotImplementedException();
      }
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
          //if (_timeFilterState != TimeEnum.None)
          //  IsResetButtonEnabled = true;
          if (_timeFilterState == TimeEnum.None)
            TimeFilterText = "Time";

          CheckOnTimeFilters();
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
          if (_isYearChecked)
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
          if (_isMonthChecked)
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
          if (_isWeekChecked)
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
          if (_is24HoursChecked)
            SetTimeFilterState(TimeEnum.Last24Hours);

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
          if (_isLastHourChecked)
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
          if (_isAllTimeChecked)
            SetTimeFilterState(TimeEnum.AllTime);

          NotifyOfPropertyChange(() => IsAllTimeChecked);
        }
      }
    }
  }




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
      _locator = locator;
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();
      _database = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioEngine = _locator.GetProcessor<IBioEngine>();

      Filters = new ObservableCollection<IFilterable>();
      Filters.Add(new LocationFilter(locator));
      Filters.Add(new CountryFilter(locator));

      TimeFilterText = "Time";
      TimeFilterState = TimeEnum.None;

      _query = new QueryVisitors();
    }



    #region Time

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

    private long[] periodTime = { 0, 0 };
    private long[] currentTimeFilter = { 0, 0 };


    public long[] GetTimeFilterInterval(TimeEnum state)
    {
      long fromTime = 0;
      long toTime = 0;

      DateTime nullDate = new DateTime();
      DateTime now = DateTime.Now;
      DateTime dateTo = now;
      DateTime dateFrom = now;

      switch (state)
      {
        case TimeEnum.AllTime:
          dateTo = now;
          dateFrom = nullDate;
          TimeFilterText = "All Time";
          break;

        case TimeEnum.LastHour:
          dateFrom = now.AddHours(-1);
          TimeFilterText = "Last Hour";
          break;

        case TimeEnum.Last24Hours:
          dateFrom = now.AddHours(-24);
          TimeFilterText = "Last 24 Hours";
          break;

        case TimeEnum.LastMonth:
          dateFrom = now.AddMonths(-1);
          TimeFilterText = "Last Month";
          break;

        case TimeEnum.LastWeek:
          dateFrom = now.AddDays(-7);
          TimeFilterText = "Last Week";
          break;

        case TimeEnum.LastYear:
          dateFrom = now.AddYears(-7);
          TimeFilterText = "Last Year";
          break;
      }

      fromTime = dateFrom.Ticks;
      toTime = dateTo.Ticks;

      long[] timeFilter = { fromTime, toTime };

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

    public void CheckOnTimeFilters()
    {
      if (TimeFilterState != TimeEnum.AllTime)
        IsAllTimeChecked = false;

      if (TimeFilterState != TimeEnum.Last24Hours)
        Is24HoursChecked = false;

      if (TimeFilterState != TimeEnum.LastHour)
        IsLastHourChecked = false;

      if (TimeFilterState != TimeEnum.LastMonth)
        IsMonthChecked = false;

      if (TimeFilterState != TimeEnum.LastWeek)
        IsWeekChecked = false;

      if (TimeFilterState != TimeEnum.LastYear)
        IsYearChecked = false;
    }

    #endregion   

    public void ResetSettings()
    {
      foreach (IFilterable filter in Filters)
        filter.Reset();
      /*
      SelectedCountry = null;
      CountryFilterText = "Countries";

      TimeFilterState   = TimeEnum.None;
      currentTimeFilter = null;

      SelectedLocations.Clear();
      LocationFilterText = "Location";
      LocationFilterState = LocationsEnum.None;

      IsResetButtonEnabled = false;
      */
    }

    #region Query
    public void UpdateQuery()
    {
      foreach (IFilterable filter in Filters)
        filter.Apply(_query);
      /*
      _query.ItemsPerPage = _pagesCount;

      if (SelectedLocations != null || SelectedLocations.Count > 0)
      {
        foreach (TrackLocation trackLocation in SelectedLocations)
          _query.Locations.Add(trackLocation.CurrentLocation.Id);
      }

      if (SelectedCountry != null)
        _query.Countries.Add(SelectedCountry);

      if (currentTimeFilter != null)
      {
        _query.DatetimeFrom = currentTimeFilter[0];
        _query.DatetimeTo   = currentTimeFilter[1];
      }
      */
    }

    public QueryVisitors GetQuery()
    {
      UpdateQuery();
      return _query;
    }

    #endregion

    #region UI    

    #region Time

    private TimeEnum _timeFilterState;
    public TimeEnum TimeFilterState
    {
      get { return _timeFilterState; }
      set
      {
        if (_timeFilterState != value)
        {
          _timeFilterState = value;
          //if (_timeFilterState != TimeEnum.None)
          //  IsResetButtonEnabled = true;
          if (_timeFilterState == TimeEnum.None)
            TimeFilterText = "Time";

          CheckOnTimeFilters();
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
          if (_isYearChecked)
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
          if (_isMonthChecked)
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
          if (_isWeekChecked)
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
          if (_is24HoursChecked)
            SetTimeFilterState(TimeEnum.Last24Hours);

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
          if (_isLastHourChecked)
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
          if (_isAllTimeChecked)
            SetTimeFilterState(TimeEnum.AllTime);

          NotifyOfPropertyChange(() => IsAllTimeChecked);
        }
      }
    }
    #endregion

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

    #endregion

    #region GlobalVariables

    QueryVisitors _query;
    private readonly IProcessorLocator _locator;
    private readonly DialogsHolder _dialogsHolder;
    private readonly IBioSkyNetRepository _database;
    private readonly IBioEngine _bioEngine;


    #endregion
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
   , Period
  }
}
