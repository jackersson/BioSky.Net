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
    private BioSkyNetDataModel _bioSkyNetContext;
    

    public BioSkyNetRepository(IEntityFrameworkContextFactory entityFrameworkContextFactory)
    {
      if (entityFrameworkContextFactory == null)
        throw new ArgumentNullException("entityFrameworkContextFactory");

      _entityFrameworkContextFactory = entityFrameworkContextFactory;

      InitializeContext();
      
    }

    public bool InitializeContext()
    {
      try
      {
        _bioSkyNetContext = _entityFrameworkContextFactory.Create<BioSkyNetDataModel>();

        _bioSkyNetContext.Configuration.AutoDetectChangesEnabled = true;

        _bioSkyNetContext.Users.ToList();
        _bioSkyNetContext.Visitors.ToList();
        _bioSkyNetContext.Locations.ToList();
        _bioSkyNetContext.Cards.ToList();

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

     

    public bool AddUser( User user )
    {
      if (user == null)
        return false;

      try
      {

       User existingUser =  _bioSkyNetContext.Users.Where(x => x.First_Name_ == user.First_Name_
                                                            && x.Last_Name_  == user.Last_Name_).FirstOrDefault();

        if (existingUser != null )
          return false;

        _bioSkyNetContext.Users.Add(user);    

        return SaveChanges();
      }
      catch {}

      return false;
    }

    public bool UpdateUser(User user)
    {
      if (user == null)
        return false;

      try
      {
        _bioSkyNetContext.Entry(user).State = System.Data.Entity.EntityState.Modified;
        return SaveChanges();
      }
      catch { }

      return false;
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

    public ObservableCollection<Card> GetCards()
    {
      try
      {
        return _bioSkyNetContext.Cards.Local;
      }
      catch
      {
        return new ObservableCollection<Card>();
      }
    }

    public void UpdateCards(ObservableCollection<Card> cards, User user)
    {
      if (user == null || cards.Count <= 0)
        return;


      try
      {

        User existingUser = _bioSkyNetContext.Users.Where(x => x.First_Name_ == user.First_Name_
                                                         && x.Last_Name_ == user.Last_Name_).FirstOrDefault();

        if (existingUser == null)
          return;

        foreach (Card card in cards)
        {
          Card existingCard = _bioSkyNetContext.Cards.Where(x => x.CardID == card.CardID).FirstOrDefault();
          if ( existingCard != null && existingCard.UserID != existingUser.UID)
          {
            existingCard.UserID = existingUser.UID;
            _bioSkyNetContext.Entry(existingCard).State = System.Data.Entity.EntityState.Modified;
          }

          if (existingCard == null)
            _bioSkyNetContext.Cards.Add(card);
          
        }

        IEnumerable<Card> existingCards = _bioSkyNetContext.Cards.Where(x => x.UserID == existingUser.UID);
        foreach (Card card in existingCards)
        {
          if (!cards.Contains(card))
            _bioSkyNetContext.Cards.Remove(card);
        }

        SaveChanges();
      }
      catch
      {
       
      }
    }



    public bool SaveChanges()
    {
      try
      {       
        _bioSkyNetContext.SaveChanges();
        return true;     
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

      return false;
    }

  }
}
