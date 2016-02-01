using System;
using System.Collections.Generic;

using BioContracts;

using BioFaceService;
using Caliburn.Micro;

using System.Collections.ObjectModel;
using BioData.Holders;
using Google.Protobuf;
using System.Collections;
using System.Diagnostics;

namespace BioData
{

  

  public class BioSkyNetRepository : IBioSkyNetRepository
  {
    /*
    private Dictionary<long, Photo>         photoSet        ;
    private Dictionary<long, Person>        personSet       ;
    private Dictionary<long, Location>      locationSet     ;
    private Dictionary<long, Visitor>       visitorSet      ;
    private Dictionary<long, AccessDevice>  accessDeviceSet ;
    private Dictionary<long, CaptureDevice> captureDeviceSet;
    private Dictionary<string, Card>        cardSet         ;

    

    public event EventHandler AccessDevicesChanged;
    public event EventHandler CaptureDevicesChanged;
    public event EventHandler CardsChanged;
    public event EventHandler PersonChanged;
    public event EventHandler VisitorChanged;
    public event EventHandler LocationChanged;
    public event EventHandler PhotoChanged;
    */
    /*
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
    */
    // private Dictionary<Type, IHolderUpdatable> _holders;

    VisitorHolder       _visitorHolder      ;
    PersonHolder        _personHolder       ;
    LocationHolder      _locationHolder     ;
    CardHolder          _cardHolder         ;
    CaptureDeviceHolder _captureDeviceHolder;
    AccessDeviceHolder  _accessDeviceHolder ;
    PhotoHolder         _photoHolder        ;

    public BioSkyNetRepository()
    {
      /*
      _persons         = new ObservableCollection<Person>();
      //_visitors        = new ObservableCollection<IMessage>();
      _locations       = new ObservableCollection<Location>();
      _access_devices  = new ObservableCollection<AccessDevice>();
      _capture_devices = new ObservableCollection<CaptureDevice>();
      _photos          = new ObservableCollection<Photo>();
      _cards           = new ObservableCollection<Card>();

      personSet        = new Dictionary<long, Person>();
      visitorSet       = new Dictionary<long, Visitor>();
      locationSet      = new Dictionary<long, Location>();
      accessDeviceSet  = new Dictionary<long, AccessDevice>();
      captureDeviceSet = new Dictionary<long, CaptureDevice>();
      cardSet          = new Dictionary<string, Card>();
      photoSet         = new Dictionary<long, Photo>();

      _visitorHolder = new VisitorHolder();

      _holders = new Dictionary<Type, IHolderUpdatable>();
      _holders.Add(typeof(Visitor), _visitorHolder);
      */
      _visitorHolder       = new VisitorHolder      ();
      _personHolder        = new PersonHolder       ();     
      _locationHolder      = new LocationHolder     ();
      _cardHolder          = new CardHolder         ();
      _captureDeviceHolder = new CaptureDeviceHolder();
      _accessDeviceHolder  = new AccessDeviceHolder ();
      _photoHolder         = new PhotoHolder        ();

    }

    /*

    VisitorHolder _visitorHolder;

    public void Update<T>(IEnumerable list, Result res )
    {
      Type generictype = typeof(T);
      if (_holders.ContainsKey(generictype))
        _holders[generictype].UpdateRequestResult(list, res);
    }
    */

    IHolder<Visitor, long> IBioSkyNetRepository.VisitorHolder
    {
      get { return _visitorHolder; }
    }

    IHolder<AccessDevice, long> IBioSkyNetRepository.AccessDeviceHolder
    {
      get { return _accessDeviceHolder; }
    }

    IHolder<Location, long> IBioSkyNetRepository.LocationHolder
    {
      get { return _locationHolder; }
    }

    IHolder<Photo, long> IBioSkyNetRepository.PhotoHolder
    {
      get { return _photoHolder; }
    }

    IHolder<Person, long> IBioSkyNetRepository.PersonHolder
    {
      get { return _personHolder; }
    }

