using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Drawing;
using System.Reflection;

using BioModule.ResourcesLoader;
using System.ComponentModel;
using System.Collections.ObjectModel;

using BioData;
using BioModule.Model;

using System.Windows.Input;

using System.Windows.Data;


namespace BioModule.ViewModels
{ 
  public class UsersViewModel : PropertyChangedBase
  {    
    IBioEngine _bioEngine;
    public UsersViewModel(IBioEngine bioEngine)
    {
      _bioEngine = bioEngine;
      _users = new ObservableCollection<User>();    

      List<User> users = (List<User>)_bioEngine.Database().getAllUsers();
      foreach (User user in users)
        _users.Add(user);   
      
    }

    private ObservableCollection<User> _users;
    public ObservableCollection<User> Users
    {
      get { return _users; }
      set
      {
        if ( _users != value )
        {
          _users = value;
          NotifyOfPropertyChange(() => Users);
        }
      }
    }  


    public BitmapSource AddIconSource
    {
      get { return ResourceLoader.AddIconSource; }
    }

    public BitmapSource RemoveIconSource
    {
      get { return ResourceLoader.RemoveIconSource; }
    }

    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }


    
    //*************************************************************Context Menu******************************************\
   

    private User _selectedItem;
    public User SelectedItem
    {
      get
      {
        return _selectedItem;
      }
      set
      {
        if (_selectedItem != value)
          _selectedItem = value;

        NotifyOfPropertyChange(() => SelectedItem);
      }
    }

    private bool _menuOpenStatus;
    public bool MenuOpenStatus
    {
      get
      {
        return _menuOpenStatus;
      }
      set
      {
        if (_menuOpenStatus != value)
          _menuOpenStatus = value;

        NotifyOfPropertyChange(() => MenuOpenStatus);
      }
    }    

    public void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {           
    }


    //************************************************************SearchBox***************************************************

    public void OnSearchTextChanged(string s)
    {
      Console.WriteLine(s);      
    }  
  }

  //**********************************************************String to Image Converter****************************************

  public class ConvertTextToImage : IValueConverter
  {    
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        BitmapImage img = new BitmapImage(new Uri(value.ToString(), UriKind.RelativeOrAbsolute));
        return new BitmapImage(new Uri(value.ToString(), UriKind.RelativeOrAbsolute));
      }
      return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }   
  }
}
