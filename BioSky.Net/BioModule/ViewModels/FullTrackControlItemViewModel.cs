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

namespace BioModule.ViewModels
{
  public class FullTrackControlItemViewModel : Screen, IFullLocationObserver
  {
    public FullTrackControlItemViewModel(IProcessorLocator locator)
    {
      //_uiDispatcher = locator.GetProcessor<Dispatcher>();
      _bioEngine = locator.GetProcessor<IBioEngine>();

      BioImageView = new BioImageViewModel(locator, 1);

      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Location");      
    }

    public void Update(TrackLocation location)
    {
       if (CurrentLocation != null)
       CurrentLocation.Unsubscribe(this);

      //  Console.WriteLine(_uiDispatcher.GetHashCode());
      // Console.WriteLine(Dispatcher.CurrentDispatcher.GetHashCode());
      BioImageView.SetSingleImage(null);
      if (location == null)
        return;

      CurrentLocation = location;
      location.Subscribe(this);      
    }
   

    public void OnOk(bool ok)
    {
      //throw new NotImplementedException();
    }

    public void OnStartVerificationByCard(string cardNumber)
    {
      //throw new NotImplementedException();
    }

    public void OnVerificationFailed(string message)
    {
      //throw new NotImplementedException();
    }

    public void OnCaptureDeviceFrameChanged(ref Bitmap frame)
    {
      //BitmapSource convertedFrame = BitmapConversion.BitmapToBitmapSource(frame);
      BioImageView.UpdateFrame(frame);
    }

    public void OnError(Exception ex)
    {
      BioImageView.UpdateFrame(null);
    }

    public void OnVerificationFailure(Exception ex)
    {
      //throw new NotImplementedException();
    }

    public void OnVerificationSuccess(bool state)
    {
     // throw new NotImplementedException();
    }

    public void OnVerificationProgress(int progress)
    {
     // throw new NotImplementedException();
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

    //private readonly Dispatcher _uiDispatcher;
    private readonly IBioEngine _bioEngine;
    private long MIN_BIO_IMAGE_STYLE = 65;
  }
}