    IHolder<CaptureDevice, long> IBioSkyNetRepository.CaptureDeviceHolder
    {
      get { return _captureDeviceHolder; }
    }

    IHolder<Card, string> IBioSkyNetRepository.CardHolder
    {
      get { return _cardHolder; }
    }
     

    /*
    public T GetDataByKey<T, Key>( Key key )
    {            
      return (T)_visitorHolder.GetDataByKey2( 1 );
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
      return _visitorHolder.GetDataByKey(id);
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


    public void UpdatePersonSet(PersonList list)
    {
      Persons = new ObservableCollection<Person>(list.Persons);

      foreach (Person pers in list.Persons)
      {
        if (!personSet.ContainsKey(pers.Id))
          personSet.Add(pers.Id, pers);        
      }
    }
    public void UpdateVisitorSet(VisitorList list)
    {
      Stopwatch timer = new Stopwatch();
      timer.Start();
      _visitorHolder.Update(list.Visitors);
      timer.Stop();

      Dictionary<long, Visitor> dc = new Dictionary<long, Visitor>();
      foreach (Visitor pers in list.Visitors)
      {       
        if (!dc.ContainsKey(pers.Id))
          dc.Add(pers.Id, pers);
      }

      long first = timer.ElapsedMilliseconds;



      double avarage = 0;

      avarage = 0;

      for (int i = 0; i < 100; i++)
      {
        timer.Reset();
        timer.Start();

        _visitorHolder.Update(list.Visitors);
        timer.Stop();
        /*
        Visitor vs23fdf;
        dc.TryGetValue(1, out vs23fdf);
        timer.Stop();
        
        avarage += timer.ElapsedTicks;
      }
      Console.WriteLine((double)(avarage));

      avarage = 0;

      for (int i = 0; i < 100; i++)
      {
        timer.Reset();
        timer.Start();
        _visitorHolder.Update(list.Visitors);
        //Visitor vs23 = _visitorHolder.GetDataByKey(1);
        timer.Stop();
        avarage += timer.ElapsedTicks;
      }
      Console.WriteLine((double)(avarage));

      avarage = 0;

      /*
      Dictionary<long, Visitor> dc2 = _visitorHolder.DataSet;
      for (int i = 0; i < 100; i++)
      {
        timer.Reset();
        timer.Start();
        Visitor vs23fddf;
        dc2.TryGetValue(1, out vs23fddf);
        timer.Stop();
        avarage += timer.ElapsedTicks;
      }
      Console.WriteLine( (double)(avarage));
      */


