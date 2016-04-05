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

namespace BioModule.ViewModels
{
  public class FullTrackControlItemViewModel : Screen, ICaptureDeviceObserver, IAccessDeviceObserver
  {
    public FullTrackControlItemViewModel(IProcessorLocator locator)
    {

      _bioEngine = locator.GetProcessor<IBioEngine>();

      BioImageView = new PhotoImageViewModel(locator);

      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Location");
    }

    public void Update(TrackLocation location)
    {
      CurrentLocation = location;

      _bioEngine.AccessDeviceEngine() .Unsubscribe(this);
      _bioEngine.CaptureDeviceEngine().Unsubscribe(this);

      if (BioImageView.CurrentPhotoView is ICaptureDeviceObserver)
        _bioEngine.CaptureDeviceEngine().Unsubscribe(BioImageView.CurrentPhotoView as ICaptureDeviceObserver);
    }

    public void OnFrame(ref Bitmap frame)
    {
      //CurrentLocation.Sub
    }

    public void OnStop(bool stopped, string message)
    {
      throw new NotImplementedException();
    }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all)
    {
      throw new NotImplementedException();
    }

    public void OnCardDetected(string cardNumber)
    {
      throw new NotImplementedException();
    }

    public void OnError(Exception ex)
    {
      throw new NotImplementedException();
    }

    public void OnReady(bool isReady)
    {
      throw new NotImplementedException();
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

    private PhotoImageViewModel _bioImageView;
    public PhotoImageViewModel BioImageView
    {
      get {  return _bioImageView; }
      set
      {
        if (_bioImageView != value)
        {
          _bioImageView = value;
          NotifyOfPropertyChange(() => BioImageView);
        }
      }
    }

    private readonly IBioEngine _bioEngine;
  }
}
