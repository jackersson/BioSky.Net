using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;

using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using BioData;
using BioModule.Model;
using System.Collections.ObjectModel;

using System.Windows.Data;
using System.Reflection;
using System.Globalization;

namespace BioModule.ViewModels
{ 
  public class UserInformationViewModel : Screen
  {    
    public UserInformationViewModel()
    {           
      DisplayName = "Information";     
    }    

    public void Update(ref User user)
    {
      User = user;      
    }    
    
    private User _user;
    public User User
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

  }
}
