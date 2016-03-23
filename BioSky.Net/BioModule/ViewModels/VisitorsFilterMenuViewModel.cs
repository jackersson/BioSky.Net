using BioContracts;
using BioModule.Utils;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace BioModule.ViewModels
{
  public class VisitorsFilterMenuViewModel : Screen
  {
    public VisitorsFilterMenuViewModel(IProcessorLocator locator, int pagesCount)
    {
      _locator       = locator;
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioEngine     = _locator.GetProcessor<IBioEngine>();

      _pagesCount = pagesCount;

      TimeFilterText      = "Time";
      TimeFilterState     = TimeEnum.None;
      LocationFilterState = LocationsEnum.None;
      LocationButtonText  = "Select location...";
      LocationFilterText  = "Location";
      CountryFilterText   = "Countries";

      _query                 = new QueryVisitors();
      SelectedLocations      = new ObservableCollection<TrackLocation>();
      
      //CheckOnTimeFilters();
    }

    int _pagesCount;

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

    private long[] periodTime        = { 0, 0 };
    private long[] currentTimeFilter = { 0, 0 };


    public long[] GetTimeFilterInterval(TimeEnum state)
    {
      long fromTime = 0;
      long toTime = 0;

      DateTime nullDate = DateTime.Now;
      DateTime now = DateTime.Now;
      DateTime dateTo = now;
      DateTime dateFrom = now;

      switch (state)
      {
        case TimeEnum.AllTime:
          dateTo = nullDate;
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
      if(TimeFilterState != TimeEnum.AllTime)      
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

    #region Location

    public void OnLocationsAllClick()
    {
      foreach(TrackLocation location in LocationItems)      
        SelectedLocations.Add(location);
      
      LocationFilterText = "All locations";
      LocationFilterState = LocationsEnum.All;
    }

    public void OnLocationsNoneClick()
    {
      SelectedLocations.Clear();
      LocationFilterText = "Location";
      LocationFilterState = LocationsEnum.None;
    }

    public void OnSelectionLocationChanged(SelectionChangedEventArgs e)
    {
      IList selectedRecords   = e.AddedItems as IList;
      IList unselectedRecords = e.RemovedItems as IList;

      if (selectedRecords == null || unselectedRecords == null)
        return;

      foreach (TrackLocation location in selectedRecords)
        SelectedLocations.Add(location);

      foreach (TrackLocation location in unselectedRecords)
        SelectedLocations.Remove(location);

      bool isLocationSelect = (SelectedLocations.Count >= 1) ? true : false;
      if (isLocationSelect)
      {
        LocationButtonText = "";
        LocationFilterText = "Location" + " (" + SelectedLocations.Count + ")";
        LocationFilterState = LocationsEnum.Several;
      }
      else
      {
        LocationButtonText = "Select location...";
        LocationFilterState = LocationsEnum.None;
      }

      if (SelectedLocations.Count == LocationItems.Count)
        LocationFilterState = LocationsEnum.All;      
    }

    #endregion

    public void ResetSettings()
    {
      SelectedCountry = null;
      CountryFilterText = "Countries";

      TimeFilterState   = TimeEnum.None;
      currentTimeFilter = null;

      SelectedLocations.Clear();
      LocationFilterText = "Location";
      LocationFilterState = LocationsEnum.None;

      IsResetButtonEnabled = false;
    }

    #region Query
    public void UpdateQuery()
    {
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
    }

    public QueryVisitors GetQuery()
    {
      UpdateQuery();
      return _query;
    }

    #endregion

    #region UI

    #region Country
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
          if (_selectedCountry != null)
          {
            CountryFilterText = "Countries" + " (" + 1 + ")";
            IsResetButtonEnabled = true;
          }

          NotifyOfPropertyChange(() => SelectedCountry);
        }
      }
    }

    private string _countryFilterText;
    public string CountryFilterText
    {
      get { return _countryFilterText; }
      set
      {
        if (_countryFilterText != value)
        {
          _countryFilterText = value;
          NotifyOfPropertyChange(() => CountryFilterText);
        }
      }
    }

    #endregion

    #region Location
    public AsyncObservableCollection<TrackLocation> LocationItems
    {
      get { return _bioEngine.TrackLocationEngine().TrackLocations; }
    }

    private LocationsEnum _locationFilterState;
    public LocationsEnum LocationFilterState
    {
      get { return _locationFilterState; }
      set
      {
        if (_locationFilterState != value)
        {
          _locationFilterState = value;
          if (_locationFilterState != LocationsEnum.None)
            IsResetButtonEnabled = true;
                      
          NotifyOfPropertyChange(() => LocationFilterState);
        }
      }
    }

    private string _locationFilterText;
    public string LocationFilterText
    {
      get { return _locationFilterText; }
      set
      {
        if (_locationFilterText != value)
        {
          _locationFilterText = value;
          NotifyOfPropertyChange(() => LocationFilterText);
        }
      }
    }

    private ObservableCollection<TrackLocation> _selectedLocations;
    public ObservableCollection<TrackLocation> SelectedLocations
    {
      get { return _selectedLocations; }
      set
      {
        if (_selectedLocations != value)
        {
          _selectedLocations = value;
          NotifyOfPropertyChange(() => SelectedLocations);
        }
      }
    }

    private string _locationButtonText;
    public string LocationButtonText
    {
      get { return _locationButtonText; }
      set
      {
        if (_locationButtonText != value)
        {
          _locationButtonText = value;
          NotifyOfPropertyChange(() => LocationButtonText);
        }
      }
    }

    #endregion

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
          if (_timeFilterState != TimeEnum.None)
            IsResetButtonEnabled = true;
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
          if(_isYearChecked)
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
          if(_isMonthChecked)
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
          if(_isWeekChecked)
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
          if(_is24HoursChecked)
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
          if(_isLastHourChecked)
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
          if(_isAllTimeChecked)
            SetTimeFilterState(TimeEnum.AllTime);

          NotifyOfPropertyChange(() => IsAllTimeChecked);
        }
      }
    }
    #endregion

    private bool _isResetButtonEnabled;
    public bool IsResetButtonEnabled
    {
      get { return _isResetButtonEnabled; }
      set
      {
        if (_isResetButtonEnabled != value)
        {
          _isResetButtonEnabled = value;
          NotifyOfPropertyChange(() => IsResetButtonEnabled);
        }
      }
    }

    #endregion

    #region GlobalVariables

    QueryVisitors _query;
    private readonly IProcessorLocator    _locator      ;
    private readonly DialogsHolder        _dialogsHolder;
    private readonly IBioSkyNetRepository _database     ;
    private readonly IBioEngine           _bioEngine    ;


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

  public enum LocationsEnum
  {
     None
   , All
   , Several
  }
}
