using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq.Expressions;

using BioFaceService;
using Caliburn.Micro;

namespace BioData
{
  public class BioSkyNetRepository : PropertyChangedBase, IBioSkyNetRepository
  {

    private Dictionary<long, Photo>    photoSet;
    private Dictionary<long, Person>   personSet;
    private Dictionary<long, Location> locationSet;
    private Dictionary<long, Visitor>  visitorSet;


    public event EventHandler DataChanged;

    public BioSkyNetRepository()
    {

      _persons         = new PersonList();
      _visitors        = new VisitorList();
      _locations       = new LocationList();
      _access_devices  = new AccessDeviceList();
      _capture_devices = new CaptureDeviceList();
      _photos          = new PhotoList();
      _cards           = new CardList();

      photoSet    = new Dictionary<long, Photo>();
      personSet   = new Dictionary<long, Person>();
      locationSet = new Dictionary<long, Location>();
      visitorSet  = new Dictionary<long, Visitor>();



/*
      Person pers = new Person() { Id = 1, Firstname = "Sasha", Lastname = "Iskra", Gender = Person.Types.Gender.Male, Rights = Person.Types.Rights.Operator, Country = "Vinnitsya", Thumbnail = 1 };
      Persons.Persons.Add(pers);

      Visitor vis = new Visitor() { Id = 1, Personid = 1, Status = Visitor.Types.VisitorStatus.Success, Time = 1297380023295, Locationid = 1, Photoid = 1 };
      Visitors.Visitors.Add(vis);

      Location loc1 = new Location() { Id = 1, LocationName = "Location 1", Desctiption = "Location" };
      Location loc2 = new Location() { Id = 2, LocationName = "Location 2", Desctiption = "Location" };
      Location loc3 = new Location() { Id = 3, LocationName = "Location 3", Desctiption = "Location" };

      Locations.Locations.Add(loc1);
      Locations.Locations.Add(loc2);
      Locations.Locations.Add(loc3);

      AccessDevice acDev1 = new AccessDevice() { Id = 1, Locationid = 1, Portname = "Com1", Type = AccessDevice.Types.AccessDeviceType.DeviceIn };
      AccessDevice acDev2 = new AccessDevice() { Id = 2, Locationid = 1, Portname = "Com2", Type = AccessDevice.Types.AccessDeviceType.DeviceOut };
      AccessDevice acDev3 = new AccessDevice() { Id = 3, Locationid = 1, Portname = "Com3", Type = AccessDevice.Types.AccessDeviceType.DeviceIn };
      AccessDevice acDev4 = new AccessDevice() { Id = 1, Locationid = 2, Portname = "Com4", Type = AccessDevice.Types.AccessDeviceType.DeviceIn };
      AccessDevice acDev5 = new AccessDevice() { Id = 2, Locationid = 2, Portname = "Com5", Type = AccessDevice.Types.AccessDeviceType.DeviceOut };
      AccessDevice acDev6 = new AccessDevice() { Id = 3, Locationid = 2, Portname = "Com6", Type = AccessDevice.Types.AccessDeviceType.DeviceIn };
      AccessDevice acDev7 = new AccessDevice() { Id = 1, Locationid = 3, Portname = "Com7", Type = AccessDevice.Types.AccessDeviceType.DeviceIn };
      AccessDevice acDev8 = new AccessDevice() { Id = 2, Locationid = 3, Portname = "Com8", Type = AccessDevice.Types.AccessDeviceType.DeviceOut };
      AccessDevice acDev9 = new AccessDevice() { Id = 3, Locationid = 3, Portname = "Com9", Type = AccessDevice.Types.AccessDeviceType.DeviceIn };
      AccessDevices.AccessDevices.Add(acDev1);
      AccessDevices.AccessDevices.Add(acDev2);
      AccessDevices.AccessDevices.Add(acDev3);
      AccessDevices.AccessDevices.Add(acDev4);
      AccessDevices.AccessDevices.Add(acDev5);
      AccessDevices.AccessDevices.Add(acDev6);
      AccessDevices.AccessDevices.Add(acDev7);
      AccessDevices.AccessDevices.Add(acDev8);
      AccessDevices.AccessDevices.Add(acDev9);

      CaptureDevice capDev1 = new CaptureDevice() { Id = 1, Locationid = 1, Devicename = "Camera1" };
      CaptureDevice capDev2 = new CaptureDevice() { Id = 2, Locationid = 1, Devicename = "Camera2" };
      CaptureDevice capDev3 = new CaptureDevice() { Id = 3, Locationid = 1, Devicename = "Camera3" };

      Card card = new Card() { Id = 1, Personid = 1, UniqueNumber = "34567834567" };
      Cards.Cards.Add(card);

      Photo photo = new Photo() { Id = 1, Personid = 1, FileLocation = "taras.jpg" };
      Photos.Photos.Add(photo);


      foreach (Person person in Persons.Persons)
      {
        if (!personSet.ContainsKey(pers.Id))
          personSet.Add(pers.Id, pers);
      }

      foreach (Location location in Locations.Locations)
      {
        if (!locationSet.ContainsKey(location.Id))
          locationSet.Add(location.Id, location);
      }*/


    }

