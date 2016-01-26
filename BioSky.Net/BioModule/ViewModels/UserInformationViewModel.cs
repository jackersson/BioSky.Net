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
//using static BioFaceService.Person.Types;



namespace BioModule.ViewModels
{ 
  public class UserInformationViewModel : Screen
  {    
    public UserInformationViewModel()
    {           
      DisplayName = "Information";     
    }    

    /*
    public void Update( Person user)
    {
      User = user;      
    }

    public List<string> GenderSources
    {
      get { return Enum.GetNames(typeof(Gender)).ToList(); }
    }

    public List<string> RightsSources
    {
      get { return Enum.GetNames(typeof(Rights)).ToList(); }
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
          NotifyOfPropertyChange(() => User);
       }
      }
    } 
    */
  }
}
