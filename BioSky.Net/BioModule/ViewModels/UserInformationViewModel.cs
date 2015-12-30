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
    IBioEngine _bioEngine;
    public UserInformationViewModel(IBioEngine bioEngine)
    {     
      _bioEngine = bioEngine;
      DisplayName = "Information";
    }    
    public void Update(User user)
    {
      User = user;      
    }    
    
    override protected void OnActivate() 
    {
      Console.WriteLine("Activated " + DisplayName);
    }

    override protected void OnDeactivate(bool canClose)
    {
      
    }   

    //**************************************************Properties************************************************************

     private User _user;
     public User User
     {
       get
       {
         return _user;
       }
       set
       {
         if (_user != value)
           _user = value;

         NotifyOfPropertyChange(() => User);
       }
     }

    //*************************************************Icon Source**************************************************************

    public BitmapSource OkIconSource
    {
      get { return ResourceLoader.OkIconSource; }
    }

    public BitmapSource CancelIconSource
    {
      get { return ResourceLoader.CancelIconSource; }
    }

    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }
  }
}
