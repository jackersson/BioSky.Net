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


namespace BioModule.ViewModels
{

  public class TrackControlItemViewModel : Screen, IObserver<AccessDeviceActivity>
  {   
    public TrackControlItemViewModel( IProcessorLocator locator, TrackLocation location )     
    {
      _location  = location;   

      UserVerified = true;
      UserVerificationIconVisible = false;
      CardDetectedIconVisible = false;      

      _visitorsView = new VisitorsViewModel(locator);   
    }

    public void Update()
    {
      _visitorsView.Update();
    }

    private VisitorsViewModel _visitorsView;
    public VisitorsViewModel VisitorsView
    {
      get { return _visitorsView; }
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
