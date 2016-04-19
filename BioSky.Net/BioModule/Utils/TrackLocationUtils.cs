using BioContracts;
using BioModule.ResourcesLoader;
using BioModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Caliburn.Micro;

namespace BioModule.Utils
{
  public class TrackLocationUtils : PropertyChangedBase
  {
    public TrackLocationUtils(IProcessorLocator locator)
    {
      _notifier   = locator.GetProcessor<INotifier>();
      _dispatcher = locator.GetProcessor<Dispatcher>();

      _timer = new DispatcherTimer();
      _timer.Interval = new TimeSpan(0,0,1);
      _timer.Tick += _timer_Tick;
      UpdateVerificationState();
      UpdateLocationState();
    }
    public void _timer_Tick(object sender, EventArgs e)
    {
      _timer.Stop();
      UpdateVerificationState(VerificationStatus.Start, false);
    }

    public void UpdateLocationState(bool state = false)
    {
      try
      {
        _dispatcher.Invoke(() =>
        {
          BitmapSource target = state ? ResourceLoader.OkIconSource : ResourceLoader.WarningIconSource;
          target.Freeze();
          LocationStateIcon = target;
        }
        );
      }
      catch (Exception ex)
      {
        _notifier.Notify(ex);
      }     
    }

    public void UpdateVerificationState(VerificationStatus state = VerificationStatus.Start, bool visible = false)
    {
      try
      {
        UserVerificationIconVisible = visible;

        if (!visible)       
          return;       

        BitmapSource target;
        switch (state)
        {
          case VerificationStatus.Start:
            target = ResourceLoader.CardIconSource;
            break;
          case VerificationStatus.Success:
            target = ResourceLoader.VerificationIconSource;
            _timer.Start();
            break;
          case VerificationStatus.Failed:
            target = ResourceLoader.VerificationFailedIconSource;
            _timer.Start();
            break;

          default:
            target = ResourceLoader.VerificationFailedIconSource;
            break;
        }

        target.Freeze();        
        VerificationStateIcon = target;
      }
      catch (Exception ex)
      {
        _notifier.Notify(ex);
      }
    }

    private BitmapSource _locationStateIcon;
    public BitmapSource LocationStateIcon
    {
      get { return _locationStateIcon; }
      private set
      {
        try
        {
          if (_locationStateIcon != value)
          {
            _locationStateIcon = value;
            NotifyOfPropertyChange(() => LocationStateIcon);
          }
        }
        catch (TaskCanceledException ex)
        {
          _notifier.Notify(ex);
        }
      }
    }

    private BitmapSource _verificationStateIcon;
    public BitmapSource VerificationStateIcon
    {
      get { return _verificationStateIcon; }
      private set
      {
        try
        {
          if (_verificationStateIcon != value)
          {
            _verificationStateIcon = value;
            NotifyOfPropertyChange(() => VerificationStateIcon);
          }
        }
        catch (TaskCanceledException ex)
        {
          _notifier.Notify(ex);
        }
      }
    }

    private bool _userVerificationVisible;
    public bool UserVerificationIconVisible
    {
      get { return _userVerificationVisible; }
      set
      {
        if (_userVerificationVisible != value)
        {
          _userVerificationVisible = value;
          NotifyOfPropertyChange(() => UserVerificationIconVisible);
        }
      }
    }

    private Dispatcher      _dispatcher;
    private INotifier       _notifier  ;
    private DispatcherTimer _timer     ;
  }

  public enum VerificationStatus
  {
      Start
    , Success
    , Failed
  }
}
