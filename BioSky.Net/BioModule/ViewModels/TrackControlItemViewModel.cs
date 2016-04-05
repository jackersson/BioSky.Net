using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using BioContracts;
using System.Drawing;
using WPFLocalizeExtension.Extensions;
using System;
using BioContracts.Common;
using AForge.Video.DirectShow;

namespace BioModule.ViewModels
{
  public class TrackControlItemViewModel : Conductor<IScreen>.Collection.AllActive, IAccessDeviceObserver, ICaptureDeviceObserver
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

    public void OnBioImageViewLoaded()
    {
      if (ImageView == null)
        ImageView = new BioImageViewModel(_locator);
    }

    public void Update(TrackLocation location)
    {
      if (CurrentLocation != null)
      {
     //   CurrentLocation.FrameChanged    -= OnNewFrame;
       // CurrentLocation.PropertyChanged -= OnLocationStatusChanged;
      }

      if (location == null)
        return;

      CurrentLocation = location;

    //  CurrentLocation.PropertyChanged += OnLocationStatusChanged;    
     // trackLocation.FrameChanged      += OnNewFrame;      
    }
    #endregion

    #region Interface
    protected override void OnActivate()
    {
      if (_visitorsView != null)
        ActivateItem(_visitorsView);
      base.OnActivate();
    }

    private void Initialize(IProcessorLocator locator)
    {
      _locator = locator;

      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Location");

      UserVerified = true;
      UserVerificationIconVisible = CardDetectedIconVisible = false;     

      _visitorsView = new VisitorsViewModel(locator);

      Items.Add(_visitorsView);

    }

    public void OnCardDetected(string cardNumber){
      LocationStateIcon = ResourceLoader.UserContactlessCardsIconSource;
    }

    public void OnError(Exception ex) {
      LocationStateIcon = ResourceLoader.WarningIconSource;
    }

    public void OnReady(bool isReady) {
      LocationStateIcon = ResourceLoader.OkIconSource;
    }

    public void OnFrame(ref Bitmap frame) {
      BitmapSource convertedFrame = BitmapConversion.BitmapToBitmapSource(frame);
      ImageView.SetSingleImage(convertedFrame);
    }

    public void OnStop(bool stopped, string message) {
      LocationStateIcon = ResourceLoader.StopIconSource;
    }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) {
      LocationStateIcon = ResourceLoader.OkIconSource;
    }
    #endregion

    #region UI

    public BitmapSource OkIconSource
    {
      get
      {
        if (CurrentLocation == null)
          return ResourceLoader.ErrorIconSource;
        else
          return ResourceLoader.ErrorIconSource;
         // return CurrentLocation.AccessDevicesStatus ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource;
      }
    }
    public BitmapSource VerificationIconSource
    {
      get
      {
        return UserVerified ? ResourceLoader.VerificationIconSource
                            : ResourceLoader.VerificationFailedIconSource;
      }
    } 
    
    private VisitorsViewModel _visitorsView;
    public VisitorsViewModel VisitorsView
    {
      get { return _visitorsView; }
      set
      {
        if (_visitorsView != value)
        {
          _visitorsView = value;
          NotifyOfPropertyChange(() => VisitorsView);
        }
      }
    }

    
    private BioImageViewModel _imageView;
    public BioImageViewModel ImageView
    {
      get {
        if (_imageView == null)
          _imageView = new BioImageViewModel(_locator);
        return _imageView;
      }
      set
      {
        if (_imageView != value)
        {
          _imageView = value;
          NotifyOfPropertyChange(() => ImageView);
        }
      }
    }
    


    private bool _accessDeviceOK;
    public bool AccessDeviceOK
    {
      get { return _accessDeviceOK; }
      set
      {
        if (_accessDeviceOK != value)
        {
          _accessDeviceOK = value;

          NotifyOfPropertyChange(() => OkIconSource);
        }
      }

    }

    private bool _userVerified;
    public bool UserVerified
    {
      get { return _userVerified; }
      set
      {
        if (_userVerified != value)
        {
          _userVerified = value;
          NotifyOfPropertyChange(() => VerificationIconSource);
        }
      }
    }

    private bool _userVerificationIconVisible;
    public bool UserVerificationIconVisible
    {
      get { return _userVerificationIconVisible; }
      set
      {
        if (_userVerificationIconVisible != value)
        {
          _userVerificationIconVisible = value;
          NotifyOfPropertyChange(() => UserVerificationIconVisible);
        }
      }
    }

    private bool _cardDetectedIconVisible;
    public bool CardDetectedIconVisible
    {
      get { return _cardDetectedIconVisible; }
      set
      {
        if (_cardDetectedIconVisible != value)
        {
          _cardDetectedIconVisible = value;
          NotifyOfPropertyChange(() => CardDetectedIconVisible);
        }
      }
    }

    private BitmapSource _locationStateIcon;
    public BitmapSource LocationStateIcon
    {
      get { return _locationStateIcon; }
      set
      {
        if (_locationStateIcon != value)
        {
          _locationStateIcon = value;
          NotifyOfPropertyChange(() => LocationStateIcon);
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
    private IProcessorLocator    _locator;
    #endregion
     
  }  
}
