using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using BioContracts;
using System.Drawing;
using WPFLocalizeExtension.Extensions;

namespace BioModule.ViewModels
{
  public class TrackControlItemViewModel : Conductor<IScreen>.Collection.AllActive
  {    
    public TrackControlItemViewModel(IProcessorLocator locator)
    {      
      Initialize(locator);      
    }

    public TrackControlItemViewModel( IProcessorLocator locator, TrackLocation location )     
    {
      _locator = locator;   
      
      Initialize(locator);
      
      if ( location != null )
       Update(location);
    }

    #region Update
    public void Update(TrackLocation trackLocation)
    {
      if (CurrentLocation != null)
      {
     //   CurrentLocation.FrameChanged    -= OnNewFrame;
        CurrentLocation.PropertyChanged -= OnLocationStatusChanged;
      }

      if (trackLocation == null)
        return;

      CurrentLocation = trackLocation;

      CurrentLocation.PropertyChanged += OnLocationStatusChanged;    
     // trackLocation.FrameChanged      += OnNewFrame;      
    }

    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      if (bitmap == null || ImageView == null)
        return;

      ImageView.UpdateOneImage(ref bitmap);
    }
   
    private void OnLocationStatusChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => OkIconSource);
    }
    #endregion

    #region Interface
    protected override void OnActivate()
    {
      if (_visitorsView != null)
        ActivateItem(_visitorsView);
      base.OnActivate();
    }
    public void OnDataContextChanged()
    {
      if (ImageView != null)
        ImageView = new ImageViewModel(_locator);
    } 
    private void Initialize(IProcessorLocator locator)
    {
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Location");

      UserVerified = true;
      UserVerificationIconVisible = CardDetectedIconVisible = false;     

      _visitorsView = new VisitorsViewModel(locator);

      Items.Add(_visitorsView);

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

    private ImageViewModel _imageView;
    public ImageViewModel ImageView
    {
      get { return _imageView; }
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
          NotifyOfPropertyChange(() => AccessDeviceOK);
        }
      }
    }
    #endregion

    #region Global Variables
    private readonly IProcessorLocator    _locator;
    #endregion
     
  }  
}
