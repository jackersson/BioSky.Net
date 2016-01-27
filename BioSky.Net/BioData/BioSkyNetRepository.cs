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
    public BioSkyNetRepository()
    {
      Locations = new LocationList();

      Location loc = new Location() { Id = 1, LocationName = "Testable", Desctiption = "Test location" };
      Locations.Locations.Add(loc);
      
    }

    public HashSet<Location> locs;

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
        }
      }
    }


  }
}