    public Photo GetPhotoByID( long id )
    {
      Photo photo;
     
      //TODO if !flag - default photo
      bool flag = photoSet.TryGetValue(id, out photo);
      return photo;
    }
    public Person GetPersonByID(long id)
    {
      Person person;
      bool flag = personSet.TryGetValue(id, out person);
      return person;
    }
    public Visitor GetVisitorByID(long id)
    {
      Visitor visitor;
      bool flag = visitorSet.TryGetValue(id, out visitor);
      return visitor;
    }
    public Location GetLocationByID(long id)
    {
      Location location;
      bool flag = locationSet.TryGetValue(id, out location);
      return location;
    }
    
    public void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged(this, EventArgs.Empty);
    }

    public PersonList _persons;
    public PersonList Persons
    {
      get { return _persons;  }
      set
      {
        if ( _persons != value )
        {
          _persons = value;
          NotifyOfPropertyChange(() => Persons);
          OnDataChanged();

          foreach (Person pers in Persons.Persons)
          {
            if (!personSet.ContainsKey(pers.Id))
              personSet.Add(pers.Id, pers);
          }
        }
      }      
    }

    public VisitorList _visitors;
    public VisitorList Visitors
    {
      get { return _visitors; }
      set
      {
        if (_visitors != value)
        {
          _visitors = value;
          NotifyOfPropertyChange(() => Visitors);
          OnDataChanged();

          foreach (Visitor visitor in Visitors.Visitors)
          {
            if (!visitorSet.ContainsKey(visitor.Id))
              visitorSet.Add(visitor.Id, visitor);
          }
        }
      }
    }

    public AccessDeviceList _access_devices;
    public AccessDeviceList AccessDevices
    {
      get { return _access_devices; }
      set
      {
        if (_access_devices != value)
        {
          _access_devices = value;
          NotifyOfPropertyChange(() => AccessDevices);
          OnDataChanged();
        }
      }
    }

    public CaptureDeviceList _capture_devices;
    public CaptureDeviceList CaptureDevices
    {
      get { return _capture_devices; }
      set
      {
        if (_capture_devices != value)
        {
          _capture_devices = value;
          NotifyOfPropertyChange(() => CaptureDevices);
          OnDataChanged();
        }
      }
    }

    public CardList _cards;
    public CardList Cards
    {
      get { return _cards; }
      set
      {
        if (_cards != value)
        {
          _cards = value;
          NotifyOfPropertyChange(() => Cards);
          OnDataChanged();
        }
      }
    }

    public LocationList _locations;
    public LocationList Locations
    {
      get { return _locations; }
      set
      {
        if (_locations != value)
        {
          _locations = value;
          NotifyOfPropertyChange(() => Locations);

          foreach (Location location in Locations.Locations)
          {
            if (!locationSet.ContainsKey(location.Id))
              locationSet.Add(location.Id, location);
          }
        }
      }
    }

    public PhotoList _photos;
    public PhotoList Photos
    {
      get { return _photos; }
      set
      {
        if (_photos != value)
        {
          _photos = value;
          NotifyOfPropertyChange(() => Photos);

          foreach ( Photo ph in Photos.Photos )
          {
            if (!photoSet.ContainsKey(ph.Id))
              photoSet.Add(ph.Id, ph);     
          }
         

        }
      }
    }

  


  }
}
