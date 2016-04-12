
using Caliburn.Micro;
using static BioService.Person.Types;
using BioContracts;
using System.Collections.Generic;

namespace BioEngine
{

  public class PermissionController : PropertyChangedBase
  {
    Dictionary<Rights, long> roleDictionary = new Dictionary<Rights, long>();

    private static  object syncObject = new object();

    private static volatile PermissionController _instance;
    public static PermissionController Instance
    {
      get
      {
        if (_instance == null)
        {
          lock (syncObject)
          {
            if (_instance == null)
              _instance = new PermissionController();
          }
        }
        return _instance;        
      }
    }

    private PermissionController()
    {
      Initialize();
    }

    private void Initialize()
    {
      long _managerRole    = 0;
      long _customRole     = 0;
      long _operatorRole   = 0;  
      long _superviserRole = 0;

      _managerRole = GenerateManagerRole(_managerRole);

      roleDictionary.Add(Rights.Operator  , _operatorRole  );
      roleDictionary.Add(Rights.Supervisor, _superviserRole);
      roleDictionary.Add(Rights.Manager   , _managerRole   );
      roleDictionary.Add(Rights.Custom    , _customRole    );
    }

    private long GenerateManagerRole(long _managerRole)
    {
      _managerRole = SetFlag(_managerRole, Activity.UserAdd       );
      _managerRole = SetFlag(_managerRole, Activity.UserUpdate    );
      _managerRole = SetFlag(_managerRole, Activity.UserRemove    );
      _managerRole = SetFlag(_managerRole, Activity.LocationAdd   );
      _managerRole = SetFlag(_managerRole, Activity.LocationUpdate);
      _managerRole = SetFlag(_managerRole, Activity.LocationRemove);
      _managerRole = SetFlag(_managerRole, Activity.VisitorRemove );
      _managerRole = SetFlag(_managerRole, Activity.CardAdd       );
      _managerRole = SetFlag(_managerRole, Activity.CardRemove    );
      _managerRole = SetFlag(_managerRole, Activity.PhotoRemove   );
      return _managerRole;
    }
    public void UpdateAuthenticatedPersonRights(Rights rights)
    {    
      long role = 0;
      roleDictionary.TryGetValue(rights, out role);
      _currentRole = role;

      CurrentPermissionRights = rights;
    }

    public bool isActivityAllowed(Activity activity)
    {
      return HasFlag(CurrentRole, activity);      
    }
    private long SetFlag(long role, Activity activity)
    {      
      return role | (long)activity;
    }
    private bool HasFlag(long role, Activity activity)
    {
      long activityL = (long)activity;
      return (role & activityL) == activityL;
    }

    private long _currentRole;
    private long CurrentRole
    {
      get { return _currentRole; }
    }


    private  Rights _currentPermissionRights;
    public  Rights CurrentPermissionRights
    {
      get { return _currentPermissionRights; }
      private set
      {
        if (_currentPermissionRights != value)
        {
          _currentPermissionRights = value;
          NotifyOfPropertyChange(() => CurrentPermissionRights);
        }
      }
    }

  }
}
