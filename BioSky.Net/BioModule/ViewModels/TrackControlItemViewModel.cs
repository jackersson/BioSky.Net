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
using BioModule.Utils;

namespace BioModule.ViewModels
{ 


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

      VisitorsView.UpdateLocation(location.CurrentLocation);

      CurrentLocation = location;
      location.Subscribe(this);
    }
    protected override void OnActivate()
    {
      if (_visitorsView != null)
        ActivateItem(_visitorsView);
      base.OnActivate();
    }

    public void OnError(Exception ex, LocationDevice device)
    {
      TrackLocationUtils.UpdateLocationState(false);
    }
    public void OnOk(bool ok) {
      TrackLocationUtils.UpdateLocationState(ok); 
    }

    public void OnVerificationFailure(Exception ex)
    {
      TrackLocationUtils.UpdateVerificationState(VerificationStatus.Failed, true);
    }

    public void OnVerificationSuccess(bool state)
    {
      TrackLocationUtils.UpdateVerificationState(VerificationStatus.Success, true);
    }

    public void OnVerificationProgress(int progress)
    {
      if ( progress < 100)
        TrackLocationUtils.UpdateVerificationState(VerificationStatus.Start, true);
      else
        TrackLocationUtils.UpdateVerificationState(VerificationStatus.Start, false);
    }
    public void OnCaptureDeviceFrameChanged(ref Bitmap frame) { }

    #endregion

    #region Interface
    private void Initialize(IProcessorLocator locator)
    {
      _locator            = locator;
      _bioEngine          = locator.GetProcessor<IBioEngine>();    
      _notifier           = locator.GetProcessor<INotifier> ();

      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Location");

      _visitorsView = new VisitorsViewModel(locator);
      Items.Add(_visitorsView);
    }    
    #endregion

    #region UI 
    
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

    private TrackLocationUtils _trackLocationUtils;
    public TrackLocationUtils TrackLocationUtils
    {
      get { return (_trackLocationUtils == null) ? _trackLocationUtils = new TrackLocationUtils(_locator) 
                                                 : _trackLocationUtils; }
    }
    #endregion

    #region Global Variables

    private IBioEngine           _bioEngine         ;
    private IProcessorLocator    _locator           ;   
    private INotifier            _notifier          ;
    #endregion

  }  
}
