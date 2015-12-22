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

    IEnumerable<Visitor> GetVisitorsByLocation(string locationName);

    void AddUser(User user);
    void AddVisitor(Visitor visitor);
    void SaveChanges();

  

  }
}
