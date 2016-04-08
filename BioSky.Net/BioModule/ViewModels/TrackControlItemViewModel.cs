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

namespace BioModule.ViewModels
{
  public delegate void TestEventHandler(bool state);
  public class TrackControlItemViewModel : Conductor<IScreen>.Collection.AllActive, IAccessDeviceObserver, ICaptureDeviceObserver
  {

    public event TestEventHandler Testsss;

    public TrackControlItemViewModel(IProcessorLocator locator) {      
      Initialize(locator);
    }

    public TrackControlItemViewModel( IProcessorLocator locator, TrackLocation location )     
    {   
      Initialize(locator);

      if ( location != null )
       Update(location);
    }

    public void OnTestss(bool state)
    {
     // if (Testsss != null)
       // Testsss(state);
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
        _bioEngine.CaptureDeviceEngine().Unsubscribe(this);
        _bioEngine.AccessDeviceEngine() .Unsubscribe(this);
      }

      if (location == null)
        return;

      CurrentLocation = location;
      
      _bioEngine.CaptureDeviceEngine().Subscribe(this, CurrentLocation.GetCaptureDeviceName );
      _bioEngine.AccessDeviceEngine ().Subscribe(this, CurrentLocation.GetAccessDeviceName  );
     
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
      _locator   = locator;
      _bioEngine = locator.GetProcessor<IBioEngine>();
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Location");

      UserVerified = true;
      UserVerificationIconVisible = CardDetectedIconVisible = false;     

      _visitorsView = new VisitorsViewModel(locator);

      Items.Add(_visitorsView);

      LocationStateIcon = ResourceLoader.WarningIconSource;

      this.Testsss += TrackControlItemViewModel_Testsss;

    }

    private void TrackControlItemViewModel_Testsss(bool state)
    {
      UpdateLocationState(state ? ResourceLoader.OkIconSource : ResourceLoader.WarningIconSource);
    }

    public void OnCardDetected(string cardNumber){
      //UpdateLocationState(ResourceLoader.UserContactlessCardsIconSource);
    }

    public void OnError(Exception ex) {
      //UpdateLocationState( ResourceLoader.WarningIconSource );
      // UpdateLocStatus(false);
      // BitmapImage img = new BitmapImage(new Uri(@"F:\Biometric Software\C#\BioSky.Net\BioSky.Net\BioSky.Net\BioModule\Resources\stop.png"));
      // UpdateLocationState(img);

      OnTestss(false);
    }

    public void OnReady(bool isReady) {
      // UpdateLocationState(ResourceLoader.OkIconSource);
      OnTestss(true);
      //UpdateLocStatus(true);
    }

    public void OnFrame(ref Bitmap frame) {
      BitmapSource convertedFrame = BitmapConversion.BitmapToBitmapSource(frame);
      convertedFrame.Freeze();
      ImageView.SetSingleImage(convertedFrame);
    }

    public void OnStop(bool stopped, string message) {
     // UpdateLocationState(ResourceLoader.StopIconSource);
  
    }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all) {
    //  UpdateLocationState(ResourceLoader.OkIconSource);
     
    }

    public void UpdateLocationState( BitmapSource source )
    {
      try
      {

        //using (var ms = new MemoryStream(source.))
       // LocationStateIcon.StreamSource = ms
        source.Freeze();
       // LocationStateIcon.Freeze();
        System.Action action = delegate { LocationStateIcon = source; };
       // if (LocationStateIcon != null &&  LocationStateIcon.Dispatcher != null )
        LocationStateIcon.Dispatcher.Invoke(action);
      //  LocationStateIcon.
       // LocationStateIcon = source;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
    #endregion

    #region UI

    public void UpdateLocStatus(bool state)
    {
      DeviceIsOk = state;
      //NotifyOfPropertyChange(() => LocationStateIcon);
    }


    private bool DeviceIsOk;
    private BitmapSource _locationStateIcon;
    public BitmapSource LocationStateIcon
    {
      get
      {
        return _locationStateIcon;
        /*
        if (_locationStateIcon == null)
          return ResourceLoader.WarningIconSource;
        return _locationStateIcon;
        if (DeviceIsOk)
          return ResourceLoader.OkIconSource;
        else
          return ResourceLoader.ErrorIconSource;
          */
        // return CurrentLocation.AccessDevicesStatus ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource;
      }
      set
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
          Console.WriteLine(ex.Message);
        }
      }
    }
    
    public BitmapSource VerificationIconSource
    {
      get {
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
    

    /*
    private bool _accessDeviceOK;
    public bool AccessDeviceOK
    {
      get { return _accessDeviceOK; }
      set
      {
        if (_accessDeviceOK != value)
        {
          _accessDeviceOK = value;

         // NotifyOfPropertyChange(() => OkIconSource);
        }
      }

    }
    */

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
    /*
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
    
  */
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
    private IBioEngine           _bioEngine;
    private IProcessorLocator    _locator  ;
    #endregion
     
  }  
}
