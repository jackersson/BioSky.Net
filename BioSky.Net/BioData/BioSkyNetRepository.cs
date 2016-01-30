using System;
using System.Collections.Generic;

using BioContracts;

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
    private Dictionary<string, Card>   cardSet;


    //public event EventHandler DataChanged;


    public event EventHandler AccessDevicesChanged;
    public event EventHandler CaptureDevicesChanged;
    public event EventHandler CardsChanged;
    public event EventHandler PersonChanged;
    public event EventHandler VisitorChanged;
    public event EventHandler LocationChanged;
    public event EventHandler PhotoChanged;

    public void OnAccessDevicesChanged()
    {
      if (AccessDevicesChanged != null)
        AccessDevicesChanged(this, EventArgs.Empty);
    }
    public void OnCaptureDevicesChanged()
    {
      if (CaptureDevicesChanged != null)
        CaptureDevicesChanged(this, EventArgs.Empty);
    }
    public void OnCardsChanged()
    {
      if (CardsChanged != null)
        CardsChanged(this, EventArgs.Empty);
    }
    public void OnPhotoChanged()
    {
      if (PhotoChanged != null)
        PhotoChanged(this, EventArgs.Empty);
    }
    public void OnLocationChanged()
    {
      if (LocationChanged != null)
        LocationChanged(this, EventArgs.Empty);
    }
    public void OnVisitorChanged()
    {
      if (VisitorChanged != null)
        VisitorChanged(this, EventArgs.Empty);
    }

    public void OnPersonChanged()
    {
      if (PersonChanged != null)
        PersonChanged(this, EventArgs.Empty);
    }

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
      cardSet     = new Dictionary<string, Card>();
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

    public Card GetCardByNumber(string number)
    {
      Card card;
      bool flag = cardSet.TryGetValue(number, out card);
      return card;
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
          OnPersonChanged();

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
          OnVisitorChanged();

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
          OnAccessDevicesChanged();
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
          OnCaptureDevicesChanged();
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
          OnCardsChanged();

          foreach (Card card in Cards.Cards)
          {
            if (!cardSet.ContainsKey(card.UniqueNumber))
              cardSet.Add(card.UniqueNumber, card);
          }
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

          OnLocationChanged();

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

          OnPhotoChanged();

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
