using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData
{
  public interface IBioSkyNetRepository
  {
    IList<User>    getAllUsers();
    IList<Visitor> getAllVisitors();
    IList<Location> getAllLocations();

    void saveUserChanges();

  

  }
}
