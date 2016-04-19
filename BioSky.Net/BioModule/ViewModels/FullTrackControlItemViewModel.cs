using BioContracts;
using BioContracts.Common;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using System.Drawing;
using WPFLocalizeExtension.Extensions;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.Windows.Threading;
using BioService;
using BioContracts.Locations;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class FullTrackControlItemViewModel : Screen, IFullLocationObserver
  {
    public FullTrackControlItemViewModel(IProcessorLocator locator)
    {
      _locator   = locator;
      _bioEngine = locator.GetProcessor<IBioEngine>();

      BioImageView = new BioImageViewModel(locator, MIN_BIO_IMAGE_STYLE);

      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Location");
      
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      ActivateTimeDispatcher();
    }

    protected override void OnDeactivate(bool close)
    {
      _dayTimer.Tick -= dayTimer_Tick;
      base.OnDeactivate(close);
    }

    public void Update(TrackLocation location)
    {
      if (CurrentLocation != null)
        CurrentLocation.Unsubscribe(this);

      BioImageView.SetSingleImage(null);
      CurrentLocation = location;

      if (location == null)
        return;

      location.Subscribe(this);
    }

    private void ActivateTimeDispatcher()
    {
      if (!IsActive)
        return;

      if(_dayTimer == null)
        _dayTimer = new DispatcherTimer();

      _dayTimer.Interval = TimeSpan.FromMilliseconds(500);
      _dayTimer.Tick += dayTimer_Tick;
      _dayTimer.Start();
    }

    void dayTimer_Tick(object sender, EventArgs e)
    {
      _currentDatetime = DateTime.Now.ToString();
      NotifyOfPropertyChange(() => CurrentDatetime);
    }


    public void OnOk(bool ok)
    {
      CurrentTrackLocationUtils.UpdateLocationState(ok);
    }

    public void OnStartVerificationByCard(string cardNumber)
    {
      //throw new NotImplementedException();
    }
        
    public void OnCaptureDeviceFrameChanged(ref Bitmap frame)
    {
      BioImageView.UpdateFrame(frame);
    }

    public void OnError(Exception ex, LocationDevice device)
    {
      CurrentTrackLocationUtils.UpdateLocationState(false);
      if (device == LocationDevice.CaptureDevice)
        BioImageView.UpdateFrame(null);
    }

    public void OnVerificationFailure(Exception ex)
    {
      CurrentTrackLocationUtils.UpdateVerificationState(VerificationStatus.Failed, true);
    }

    public void OnVerificationSuccess(bool state)
    {
      CurrentTrackLocationUtils.UpdateVerificationState(VerificationStatus.Success, true);
    }

    public void OnVerificationProgress(int progress)
    {
      if (progress < 100)
        CurrentTrackLocationUtils.UpdateVerificationState(VerificationStatus.Start, true);
      else
        CurrentTrackLocationUtils.UpdateVerificationState(VerificationStatus.Start, false);
    }

    public TrackLocationUtils _currentTrackLocationUtils;
    public TrackLocationUtils CurrentTrackLocationUtils
    {
      get{ return (_currentTrackLocationUtils == null)? _currentTrackLocationUtils = new TrackLocationUtils(_locator) 
                                                      : _currentTrackLocationUtils; }
    }

    private TrackLocation _currentLocation;
    public TrackLocation CurrentLocation
    {
      get { return _currentLocation; }
      private set
      {
        if (_currentLocation != value)
        {
          _currentLocation = value;
          NotifyOfPropertyChange(() => CurrentLocation);
        }
      }
    }

    private BioImageViewModel _bioImageView;
    public BioImageViewModel BioImageView
    {
      get {  return _bioImageView; }
      private set
      {
        if (_bioImageView != value)
        {
          _bioImageView = value;
          NotifyOfPropertyChange(() => BioImageView);
        }
      }
    }

    private string _currentDatetime;
    public string CurrentDatetime
    {
      get { return _currentDatetime; }
    }

    //private readonly Dispatcher _uiDispatcher;
    private readonly IBioEngine _bioEngine;
    private long MIN_BIO_IMAGE_STYLE = 1;
    private DispatcherTimer _dayTimer;
    private readonly IProcessorLocator _locator;
  }
}
