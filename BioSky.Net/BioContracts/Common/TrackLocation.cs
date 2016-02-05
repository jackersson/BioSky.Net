﻿using BioContracts.Common;
using BioContracts.Services;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  


  public class TrackLocation : PropertyChangedBase
  {    
    public TrackLocation(IProcessorLocator locator, Location location)
    {
      _locator = locator;
      _accessDeviceEngine  = locator.GetProcessor<IAccessDeviceEngine>();
      _database            = locator.GetProcessor<IBioSkyNetRepository>();
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      _bioService          = locator.GetProcessor<IServiceManager>();

      _accessDevices  = new Dictionary<string, TrackLocationAccessDeviceObserver>();
      _captureDevices = new Dictionary<string, TrackLocationCaptureDeviceObserver>();

      _database.AccessDeviceHolder.DataChanged  += AccessDevicesRepository_DataChanged;
      _database.CaptureDeviceHolder.DataChanged += CaptureDeviceHolder_DataChanged;


      _veryfier = new Verifyer(_captureDeviceEngine, _bioService);
      _requestUpdator = new UpdateRequestByForeignKey(_bioService.DatabaseService, _database);

      Update(location);
    }

    private void CaptureDeviceHolder_DataChanged()
    {
      List<CaptureDevice> capture_devices = _database.CaptureDeviceHolder.Data
                                            .Where(cap => cap.Locationid == _location.Id)
                                            .ToList();

      foreach (CaptureDevice cd in capture_devices)
      {
        TrackLocationCaptureDeviceObserver observer = new TrackLocationCaptureDeviceObserver(_locator, cd);
        _captureDevices.Add(cd.Devicename, observer);
        CaptureDeviceName = cd.Devicename;
      }
    }

    private void AccessDevicesRepository_DataChanged()
    {
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
      Card card;
      bool cardFound = _database.CardHolder.DataSet.TryGetValue(cardNumber, out card);

      /*
      VisitorList list = new VisitorList();
      Visitor visitor = new Visitor() { Locationid = _location.Id
                                      , Dbstate    = DbState.Insert
                                      , Time       = DateTime.Now.Ticks
                                      , Status     = ResultStatus.Failed
                                      , Personid   = 0
                                      , Photoid    = 0 };


  */

      Person person = null;
      if (cardFound)
      {        
        bool personFound = _database.PersonHolder.DataSet.TryGetValue(card.Personid, out person);
        //if (personFound)
        //{
         // _bioService.FaceService.VerifyFeedbackChanged += FaceService_VerifyFeedbackChanged;
             

          //sender.Success();       

          //visitor.Personid = person.Id;
          // visitor.Status   = ResultStatus.Success;
       // }
       
      }

      _bioService.FaceService.VerifyFeedbackChanged += FaceService_VerifyFeedbackChanged;
      VerificationData data = new VerificationData();
      data.Personid = ( person == null ) ? person.Id : 0;

      _veryfier.Start(CaptureDeviceName, data);

      //sender.Failed();
      //list.Visitors.Add(visitor);
      //await _bioService.DatabaseService.VisitorUpdateRequest(list);

    }

    private void FaceService_VerifyFeedbackChanged(object sender, VerificationFeedback feedback)
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


        Photo image         = _veryfier.GetImage();
        Photo feedbackPhoto = enrollFeedback.Photo;

        if (image == null)
          image = new Photo();

        if (feedbackPhoto != null)
          image.Fir = feedbackPhoto.Fir;


        image.Dbstate  = DbState.Insert;     
        image.Type     = PhotoSizeType.Full;
        image.Origin   = PhotoOriginType.Detected;
        image.Personid = personid;
        

        Visitor visitor  = new Visitor() { Locationid = _location.Id
                                           , Dbstate    = DbState.Insert
                                           , Time       = DateTime.Now.Ticks
                                           , Status     = success
                                           , Personid   = personid };

        _requestUpdator.BeginRequest(image, visitor);
      }


      
      if (enrollFeedback.Progress == 100)
      {
        _bioService.FaceService.VerifyFeedbackChanged -= FaceService_VerifyFeedbackChanged;
        Photo image = _veryfier.GetImage();
        Photo feedbackPhoto = enrollFeedback.Photo;
        VerificationData data = _veryfier.GetData();
      
        if ( image != null && feedbackPhoto != null )
        {
          PhotoList photoList = new PhotoList();          
          Photo photo = new Photo() { Dbstate      = DbState.Insert
                                    , Description  = image.Description
                                    , FileLocation = feedbackPhoto.FileLocation
                                    , FirLocation  = feedbackPhoto.FirLocation
                                    , Personid     = data.Personid
                                    , Type         = PhotoSizeType.Full
                                    , Origin       = PhotoOriginType.Detected
                                    };
      
          photoList.Photos.Add(photo);

          VisitorList list = new VisitorList();
          Visitor visitor  = new Visitor() { Locationid = _location.Id
                                           , Dbstate    = DbState.Insert
                                           , Time       = DateTime.Now.Ticks
                                           , Status     = ResultStatus.Success
                                           , Personid   = data.Personid };

          list.Visitors.Add(visitor);
          _requestUpdator.BeginRequest(photo, visitor);


          //await _serviceManager.DatabaseService.PhotoUpdateRequest(photoList);
        }        
       
      }
      if (_imageViewer != null)
         _imageViewer.ShowProgress(feedback.Progress, feedback.Success);
       
      
       Console.WriteLine(feedback);
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
    }

    public long LocationID
    {
      get { return _location.Id; }
    }


    //TODO make it once in Engine - not HERE
    public async void Start()
    {         
      if (_database.AccessDeviceHolder.Data.Count <= 0)
        await _bioService.DatabaseService.AccessDeviceRequest(new CommandAccessDevice());
      else
        AccessDevicesRepository_DataChanged();

      if (_database.CaptureDeviceHolder.Data.Count <= 0)
        await _bioService.DatabaseService.CaptureDeviceRequest(new CommandCaptureDevice());
      else
        CaptureDeviceHolder_DataChanged();  
    }
   
    public void Stop()
    {
      foreach (KeyValuePair<string, TrackLocationAccessDeviceObserver> ac in _accessDevices)
      {       
        ac.Value.AccessDeviceState -= OnAccessDeviceStateChanged;
        _accessDevices.Remove(ac.Key);
      }
    }
    
    public object ScreenViewModel { get; set; }

    public string Caption
    {
      get { return _location.LocationName; }
    }

    private Dictionary<string, TrackLocationAccessDeviceObserver>  _accessDevices ;
    private Dictionary<string, TrackLocationCaptureDeviceObserver> _captureDevices;

    private UpdateRequestByForeignKey _requestUpdator;
    private Verifyer _veryfier;
    private Location _location;
    private readonly IAccessDeviceEngine  _accessDeviceEngine ;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IServiceManager      _bioService         ;
    private readonly IBioSkyNetRepository _database           ;
    private readonly IProcessorLocator    _locator            ;
    
  }
}
