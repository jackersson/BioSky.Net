using BioContracts;
using BioModule.Utils;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.ViewModels
{
  public class ErrorViewerDialogViewModel : Screen
  {
    private int PAGES_COUNT = 15;
    public ErrorViewerDialogViewModel(IProcessorLocator locator)
    {
      //LogRecords = records;
      DisplayName = "Error Viever";

      _locator       = locator;
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();

      _sortDescriptionByTime = new SortDescription("DetectedTime", ListSortDirection.Descending);
      PageController         = new PageControllerViewModel();
      LogRecords             = new ObservableCollection<LogRecord>();
      PeriodTimePicker       = new PeriodTimePickerViewModel(_locator);

      _bioFileUtils = new BioFileUtils();
    }    

    public void OnOpenFiles()
    {     

      var dialog = _bioFileUtils.OpenFileDialogWithMultiselect();
      if (dialog.ShowDialog() == true)
      {
        LogRecords.Clear();

        LogRecord newRecord;

        foreach (string fileName in dialog.FileNames)
        {
          using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
          {
            while (fs.Position != fs.Length)
            {
              try
              {
                newRecord = LogRecord.Parser.ParseDelimitedFrom(fs);
                if (newRecord == null)
                  break;
                LogRecords.Add(newRecord);
              }
              catch (Exception ex)
              {
                Console.WriteLine(ex);
                break;
              }
            }
          }
        }

        LogRecordsCollectionView = null;
        LogRecordsCollectionView = new PagingCollectionView(LogRecords, PAGES_COUNT);
        LogRecordsCollectionView.Sort(SortDescriptionByTime);

        PageController.UpdateData(LogRecordsCollectionView);
      }
    }

    public void OnSearch()
    {

    }

    public void OnPeriodCkick()
    {
      _dialogsHolder.PeriodTimePicker.Show();
     PeriodTimePickerResult result =  _dialogsHolder.PeriodTimePicker.Result;

      
      if (result == null || LogRecordsCollectionView == null)
        return;

      LogRecordsCollectionView.Filtering = item =>
      {
        LogRecord record = item as LogRecord;


        DateTime detectedTime = DateTime.FromBinary(record.DetectedTime);
        DateTime fromDateLong = DateTime.FromBinary(result.FromDateLong);
        DateTime toDateLong   = DateTime.FromBinary(result.ToDateLong);


        if (record != null)
          return (record.DetectedTime >= result.FromDateLong
                 && record.DetectedTime <= result.ToDateLong) ? true : false;

        return false;
      };

      PageController.UpdateMove();
    }

    public void OnClear()
    {
      LogRecords.Clear();
    }


    #region UI

    private SortDescription _sortDescriptionByTime;
    public SortDescription SortDescriptionByTime
    {
      get { return _sortDescriptionByTime; }
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

    private PeriodTimePickerViewModel _periodTimePicker;
    public PeriodTimePickerViewModel PeriodTimePicker
    {
      get { return _periodTimePicker; }
      set
      {
        if (_periodTimePicker != value)
        {
          _periodTimePicker = value;
          NotifyOfPropertyChange(() => PeriodTimePicker);
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

    private BioFileUtils      _bioFileUtils ;
    private IProcessorLocator _locator      ;
    private DialogsHolder     _dialogsHolder;

    #endregion
  }
}
