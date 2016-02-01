using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;

using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using BioData;
using System.Collections.ObjectModel;

using System.Windows.Data;
using System.Reflection;
using System.Globalization;
using BioFaceService;
using BioModule.Utils;

namespace BioModule.ViewModels
{ 
  public class UserInformationViewModel : Screen, IUpdatable
  {    
    public UserInformationViewModel()
    {           
      DisplayName = "Information";     
    }
        
    public void Update( Person user)
    {
      User = user;      
    }

    public void Apply()
    {
      
    }

    public List<string> GenderSources
    {
      get { return Enum.GetNames(typeof(BioFaceService.Person.Types.Gender)).ToList(); }
    }

    public List<string> RightsSources
    {
      get { return Enum.GetNames(typeof(BioFaceService.Person.Types.Rights)).ToList(); }
    }

    private Person _user;
    public Person User
    {
      get { return _user; }
      set
      {
       if (_user != value)
       {
         _user = value;
         NotifyOfPropertyChange(() => GenderSources);
          NotifyOfPropertyChange(() => User);
       }
      }
    } 
    
  }
}
