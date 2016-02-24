using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;

namespace BioModule.ViewModels
{
  public class ProgressRingViewModel : Screen
   {
     public ProgressRingViewModel()
     {
       SetValues();
     }

     public async void ShowProgress(int progress, bool status)
     {         
       if (progress == 100)
       {         
         SetValues(true, true, false, false, false, progress + "%");
         ProgressRingIconSource = status ? ResourceLoader.OkIconSource : ResourceLoader.CancelIconSource;
       
         await Task.Delay(3000);
         ProgressRingVisibility = false;
       }
       else       
         SetValues(true, false, true, true, true, progress + "%");       
     }

     private void SetValues( bool   RingVisibility         = false
                           , bool   RingImageVisibility    = false
                           , bool   RingTextVisibility     = false
                           , bool   RingProgressVisibility = false
                           , bool   RingStatus             = false
                           , string RingText = "0")
     {
       ProgressRingVisibility         = RingVisibility;
       ProgressRingImageVisibility    = RingTextVisibility;
       ProgressRingTextVisibility     = RingTextVisibility;
       ProgressRingProgressVisibility = RingProgressVisibility;
       ProgressRingStatus             = RingStatus;
       ProgressRingText               = RingText;
     }

     #region UI

     private bool _progressRingVisibility;
     public bool ProgressRingVisibility
     {
       get { return _progressRingVisibility; }
       set
       {
         if (_progressRingVisibility != value)
         {
           _progressRingVisibility = value;
           NotifyOfPropertyChange(() => ProgressRingVisibility);
         }
       }
     }

     private bool _progressRingStatus;
     public bool ProgressRingStatus
     {
       get { return _progressRingStatus; }
       set
       {
         if (_progressRingStatus != value)
         {
           _progressRingStatus = value;
           NotifyOfPropertyChange(() => ProgressRingStatus);
         }
       }
     }

     private string _progressRingText;
     public string ProgressRingText
     {
       get { return _progressRingText; }
       set
       {
         if (_progressRingText != value)
         {
           _progressRingText = value;
           NotifyOfPropertyChange(() => ProgressRingText);
         }
       }
     }

     private bool _progressRingTextVisibility;
     public bool ProgressRingTextVisibility
     {
       get { return _progressRingTextVisibility; }
       set
       {
         if (_progressRingTextVisibility != value)
         {
           _progressRingTextVisibility = value;
           NotifyOfPropertyChange(() => ProgressRingTextVisibility);
         }
       }
     }

     private bool _progressRingImageVisibility;
     public bool ProgressRingImageVisibility
     {
       get { return _progressRingImageVisibility; }
       set
       {
         if (_progressRingImageVisibility != value)
         {
           _progressRingImageVisibility = value;
           NotifyOfPropertyChange(() => ProgressRingImageVisibility);
         }
       }
     }

     private bool _progressRingProgressVisibility;
     public bool ProgressRingProgressVisibility
     {
       get { return _progressRingProgressVisibility; }
       set
       {
         if (_progressRingProgressVisibility != value)
         {
           _progressRingProgressVisibility = value;
           NotifyOfPropertyChange(() => ProgressRingProgressVisibility);
         }
       }
     }

     private BitmapSource _progressRingIconSource;
     public BitmapSource ProgressRingIconSource
     {
       get
       {
         return _progressRingIconSource;
       }
       set
       {
         if (_progressRingIconSource != value)
         {
           _progressRingIconSource = value;
           NotifyOfPropertyChange(() => ProgressRingIconSource);
         }
       }
     }
     #endregion
   }
}
