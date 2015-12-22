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

namespace BioData
{
  public class BioSkyNetRepository : IBioSkyNetRepository
  {

    private readonly IEntityFrameworkContextFactory _entityFrameworkContextFactory;
    private BioSkyNetEntities _bioSkyNetContext;
    private ObservableCollection<Notification> _notifications;

    public BioSkyNetRepository(IEntityFrameworkContextFactory entityFrameworkContextFactory)
    {
      if (entityFrameworkContextFactory == null)
        throw new ArgumentNullException("entityFrameworkContextFactory");

      _entityFrameworkContextFactory = entityFrameworkContextFactory;

      _notifications = new ObservableCollection<Notification>();

      InitializeContext();
      
    }

    public bool InitializeContext()
    {
      try
      {
        _bioSkyNetContext = _entityFrameworkContextFactory.Create<BioSkyNetEntities>();

        _bioSkyNetContext.Configuration.AutoDetectChangesEnabled = true;

        _bioSkyNetContext.Users.ToList();
        _bioSkyNetContext.Visitors.ToList();
        _bioSkyNetContext.Locations.ToList();

        return true;
      }
      catch {}

      return false;
    }

    public ObservableCollection<User> GetAllUsers()
    {
      try
      {      
        return _bioSkyNetContext.Users.Local;       
      }
      catch 
      {
        return new ObservableCollection<User>();
      }
    }

    public void AddUser( User user )
    {
      try
      {
        _bioSkyNetContext.Users.Add(user);
      }
      catch {}
    }

    public ObservableCollection<Visitor> GetAllVisitors()
    {
      try
      {              
        return _bioSkyNetContext.Visitors.Local;        
      }
      catch 
      {
        return new ObservableCollection<Visitor>();
      }
    }

    public IEnumerable<Visitor> GetVisitorsByLocation(string locationName)
    {
      try
      {
        return _bioSkyNetContext.Visitors.Local.Where(x => x.Location != null && x.Location.Location_Name == locationName);
      }
      catch
      {
        return new ObservableCollection<Visitor>();
      }
    }

    public void AddVisitor(Visitor visitor)
    {
      try
      {
        _bioSkyNetContext.Visitors.Add(visitor);
      }
      catch
      {
    
      }
    }

    public ObservableCollection<Location> GetAllLocations()
    {
      try
      {
        return _bioSkyNetContext.Locations.Local;         
      }
      catch
      {
        return new ObservableCollection<Location>();
      }
    }    


   
    public void SaveChanges()
    {
      try
      {       
        _bioSkyNetContext.SaveChanges();      
      }
      catch (DbEntityValidationException e)
      {
        foreach (var validationErrors in e.EntityValidationErrors)
        {
          foreach (var validationError in validationErrors.ValidationErrors)
          {
            Trace.TraceInformation("Property: {0} Error: {1}",
                                    validationError.PropertyName,
                                    validationError.ErrorMessage);
          }
        }
      }
    }

  }
}
