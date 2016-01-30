using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

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
    Card GetCardByNumber(string number);

    void UpdatePersonSet(PersonList Persons);
    void UpdateVisitorSet(VisitorList Visitors);
    void UpdateLocationSet(LocationList Locations);
    void UpdateAccessDeviceSet(AccessDeviceList AccessDevices);
    void UpdateCaptureDeviceSet(CaptureDeviceList CaptureDevices);
    void UpdatePhotoSet(PhotoList Photos);
    void UpdateCardSet(CardList Cards);


    void UpdatePerson(Person person, DbState state);

    ObservableCollection<Person> Persons { get; }
    ObservableCollection<Visitor> Visitors { get; }
    ObservableCollection<Location> Locations { get; }
    ObservableCollection<AccessDevice> AccessDevices { get; }
    ObservableCollection<CaptureDevice> CaptureDevices { get; }
    ObservableCollection<Photo> Photos { get; }
    ObservableCollection<Card> Cards { get; }

  }
}
