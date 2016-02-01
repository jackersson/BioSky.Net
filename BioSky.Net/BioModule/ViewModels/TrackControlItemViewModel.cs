using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;
using System.Windows;

using BioContracts;
using System.Collections.ObjectModel;
using System.Windows.Data;
using BioModule.Utils;
using System.Drawing;

namespace BioModule.ViewModels
{

  public class TrackControlItemViewModel : Conductor<IScreen>.Collection.AllActive, IObserver<AccessDeviceActivity>
  {
    private readonly IBioSkyNetRepository _database;

    public TrackControlItemViewModel(IProcessorLocator locator)
    {      
      Initialize(locator);      
    }

    protected override void OnActivate()
    {
      if (_visitorsView != null)
        ActivateItem(_visitorsView);
      base.OnActivate();
    }

    public TrackControlItemViewModel( IProcessorLocator locator, TrackLocation location )     
    {     
      _accessDeviceEngine  = locator.GetProcessor<IAccessDeviceEngine>();
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      
      Initialize(locator);

      if ( location != null )
       Update(location);
    }

    private void Initialize(IProcessorLocator locator)
    {
      DisplayName = "Location";

      UserVerified = true;
      UserVerificationIconVisible = false;
      CardDetectedIconVisible = false;

      _visitorsView = new VisitorsViewModel(locator);

      Items.Add(_visitorsView);

      ImageView = new ImageViewModel();
    }

    public void Update(TrackLocation trackLocation)
    {
      if (trackLocation == null)
        return;

      CurrentLocation = trackLocation;

      _captureDeviceEngine.Subscribe(OnNewFrame, trackLocation.CaptureDeviceName) ;
      _visitorsView.Update();

      string accessDeviceName = trackLocation.AccessDeviceName;
      if (!_accessDeviceEngine.HasObserver(this, accessDeviceName))
        _accessDeviceEngine.Subscribe(this, accessDeviceName);

      AccessDeviceOK = _accessDeviceEngine.AccessDeviceActive(accessDeviceName);    
    }

    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      if (bitmap == null)
        return;
      
      ImageView.UpdateImage(ref bitmap);
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
        if ( _imageView != value)
        {
          _imageView = value;
          NotifyOfPropertyChange(() => ImageView);
        }
      }
    }

    public void OnChecked(object name)
    {
      Console.WriteLine(name);
    }
    private bool _accessDeviceOK;
    public bool AccessDeviceOK
    {
      get { return _accessDeviceOK; }
      set {
        if ( _accessDeviceOK != value )
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
        if ( _userVerified != value)
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

    private TrackLocation _location;
    public TrackLocation CurrentLocation
    {
      get { return _location; }

      set
      {
        if (_location != value)
        {
          _location = value;
          NotifyOfPropertyChange(() => CurrentLocation);
        }
      }
    }

    public void OnNext(AccessDeviceActivity value)
    {
      AccessDeviceOK = true;

      if (value.Data != null)
      {
        CardDetectedIconVisible = true;
      }
      else
        CardDetectedIconVisible = false;
    }

    public void OnError(Exception error)
    {
      AccessDeviceOK = false;
      UserVerificationIconVisible = false;
      CardDetectedIconVisible = false;
    }

    public void OnCompleted()
    {
      throw new NotImplementedException();
    }

    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IAccessDeviceEngine  _accessDeviceEngine ;

    //**************************************************** UI **********************************************
    public BitmapSource OkIconSource
    {
      get { return AccessDeviceOK ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }

    public BitmapSource VerificationIconSource
    {
      get { return UserVerified ? ResourceLoader.VerificationIconSource 
                                : ResourceLoader.VerificationFailedIconSource; }
    }      
  }  
}
