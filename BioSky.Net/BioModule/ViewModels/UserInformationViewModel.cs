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
 
  public class StringToGenderConverter : IValueConverter
  {
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      Gender gender;
      Enum.TryParse(value.ToString(), out gender);
      return gender;
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      return Enum.GetName(value.GetType(), value);       
    }
  }

  public class StringToRightsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
      Rights right;
      Enum.TryParse(value.ToString(), out right);
      return right;
    }

    public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
    {
      return Enum.GetName(value.GetType(), value); ;
    }
  }

  public class UserInformationViewModel : PropertyChangedBase
  {
    IBioEngine _bioEngine;
    public UserInformationViewModel(IBioEngine bioEngine)
    {     
      _bioEngine = bioEngine;   
    }

    public void Update(User user)
    {
      User = user;      
    }

    public void SaveUserInfo()
    {      
      _bioEngine.Database().saveUserChanges();       
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

    //**************************************************Propertyes************************************************************

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
  }
}
