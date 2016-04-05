using BioContracts;
using BioModule.Utils;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace BioModule.ViewModels
{
  public class ErrorViewerViewModel : Screen
  {
    private int PAGES_COUNT = 15;
    public ErrorViewerViewModel(IProcessorLocator locator)
    {      
      DisplayName = "Error Viever";
     
      _dialogsHolder = locator.GetProcessor<DialogsHolder>();
      _database      = locator.GetProcessor<IBioSkyNetRepository>();

      _sortDescriptionByTime = new SortDescription("DetectedTime", ListSortDirection.Descending);
      PageController         = new PageControllerViewModel();
      LogRecords             = new ObservableCollection<LogRecord>();

      Filters                = new ObservableCollection<TimeFilter>();
      CurrentDateTimePeriod  = new DateTimePeriod();

      Filters.Add(new TimeFilter(locator));

      _bioFileUtils = new BioFileUtils();

      logFileFormat = _database.LocalStorage.LogFileFormat;
    }

    protected override void OnActivate()
    {
      foreach (TimeFilter filter in Filters)
        filter.PropertyChanged += OnFiltersChanged;
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      foreach (TimeFilter filter in Filters)
        filter.PropertyChanged -= OnFiltersChanged;
      base.OnDeactivate(close);
    }

    public void OnFiltersChanged(object sender, PropertyChangedEventArgs e)
    {
      foreach (TimeFilter filter in Filters)
        CurrentDateTimePeriod = filter.GetPeriod(); 
      UploadByFilter();
    }
    
    private void UploadByFilter()
    {
      OnClear();

      string directoryPath = _database.LocalStorage.LogDirectoryPath;

      if (CurrentDateTimePeriod.IsEmpty())
      {
        string[] files = Directory.GetFiles(directoryPath, "*" + logFileFormat, SearchOption.AllDirectories);
        LoadRecords(files);
        UpdateCollectionView();
        return;
      }           

      DateTime dateTo = (CurrentDateTimePeriod.DateTimeTo.Ticks == 0) 
                                                                    ? DateTime.Now 
                                                                    : CurrentDateTimePeriod.DateTimeTo;

      DateTime dateFrom = CurrentDateTimePeriod.DateTimeFrom;

      while (dateFrom.Date <= dateTo.Date)
      {
        string month = (dateFrom.Month < 10) ? "0" + dateFrom.Month : dateFrom.Month.ToString();

        string path = string.Format("{0}\\{1}\\{2}", dateFrom.Year, month, dateFrom.Day);        
      
        string fullPath = directoryPath + path;

        dateFrom = dateFrom.AddDays(1);

        if (!Directory.Exists(fullPath))
          continue;

        string[] files = Directory.GetFiles(fullPath, "*" + logFileFormat);

        LoadRecords(files);
      }

      UpdateCollectionView();
    }

    public void UploadFromDirectory()
    {
      string directoryPath = _database.LocalStorage.LogDirectoryPath;

      var dialog = _bioFileUtils.OpenFileDialogWithMultiselect(directoryPath);
      if (dialog.ShowDialog() == true)
      {
        foreach (TimeFilter filter in Filters)
          filter.Reset();

        OnClear();
        LoadRecords(dialog.FileNames);      

        UpdateCollectionView();
      }
    }

    private void UpdateCollectionView()
    {
      LogRecordsCollectionView = null;
      LogRecordsCollectionView = new PagingCollectionView(LogRecords, PAGES_COUNT);
      LogRecordsCollectionView.Sort(_sortDescriptionByTime);
      
      PageController.UpdateData(LogRecordsCollectionView);

      if(!CurrentDateTimePeriod.IsEmpty())
        FilteringList();

      NotifyOfPropertyChange(() => PeriodText);
    }
    private void FilteringList()
    {
      if (LogRecordsCollectionView == null)
        return;

      LogRecordsCollectionView.Filtering = item =>
      {
        LogRecord record = item as LogRecord;

        long dateTimeTo = (CurrentDateTimePeriod.DateTimeTo.Ticks == 0) 
                                                                       ? DateTime.Now.Ticks 
                                                                       : CurrentDateTimePeriod.DateTimeTo.Ticks;

        if (record != null)
          return (record.DetectedTime >= CurrentDateTimePeriod.DateTimeFrom.Ticks
                 && record.DetectedTime <= dateTimeTo) ? true : false;

        return false;
      };      
    }

    private void LoadRecords(string[] fileNames)
    {       
      foreach (string filePath in fileNames)
      {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
          while (fs.Position != fs.Length)
          {
            try  {
              LogRecord newRecord = LogRecord.Parser.ParseDelimitedFrom(fs);
              if (newRecord != null)
                LogRecords.Add(newRecord);
            }
            catch (Exception ex) {
              Console.WriteLine(ex);
              break;
            }
          }
        }
      }
    }

    public void OnPeriodClick()
    {
      _dialogsHolder.PeriodTimePicker.Show();
     PeriodTimePickerResult result =  _dialogsHolder.PeriodTimePicker.Result;
      
      if (result == null || LogRecordsCollectionView == null)
        return;

      CurrentDateTimePeriod.DateTimeFrom = result.FromDateLong;
      CurrentDateTimePeriod.DateTimeTo   = result.ToDateLong;

      FilteringList();
    }

    public void OnClear()
    {
      LogRecords.Clear();
      NotifyOfPropertyChange(() => PeriodText);
      PageController.UpdateMove();
    }


    #region UI

    private string _periodText;
    public string PeriodText
    {
      get {
        string dateFrom = "";
        string dateTo   = "";

        if (LogRecords.Count > 0)
        {
          LogRecord record = LogRecords.FirstOrDefault();
          if(record != null)
            dateFrom = new DateTime(LogRecords.FirstOrDefault().DetectedTime).ToString();

          record = LogRecords.LastOrDefault();
          if (record != null)
            dateTo = new DateTime(LogRecords.LastOrDefault().DetectedTime).ToString();
        }

        _periodText = (LogRecords.Count == 0) ? "No Files" : string.Format("{0} - {1}", dateFrom, dateTo);

        return _periodText;
      }
    }

    private DateTimePeriod _currentDateTimePeriod;
    public DateTimePeriod CurrentDateTimePeriod
    {
      get { return _currentDateTimePeriod; }
      set
      {
        if (_currentDateTimePeriod != value)
        {
          _currentDateTimePeriod = value;
          NotifyOfPropertyChange(() => CurrentDateTimePeriod);
        }
      }
    }

    private ObservableCollection<TimeFilter> _filters;
    public ObservableCollection<TimeFilter> Filters
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

    private IPagingCollectionView _logRecordsCollectionView;
    public IPagingCollectionView LogRecordsCollectionView
    {
      get { return _logRecordsCollectionView; }
      set
      {
        if (_logRecordsCollectionView != value)
        {
          _logRecordsCollectionView = value;
          NotifyOfPropertyChange(() => LogRecordsCollectionView);
        }
      }
    }

    private ObservableCollection<LogRecord> _logRecords;   
    public ObservableCollection<LogRecord> LogRecords
    {
      get { return _logRecords; }
      set
      {
        if (_logRecords != value)
        {
          _logRecords = value;
          NotifyOfPropertyChange(() => LogRecords);
        }
      }
    }
    #endregion

    #region GlobalVariables
    private string               logFileFormat         ;
    private SortDescription      _sortDescriptionByTime;
    private BioFileUtils         _bioFileUtils         ;
    private DialogsHolder        _dialogsHolder        ;
    private IBioSkyNetRepository _database             ;
    #endregion
  }
}
