using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.Windows;

namespace BioModule.ViewModels
{
  public class ProgressRingViewModel : PropertyChangedBase 
  {   
    public ProgressRingViewModel()
    {    
      //SetStyle(MIN_STYLE);
      Hide();
      //ShowWaiting("Waiting");
    }

    public void ShowWaiting(string text = "")
    {
      Message  = text;
      IsActive = true;
      SetStyle(CUSTOM_STYLE);
    }

    public void Hide()
    {
      Message  = string.Empty;
      Progress = string.Empty;
      IsActive = false;
      SetStyle(MIN_STYLE);
    }

    public void ShowProgress(int progress, bool status)
    {      
      Progress = progress + "%";
      if (progress == MAX_PROGRESS_VALUE)
       {
        // SetValues(Margin, true, true, false, false, false, progress + "%");
        Progress = string.Format("{0}%", progress);
        SetStyle(MAX_STYLE);
        StatusImageSource = status ? ResourceLoader.OkIconSource : ResourceLoader.CancelIconSource;

        //await Task.Delay(3000);

        Hide();
      }
      else
      {
        //SetValues(Margin, true, false, true, true, true, progress + "%");
        Progress = string.Format("{0}%", progress);
        SetStyle(TYPICAL_STYLE);
      }
    }
   
    private bool HasFlag(long currentStyle, ProgressRingStyle style)
    {
      long activityL = (long)style;
      return (currentStyle & activityL) == activityL;
    }

    public void SetStyle(long style) { ControlStyle = style; }

    private long _controlStyle;
    public long ControlStyle
    {
      get { return _controlStyle; }
      set
      {
        if (_controlStyle != value)
        {
          _controlStyle = value;
          NotifyOfPropertyChange(() => ControlStyle      );         
          NotifyOfPropertyChange(() => ProgressRingActive);
          NotifyOfPropertyChange(() => StatusImageActive );
          NotifyOfPropertyChange(() => ProgressRingActive);      
        }
      }
    }


    #region UI  

     private bool _isActive;
     public bool IsActive
     {
       get { return _isActive; }
       private set
       {
        if (_isActive != value)
        {
          _isActive = value;
          NotifyOfPropertyChange(() => IsActive);
        }
       }
     } 
    
     private string _progress;
     public string Progress
     {
      get { return _progress; }
      set
      {
        if (_progress != value)
        {
          _progress = value;
          NotifyOfPropertyChange(() => Progress         );
          //NotifyOfPropertyChange(() => ProgressActive);
        }
      }
    }

    private string _message;
    public string Message
    {
      get { return _message; }
      set
      {
        if (_message != value)
        {
          _message = value;
          NotifyOfPropertyChange(() => Message);
        }
      }
    }
  
    public bool StatusImageActive
     {
      get { return HasFlag(ControlStyle, ProgressRingStyle.StatusImage); }
    }

     public bool ProgressRingActive
     {
      get { return HasFlag(ControlStyle, ProgressRingStyle.ProgressRing); }
    }

     private BitmapSource _statusImageSource;
     public BitmapSource StatusImageSource
     {
       get {
         return _statusImageSource;
       }
       set
       {
         if (_statusImageSource != value)
         {
           _statusImageSource = value;
           NotifyOfPropertyChange(() => StatusImageSource);
         }
       }
     }
    #endregion

    #region global variables

    public enum ProgressRingStyle : long
    {  
        None         = 1 << 0
      , ProgressRing = 1 << 1
      , StatusImage  = 1 << 2  
      , Status       = 1 << 5      
    }

    public const int MAX_PROGRESS_VALUE = 100;

    public const long TYPICAL_STYLE  = (long)( ProgressRingStyle.ProgressRing | ProgressRingStyle.Status);
    public const long MIN_STYLE      = (long)(ProgressRingStyle.None);
    public const long MAX_STYLE      = (long)(ProgressRingStyle.ProgressRing | ProgressRingStyle.StatusImage);
    public const long CUSTOM_STYLE   = (long)(ProgressRingStyle.ProgressRing);


    #endregion
  }
}
