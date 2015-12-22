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
using BioModule.Model;
using System.Collections.ObjectModel;
using System.Windows.Data;
using BioModule.Utils;

namespace BioModule.ViewModels
{

  public class TrackControlItemViewModel : PropertyChangedBase, IObserver<AccessDeviceActivity>
  {          
    public TrackControlItemViewModel( IBioEngine bioEngine, TrackLocation location )
    {
      _location  = location ;
      _bioEngine = bioEngine;

      UserVerified = true;
      UserVerificationIconVisible = false;
      CardDetectedIconVisible = false;

      _notifications = new VisitorsViewModel(bioEngine, location.Caption );   
    }

    public void Update()
    {
      _notifications.Update();
    }

    public VisitorsViewModel Notifications
    {
      get { return _notifications; }
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

    public TrackLocation TrackLocation
    {
      get { return _location; }
    }

    public void OnNext(AccessDeviceActivity value)
    {
      throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
      throw new NotImplementedException();
    }

    public void OnCompleted()
    {
      throw new NotImplementedException();
    } 

    private TrackLocation     _location;
    private VisitorsViewModel _notifications;
    private readonly IBioEngine _bioEngine;

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

    public BitmapSource CardIconSource
    {
      get { return ResourceLoader.CardIconSource; }
    }       
  }

  
}
