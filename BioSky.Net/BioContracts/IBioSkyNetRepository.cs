using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IBioSkyNetRepository
  {
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
