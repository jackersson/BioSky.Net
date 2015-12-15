using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData
{
  public class BioSkyNetRepository : IBioSkyNetRepository
  {

    private IEntityFrameworkContextFactory _entityFrameworkContextFactory;

    public BioSkyNetRepository(IEntityFrameworkContextFactory entityFrameworkContextFactory)
    {
      if (entityFrameworkContextFactory == null)
        throw new ArgumentNullException("entityFrameworkContextFactory");

      _entityFrameworkContextFactory = entityFrameworkContextFactory;
    }

    public IList<User> getAllUsers()
    {
      try
      {
        using (var dbContext = _entityFrameworkContextFactory.Create<BioSkyNetEntities>())
        {
          return dbContext.Users.ToList();          
        }
      }
      catch ( Exception e)
      {
        return new List<User>();
      }
    }

    public IList<Visitor> getAllVisitors()
    {
      try
      {
        using (var dbContext = _entityFrameworkContextFactory.Create<BioSkyNetEntities>())
        {         
          return dbContext.Visitors.ToList();
        }
      }
      catch (Exception e)
      {
        return new List<Visitor>();
      }
    }

    public IList<Location> getAllLocations()
    {
      try
      {
        using (var dbContext = _entityFrameworkContextFactory.Create<BioSkyNetEntities>())
        {          
          return dbContext.Locations.ToList();          
        }
      }
      catch (Exception e)
      {
        return new List<Location>();
      }
    }

    public void saveUserChanges()
    {
      try
      {
        using (var dbContext = _entityFrameworkContextFactory.Create<BioSkyNetEntities>())
        {
          
          dbContext.SaveChanges();
          
        }
      }
      catch (Exception e)
      {
       
      }
    }

  }
}
