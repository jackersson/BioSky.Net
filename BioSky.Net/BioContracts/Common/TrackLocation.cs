using BioContracts.Common;
using BioContracts.Services;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{  
  public class TrackLocation : PropertyChangedBase
  {
    public event FrameEventHandler FrameChanged;

    public event EnrollFeedbackEventHandler EnrollFeedbackChanged;
    public TrackLocation(IProcessorLocator locator, Location location)
    {
      _locator = locator;
      _accessDeviceEngine  = locator.GetProcessor<IAccessDeviceEngine>();
      _database            = locator.GetProcessor<IBioSkyNetRepository>();
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      _bioService          = locator.GetProcessor<IServiceManager>();

      _accessDevices  = new Dictionary<string, TrackLocationAccessDeviceObserver>();
      _captureDevices = new Dictionary<string, TrackLocationCaptureDeviceObserver>();

      _database.Locations.DataChanged += RefreshData;     

      _veryfier = new Verifyer(_captureDeviceEngine, _bioService);     

      Update(location);
    }

    private void RefreshData()
    {
      RefreshCaptureDevices();
      RefreshAccessDevices ();
    }

    private void RefreshCaptureDevices()
    {
      StopCaptureDevices();

      List<CaptureDevice> capture_devices = _database.CaptureDeviceHolder.Data
                                            .Where(cap => cap.Locationid == _location.Id)
                                            .ToList();

      foreach (CaptureDevice cd in capture_devices)
      {       
        TrackLocationCaptureDeviceObserver observer = new TrackLocationCaptureDeviceObserver(_locator, cd);
        observer.Subscribe(OnFrameChanged);
        _captureDevices.Add(cd.Devicename, observer);
        CaptureDeviceName = cd.Devicename;
      }
    }

    private void OnFrameChanged(object sender, ref Bitmap bitmap)
    {
      if (FrameChanged != null)
      {
        FrameChanged(sender,ref  bitmap);
      }
    }

    private void StopAccessDevices()
    {
      foreach (KeyValuePair<string, TrackLocationAccessDeviceObserver> ac in _accessDevices)
      {
        ac.Value.AccessDeviceState -= OnAccessDeviceStateChanged;
        _accessDevices.Remove(ac.Key);
      }
    }

    private void StopCaptureDevices( )
    {
      foreach (KeyValuePair<string, TrackLocationCaptureDeviceObserver> ac in _captureDevices)
      {
        ac.Value.Unsubscribe(OnFrameChanged);
        ac.Value.Stop();
        _accessDevices.Remove(ac.Key);
      }
    }

    private void RefreshAccessDevices()
    {
      StopAccessDevices();

      List<AccessDevice> access_devices = _database.AccessDeviceHolder.Data
                                          .Where(ad => ad.Locationid == _location.Id)
                                          .ToList();

      foreach (AccessDevice ac in access_devices)
      {        
        TrackLocationAccessDeviceObserver observer = new TrackLocationAccessDeviceObserver(_locator, ac);
        observer.AccessDeviceState += OnAccessDeviceStateChanged;
        observer.CardDetected += OnCardDetected;
        _accessDevices.Add(ac.Portname, observer);
      }
    }

    private void OnAccessDeviceStateChanged(bool status )
    {
      AccessDevicesStatus = status;      
    }

    private void OnCardDetected(TrackLocationAccessDeviceObserver sender, string cardNumber)
    {

      Console.WriteLine("Verifier busy = " + _veryfier.Busy);
      if (_veryfier.Busy)
        return;

      CardNumber = "";
      CardNumber = cardNumber;
      observer = null;
      observer = sender;
      Card card = _database.CardHolder.GetValue(cardNumber);

      
      sender.Reset();

      if (card != null)
      {        
        Person person = _database.PersonHolder.GetValue(card.Personid);
        if (person != null)
        {         
          try
          {
            VerificationData verificationData = new VerificationData();
            verificationData.Personid = person.Id;
            _veryfier.Start(CaptureDeviceName, verificationData);
            _bioService.FaceService.VerifyFeedbackChanged += FaceService_VerifyFeedbackChanged;
          }
          catch ( Exception e)
          {
            Console.WriteLine(e.Message);
          }
        }
       
      }
    }

    string CardNumber = "";
    TrackLocationAccessDeviceObserver observer = null;


    private async void FaceService_VerifyFeedbackChanged(object sender, VerificationFeedback feedback)
    {      
      EnrollmentFeedback enrollFeedback = feedback.EnrollmentFeedback;
      if (enrollFeedback == null)
        return;

      long personid = 0;
      ResultStatus success = ResultStatus.Failed;

      if (enrollFeedback.Progress == 100)
      {
        _bioService.FaceService.VerifyFeedbackChanged -= FaceService_VerifyFeedbackChanged;
        VerificationData data = _veryfier.GetData();
        personid = data.Personid;
        success  = enrollFeedback.Success ? ResultStatus.Success : ResultStatus.Failed;

        if (observer != null)
        {
          if (success == ResultStatus.Success)
            observer.Success();
          else
            observer.Failed();
        }
       

        Photo image         = _veryfier.GetImage();
        Photo feedbackPhoto = enrollFeedback.Photo;

        if (image == null)
          image = new Photo();

        if (feedbackPhoto != null)
          image.Fir = feedbackPhoto.Fir;


        image.EntityState  = EntityState.Added;     
        image.SizeType     = PhotoSizeType.Full;
        image.OriginType   = PhotoOriginType.Detected;
        image.Personid     = personid;

        VisitorList list = new VisitorList();
        Visitor visitor  = new Visitor() { Locationid    = _location.Id
                                          , EntityState  = EntityState.Added
                                          , Time         = DateTime.Now.Ticks
                                          , Status       = success
                                          , CardNumber   = CardNumber
                                          , Personid     = personid };

        visitor.Photo = image;

        list.Visitors.Add(visitor);

        try
        {
          await _bioService.DatabaseService.VisitorUpdate(list);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
        observer.Failed();
      }


      OnEnrollFeedbackChanged(enrollFeedback);
      Console.WriteLine(enrollFeedback.Progress);
    }

    private void OnEnrollFeedbackChanged(EnrollmentFeedback feedback)
    {
      if (EnrollFeedbackChanged != null)
        EnrollFeedbackChanged(null, feedback);
    }

    private string _captureDeviceName;
    public string CaptureDeviceName
    {
      get { return _captureDeviceName; }
      set
      {
        if (_captureDeviceName != value )
        {
          _captureDeviceName = value;          
        }
      }
    }

    
    private bool _accessDevicesStatus;
    public bool AccessDevicesStatus
    {
      get { return _accessDevicesStatus; }
      set
      {
        if (_accessDevicesStatus != value)
        {
          _accessDevicesStatus = value;
          NotifyOfPropertyChange(() => AccessDevicesStatus);    
        }
      }
    }
    

    public void Update(Location location)
    {
      _location = location;
      RefreshData();
    }

    public long LocationID
    {
      get { return _location.Id; }
    }

       
    public void Stop()
    {
      StopAccessDevices();
      StopCaptureDevices();
    }
    
    public IScreen ScreenViewModel { get; set; }

    public string Caption
    {
      get { return _location.LocationName; }
    }

    private Dictionary<string, TrackLocationAccessDeviceObserver>  _accessDevices ;
    private Dictionary<string, TrackLocationCaptureDeviceObserver> _captureDevices;

    private Verifyer _veryfier;
    private Location _location;
    private readonly IAccessDeviceEngine  _accessDeviceEngine ;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IServiceManager      _bioService         ;
    private readonly IBioSkyNetRepository _database           ;
    private readonly IProcessorLocator    _locator            ;
    
  }
}
