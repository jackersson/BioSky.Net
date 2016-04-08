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

namespace BioModule.ViewModels
{
  public class FullTrackControlItemViewModel : Screen, ICaptureDeviceObserver, IAccessDeviceObserver
  {
    public FullTrackControlItemViewModel(IProcessorLocator locator)
    {

      _bioEngine = locator.GetProcessor<IBioEngine>();

      BioImageView = new BioImageViewModel(locator);
      currentDispatcher = locator.GetProcessor<Dispatcher>();
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Location");
    }

    public void Update(TrackLocation location)
    {      
      _bioEngine.AccessDeviceEngine() .Unsubscribe(this);
      _bioEngine.CaptureDeviceEngine().Unsubscribe(this);

      if (BioImageView.CurrentBioImage is ICaptureDeviceObserver)
        _bioEngine.CaptureDeviceEngine().Unsubscribe(BioImageView.CurrentBioImage as ICaptureDeviceObserver);

      if (location == null)
        return;

      CurrentLocation = location;

      _bioEngine.AccessDeviceEngine().Subscribe( this, location.GetAccessDeviceName);
      _bioEngine.CaptureDeviceEngine().Subscribe(this, location.GetCaptureDeviceName);
    }

    public void OnFrame(ref Bitmap frame)
    {
      BitmapSource convertedFrame = BitmapConversion.BitmapToBitmapSource(frame);
      convertedFrame.Freeze();
      BioImageView.SetSingleImage(convertedFrame);
    }

    

    public void OnStop(bool stopped, string message)
    {
      
     // throw new NotImplementedException();
    }

    public void OnStart(bool started, VideoCapabilities active, VideoCapabilities[] all)
    {
     // throw new NotImplementedException();
    }

    public void OnCardDetected(string cardNumber)
    {
     // throw new NotImplementedException();
    }

    public void OnError(Exception ex)
    {
      //Console.WriteLine("Here");
      //BioImageView.SetSingleImage(ResourceLoader.WarningIconSource);
      currentDispatcher.Invoke(() => BioImageView.SetSingleImage(ResourceLoader.WarningIconSource));
      // throw new NotImplementedException();
    }

    public void OnReady(bool isReady)
    {
      currentDispatcher.Invoke(() => BioImageView.SetSingleImage(ResourceLoader.OkIconSource));
      // throw new NotImplementedException();
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

    private BioImageViewModel _bioImageView;
    public BioImageViewModel BioImageView
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
    Dispatcher currentDispatcher;
    private readonly IBioEngine _bioEngine;
  }
}
