using BioContracts;
using BioModule.Utils;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFLocalizeExtension.Extensions;

namespace BioModule.ViewModels
{
  public class PeriodTimePickerViewModel : Screen
  {
    public PeriodTimePickerViewModel(IProcessorLocator locator)
    {
      _locator = locator;
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();
      _windowManager = _locator.GetProcessor<IWindowManager>(); ;
      DisplayName = "Period";
      //DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:PeriodTimePicker");  
      
    }
    public void Show()
    {
      _selectedDateFrom = _nullDateTime;
      _selectedDateTo   = _nullDateTime;
      Result = null;
      //SelectedTimeFrom = _nullDateTime;
      //SelectedTimeTo   = _nullDateTime;

      //SelectedTimeFrom = new DateTime(2016, 03, 22);
      //SelectedTimeTo   = new DateTime(2016, 03, 23);




      _windowManager.ShowDialog(this);
    }

    public void Hide()
    {
      TryClose();
    }

    public void Apply()
    {
      if (_selectedDateFrom == _nullDateTime || _selectedDateTo == _nullDateTime)
      {
        _dialogsHolder.CustomTextDialog.Update("Warning", "Please select a dates", DialogStatus.Error);
        _dialogsHolder.CustomTextDialog.Show();
        return;
      }
      DateTime fullDateFrom = new DateTime(_selectedDateFrom.Year, _selectedDateFrom.Month, _selectedDateFrom.Day
                                          , SelectedTimeFrom.Hour, SelectedTimeFrom.Minute, 0);
      DateTime fullDateTo   = new DateTime(_selectedDateTo.Year, _selectedDateTo.Month, _selectedDateTo.Day
                                          , SelectedTimeTo.Hour, SelectedTimeTo.Minute,0);
      if (Result == null)
        Result = new PeriodTimePickerResult();

      Result.FromDateLong = fullDateFrom.Ticks;
      Result.ToDateLong   = fullDateTo.Ticks  ;
            
      Result.FromDateString = fullDateFrom.ToShortDateString() + " " + fullDateFrom.ToShortTimeString();
      Result.ToDateString   = fullDateTo.ToShortDateString() + " " + fullDateTo.ToShortTimeString();


     /* SelectedTimeFrom = new DateTime(2016, 3, 22, 11, 00, 00);
      SelectedTimeTo = new DateTime(2016, 3, 23, 23, 00, 00);

      Result.FromDateLong = SelectedTimeFrom.Ticks;
      Result.ToDateLong = SelectedTimeTo.Ticks;*/


      TryClose(true);      
    }

    public PeriodTimePickerResult GetResult()
    {
      return Result;
    }

    private DateTime StringToDateTime(string date)
    {
      try
      {
        string dateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        DateTime dt = DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture);
        return dt;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      return new DateTime();
    }
    public void OnDateFromChanged(string date)
    {
      _selectedDateFrom = StringToDateTime(date);
    }

    public void OnDateToChanged(string date)
    {
      _selectedDateTo = StringToDateTime(date);
    }

    private PeriodTimePickerResult _result;
    public PeriodTimePickerResult Result
    {
      get { return _result; }
      set
      {
        if (_result != value)
          _result = value;

        NotifyOfPropertyChange(() => Result);
      }
    }

    private DateTime _selectedTimeFrom;
    public DateTime SelectedTimeFrom
    {
      get { return _selectedTimeFrom; }
      set
      {
        if (_selectedTimeFrom != value)        
          _selectedTimeFrom = value;     

        NotifyOfPropertyChange(() => SelectedTimeFrom);
      }
    }

    private DateTime _selectedTimeTo;
    public DateTime SelectedTimeTo
    {
      get { return _selectedTimeTo; }
      set
      {
        if (_selectedTimeTo != value)
          _selectedTimeTo = value;       

        NotifyOfPropertyChange(() => SelectedTimeTo);
      }
    }

    private DateTime _selectedDateFrom = new DateTime();
    private DateTime _selectedDateTo   = new DateTime();
    private DateTime _nullDateTime     = new DateTime();


    private readonly IWindowManager    _windowManager;
    private readonly IProcessorLocator _locator      ;
    private readonly DialogsHolder     _dialogsHolder;

  }

  public class PeriodTimePickerResult
  {
    public string FromDateString { get; set; }

    public string ToDateString { get; set; }

    public long FromDateLong { get; set; }
    public long ToDateLong { get; set; }
  }
}
