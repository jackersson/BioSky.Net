using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  //delegate void DataChangedHandler(object sender);

  public interface IBioSkyNetRepository
  {   
   

   event EventHandler AccessDevicesChanged;
   event EventHandler CaptureDevicesChanged;
   event EventHandler CardsChanged;
   event EventHandler PersonChanged;
   event EventHandler VisitorChanged;
   event EventHandler LocationChanged;
   event EventHandler PhotoChanged;

    Person GetPersonByID(long id);
    Location GetLocationByID(long id);
    Photo GetPhotoByID(long id);
    Visitor GetVisitorByID(long id);

    PersonList Persons
    {
      get; set;
    }

    VisitorList Visitors
    {
      get; set;
    }


    AccessDeviceList AccessDevices
    {
      get; set;
    }


    CaptureDeviceList CaptureDevices
    {
      get; set;
    }


    PhotoList Photos
    {
      get; set;
    }


    CardList Cards
    {
      get; set;
    }


    LocationList Locations
    {
      get; set;
    }




  }
}
