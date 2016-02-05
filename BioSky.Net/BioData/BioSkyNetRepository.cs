using System;
using System.Collections.Generic;

using BioContracts;

using BioService;
using Caliburn.Micro;

using System.Collections.ObjectModel;

using System.IO;
using BioData.Holders;

using BioContracts.Holders;

namespace BioData
{
  public class BioSkyNetRepository : PropertyChangedBase, IBioSkyNetRepository
  {
    VisitorHolder       _visitorHolder      ;
    PersonHolder        _personHolder       ;
    LocationHolder      _locationHolder     ;
    CardHolder          _cardHolder         ;
    CaptureDeviceHolder _captureDeviceHolder;
    AccessDeviceHolder  _accessDeviceHolder ;
    PhotoHolder         _photoHolder        ;

    ILocalStorage       _localStorage       ;

    public BioSkyNetRepository()
    {
      _visitorHolder       = new VisitorHolder      ();
      _personHolder        = new PersonHolder       ();     
      _locationHolder      = new LocationHolder     ();
      _cardHolder          = new CardHolder         ();
      _captureDeviceHolder = new CaptureDeviceHolder();
      _accessDeviceHolder  = new AccessDeviceHolder ();
      _photoHolder         = new PhotoHolder        ();
      _localStorage        = new BioLocalStorage    ();

    
    }

    public IHolder<Visitor, long> VisitorHolder
    {
      get { return _visitorHolder; }
    }

    public IHolder<AccessDevice, long> AccessDeviceHolder
    {
      get { return _accessDeviceHolder; }
    }

    public IHolder<Location, long> LocationHolder
    {
      get { return _locationHolder; }
    }

    public IHolder<Photo, long> PhotoHolder
    {
      get { return _photoHolder; }
    }

    public IHolder<Person, long> PersonHolder
    {
      get { return _personHolder; }
    }

    public IHolder<CaptureDevice, long> CaptureDeviceHolder
    {
      get { return _captureDeviceHolder; }
    }

    public IHolder<Card, string> CardHolder
    {
      get { return _cardHolder; }
    }

    public ILocalStorage LocalStorage
    {
      get { return _localStorage;  }
    }


    public IPhotoHolder PhotoHolderByPerson 
    {
      get { return _photoHolder;  }
    }
   

  }
}
