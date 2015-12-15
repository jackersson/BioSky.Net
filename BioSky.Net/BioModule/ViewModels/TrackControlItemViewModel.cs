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

namespace BioModule.ViewModels
{
  public class TrackControlItemViewModel : PropertyChangedBase, IObserver<AccessDeviceActivity>
  {       
   
    public TrackControlItemViewModel( TrackLocation location )
    {
      _location = location;
    }

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



    private bool _accessDeviceOK;

    private TrackLocation _location;

    //**************************************************** UI **********************************************
    public BitmapSource OkIconSource
    {
      get { return AccessDeviceOK ? ResourceLoader.OkIconSource : ResourceLoader.CancelIconSource; }
    }

    public BitmapSource VerificationIconSource
    {
      get { return ResourceLoader.VerificationIconSource; }
    }

    public BitmapSource CardIconSource
    {
      get { return ResourceLoader.CardIconSource; }
    }       
  }
}
