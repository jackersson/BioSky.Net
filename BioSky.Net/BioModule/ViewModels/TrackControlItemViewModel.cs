using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using BioContracts;
using System.Drawing;
using WPFLocalizeExtension.Extensions;
using System;
using BioContracts.Common;
using AForge.Video.DirectShow;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Threading;
using BioContracts.Locations;

namespace BioModule.ViewModels
{ 
  enum VerificationStatus
  {
        Start
      , Success
      , Failed
  }

  public class TrackControlItemViewModel : Conductor<IScreen>.Collection.AllActive, IFullLocationObserver
  {
    public TrackControlItemViewModel(IProcessorLocator locator) {      
      Initialize(locator);
    }

    public TrackControlItemViewModel( IProcessorLocator locator, TrackLocation location )     
    {   
      Initialize(locator);

      if ( location != null )
       Update(location);
    }
    
    #region Update
    public void Update(TrackLocation location)
    {      
      location.Unsubscribe(this);      

      if (location == null)
        return;

      CurrentLocation = location;
      location.Subscribe(this);
    }
    protected override void OnActivate()
    {
      if (_visitorsView != null)
        ActivateItem(_visitorsView);
      base.OnActivate();
    }
    private void UpdateLocationState(bool state = false)
    {
      try
      {
        BitmapSource target = state ? ResourceLoader.OkIconSource : ResourceLoader.WarningIconSource;
        target.Freeze();
        LocationStateIcon = target;
      }
      catch (Exception ex)
      {
        _notifier.Notify(ex);
      }
    }

    private void UpdateVerificationState(VerificationStatus state = VerificationStatus.Start, bool visible = false)
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

    public void OnError(Exception ex) { UpdateLocationState(false); }
    public void OnOk(bool ok) { UpdateLocationState(ok); }
      
    public void OnVerificationFailure(Exception ex)
    {
      UpdateVerificationState(VerificationStatus.Failed, true);
    }

    public void OnVerificationSuccess(bool state)
    {
      UpdateVerificationState(VerificationStatus.Success, true);
    }

    public void OnVerificationProgress(int progress)
    {
      if ( progress < 100)
        UpdateVerificationState(VerificationStatus.Start, true);
      else
        UpdateVerificationState(VerificationStatus.Start, false);
    }

    private void _timer_Tick(object sender, EventArgs e)
    {
      _timer.Stop();
      UpdateVerificationState(VerificationStatus.Start, false);
    }

    public void OnCaptureDeviceFrameChanged(ref Bitmap frame) { }

    #endregion

    #region Interface
    private void Initialize(IProcessorLocator locator)
    {
      _locator   = locator;
      _bioEngine = locator.GetProcessor<IBioEngine>();    
      _notifier  = locator.GetProcessor<INotifier> ();
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Location");

      _visitorsView = new VisitorsViewModel(locator);
      _timer = new DispatcherTimer();
      _timer.Interval = new TimeSpan(1000);
      _timer.Tick += _timer_Tick;
      UpdateVerificationState();
      UpdateLocationState();

      Items.Add(_visitorsView);
    }    
    #endregion

    #region UI
    private BitmapSource _locationStateIcon;
    public BitmapSource LocationStateIcon
    {
      get {  return _locationStateIcon; }
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
        catch (TaskCanceledException ex)     {
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
        catch (TaskCanceledException ex) {
          _notifier.Notify(ex);
        }
      }
    } 
    
    private VisitorsViewModel _visitorsView;
    public VisitorsViewModel VisitorsView
    {
      get { return _visitorsView; }
      private set
      {
        if (_visitorsView != value)
        {
          _visitorsView = value;
          NotifyOfPropertyChange(() => VisitorsView);
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
  
    private TrackLocation _currentLocation;
    public TrackLocation CurrentLocation
    {
      get { return _currentLocation; }

      set
      {
        if (_currentLocation != value)
        {
          _currentLocation = value;
          NotifyOfPropertyChange(() => CurrentLocation);         
        }
      }
    }
    #endregion

    #region Global Variables
    private DispatcherTimer      _timer;
    private IBioEngine           _bioEngine;
    private IProcessorLocator    _locator  ;   
    private INotifier            _notifier ;
    #endregion

  }  
}