    /*
    timer.Reset();
    timer.Start();
    for (int i = 0; i < 100; i++)
    {
      Visitor vs23 = _visitorHolder.GetDataByKey(1);
    }
    timer.Stop();

    Console.WriteLine(first - timer.ElapsedMilliseconds);
    Console.WriteLine("");
  }

  public void UpdateAccessDeviceSet(AccessDeviceList list)
  {
    AccessDevices = new ObservableCollection<AccessDevice>(list.AccessDevices);

    foreach (AccessDevice dev in list.AccessDevices)
    {
      if (!accessDeviceSet.ContainsKey(dev.Id))
        accessDeviceSet.Add(dev.Id, dev);
    }
  }
  public void UpdateCaptureDeviceSet(CaptureDeviceList list)
  {
    CaptureDevices = new ObservableCollection<CaptureDevice>(list.CaptureDevices);

    foreach (CaptureDevice dev in list.CaptureDevices)
    {
      if (!captureDeviceSet.ContainsKey(dev.Id))
        captureDeviceSet.Add(dev.Id, dev);
    }
  }
  public void UpdateCardSet(CardList list)
  {
    Cards = new ObservableCollection<Card>(list.Cards);

    foreach (Card card in list.Cards)
    {
      if (!cardSet.ContainsKey(card.UniqueNumber))
        cardSet.Add(card.UniqueNumber, card);
    }
  }
  public void UpdateLocationSet(LocationList list)
  {
    Locations = new ObservableCollection<Location>(list.Locations);

    OnLocationChanged();

    foreach (Location location in list.Locations)
    {
      if (!locationSet.ContainsKey(location.Id))
        locationSet.Add(location.Id, location);
    }
  }
  public void UpdatePhotoSet(PhotoList list)
  {
    Photos = new ObservableCollection<Photo>(list.Photos);

    foreach (Photo photo in list.Photos)
    {
      if (!photoSet.ContainsKey(photo.Id))
        photoSet.Add(photo.Id, photo);
    }
  }   

  private void AddPerson(Person person)
  {
    long id = person.Id;
    Persons.Add(person);
    if (!personSet.ContainsKey(id))
      personSet.Add(id, person);
  }

  private void UpdatePerson(Person person)
  {
    long id = person.Id;
    if (personSet.ContainsKey(id))
      personSet[id] = person;
  }

  private void RemovePerson(Person person)
  {
    personSet.Remove(person.Id);
    Persons.Remove(person);
  }


  public void UpdatePerson(Person person, DbState state)
  {      
    switch (state)
    {
      case DbState.Insert:
        AddPerson(person);
        break;

      case DbState.Update:
        UpdatePerson(person);         
        break;

      case DbState.Remove:
        RemovePerson(person);
        break;
    }

    NotifyOfPropertyChange(() => Persons);
  }


  public void UpdateCardFromServer(Card card)
  {
    switch (card.Dbstate)
    {
      case DbState.Insert:
        AddCard(card);
        break;

      case DbState.Update:
        UpdateCard(card);
        break;

      case DbState.Remove:
        RemoveCard(card);
        break;
    }

    NotifyOfPropertyChange(() => Cards);
  }

  private void AddCard(Card card)
  {
    string id = card.UniqueNumber;
    Cards.Add(card);
    if (!cardSet.ContainsKey(id))
      cardSet.Add(id, card);
  }

  private void UpdateCard(Card card)
  {
    string id = card.UniqueNumber;
    if (cardSet.ContainsKey(id))
      cardSet[id] = card;
  }

  private void RemoveCard(Card card)
  {
    cardSet.Remove(card.UniqueNumber);
    Cards.Remove(card);
  }

  private ObservableCollection<Person> _persons;
  public ObservableCollection<Person> Persons
  {
    get { return _persons; }  
    private set
    {
      if ( _persons != value )
      {
        _persons = value;
        NotifyOfPropertyChange(() => Persons);
      }
    }
  }

  public ObservableCollection<IMessage> Visitors
  {
    get { return _visitorHolder.Data; }    
  }

  private ObservableCollection<AccessDevice> _access_devices;
  public ObservableCollection<AccessDevice> AccessDevices
  {
    get { return _access_devices; }
    private set
    {
      if (_access_devices != value)
      {
        _access_devices = value;
        NotifyOfPropertyChange(() => AccessDevices);
      }
    }
  }

  private ObservableCollection<CaptureDevice> _capture_devices;
  public ObservableCollection<CaptureDevice> CaptureDevices
  {
    get { return _capture_devices; }
    private set
    {
      if (_capture_devices != value)
      {
        _capture_devices = value;
        NotifyOfPropertyChange(() => CaptureDevices);
      }
    }
  }

  private ObservableCollection<Card> _cards;
  public ObservableCollection<Card> Cards
  {
    get { return _cards; }
    private set
    {
      if (_cards != value)
      {
        _cards = value;
        NotifyOfPropertyChange(() => Cards);
      }
    }
  }

  private ObservableCollection<Location> _locations;
  public ObservableCollection<Location> Locations
  {
    get { return _locations; }
    private set
    {
      if (_locations != value)
      {
        _locations = value;
        NotifyOfPropertyChange(() => Locations);
      }
    }
  }

  private ObservableCollection<Photo> _photos;
  public ObservableCollection<Photo> Photos
  {
    get { return _photos; }
    private set
    {
      if (_photos != value)
      {
        _photos = value;
        NotifyOfPropertyChange(() => Photos);
      }
    }
  }
  */
  }
}
