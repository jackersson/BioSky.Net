using BioContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData
{
  public interface IBioSkyNetRepository
  {
    ObservableCollection<User>    GetAllUsers();
    ObservableCollection<Visitor> GetAllVisitors();
    ObservableCollection<Location> GetAllLocations();

    ObservableCollection<Card> GetCards();

    IEnumerable<Visitor> GetVisitorsByLocation(string locationName);

    bool AddUser(User user);
    void UpdateCards(ObservableCollection<Card> cards, User user);
    bool UpdateUser(User user);
    void AddVisitor(Visitor visitor);
    bool SaveChanges();

  

  }
}
