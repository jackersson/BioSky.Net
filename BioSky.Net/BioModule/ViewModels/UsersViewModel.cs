using System;
using System.Collections;
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

using BioModule.Utils;

using System.Text.RegularExpressions;
using System.Windows.Documents;


namespace BioModule.ViewModels
{ 
  public class UsersViewModel : PropertyChangedBase
  {    
    public UsersViewModel(IBioEngine bioEngine, ViewModelSelector selector)
    {
      _bioEngine = bioEngine;
      _selector  = selector;
     
      _users     = new ObservableCollection<User>();

      FilteredUsers = new ObservableCollection<User>(); 

      List<User> users = (List<User>)_bioEngine.Database().getAllUsers();
      foreach (User user in users)
        _users.Add(user);


      User user2 = new User { First_Name_ = "Sasha", Last_Name_ = "Iskra", Email = "adaman1991@mail.ru", Gender="Male" , Rights="Operator" };
      _users.Add(user2);

      User user3 = new User { First_Name_ = "Sasha", Last_Name_ = "Iskra", Email = "adaman1991@mail.ru", Gender = "Male", Rights = "Operator" };
      _users.Add(user3);

      User user4 = new User { First_Name_ = "Taras", Last_Name_ = "Iskra", Email = "adaman1991@mail.ru", Gender = "Male", Rights = "Operator" };
      _users.Add(user4);

      foreach (User user in _users)
        FilteredUsers.Add(user);
      
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

    private ObservableCollection<User> _filteredUsers;
    public ObservableCollection<User> FilteredUsers
    {
      get { return _filteredUsers; }
      set
      {
        if (_filteredUsers != value)
        {
          _filteredUsers = value;
          NotifyOfPropertyChange(() => FilteredUsers);
        }
      }
    }

    public string Caption()
    {
      return "Users";
    }
     
        
    //*************************************************************Context Menu******************************************\
   
    private User _selectedItem;
    public User SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem != value)
        {
          _selectedItem = value;
          NotifyOfPropertyChange(() => SelectedItem);
        }          
      }
    }

    private bool _canOpenInNewTab;
    public bool CanOpenInNewTab
    {
      get  { return _canOpenInNewTab; }
      set
      {
        if (_canOpenInNewTab != value)
        {
          _canOpenInNewTab = value;
          NotifyOfPropertyChange(() => CanOpenInNewTab);
        }          
      }
    }    

    public void OnMouseRightButtonDown( User user )
    {
      CanOpenInNewTab = (user != null);
      SelectedItem = user;
    }

    public void ShowUserPage()
    {     
      _selector.OpenTab(ViewModelsID.UserPage, new object[] { SelectedItem } );
    }

    //************************************************************SearchBox***************************************************

    public void OnSearchTextChanged(string s, Object UsersList)
    {
      FilteredUsers.Clear();
      if (s == "")
      {
        foreach (User user in _users)
          FilteredUsers.Add(user);        
      }
      else
      {
       
        IEnumerable<User> filter = Users.Where(x => (x.First_Name_.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0));
        foreach (User user in filter)
          FilteredUsers.Add(user);
      }
      NotifyOfPropertyChange(() => FilteredUsers);
      TestBlockText = s;
      NotifyOfPropertyChange(() => TestBlockText);
/*
      UIElementCollection gr = UsersList as UIElementCollection;

      foreach (Control cl in gr)
      {
        if (cl is ListView || cl is DataGrid || cl is TreeView)
        {
          FindControlItem(cl, s);
        }
      }*/
        
        //HighlightText(UsersList, s);
      
     // TxtSearchText_TextChanged(s, UsersList);
      Console.WriteLine(s);      
    }

    private readonly ViewModelSelector _selector;
    private readonly IBioEngine        _bioEngine;

    

    Regex regex;
    private void TxtSearchText_TextChanged(string searchText, DataGrid UsersList)
    {
      FindControlItem(UsersList, searchText);
    }

    public void FindControlItem(DependencyObject obj, string searchText)
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
      {
        ListViewItem lv = obj as ListViewItem;
        Object dg = obj as DataGridCell;
        TreeViewItem tv = obj as TreeViewItem;
        if (lv != null)
        {
          HighlightText(lv, searchText);
        }
        if (dg != null)
        {
          HighlightText(dg, searchText);
        }
        if (tv != null)
        {
          HighlightText(tv, searchText);
        }
        FindControlItem(VisualTreeHelper.GetChild(obj as DependencyObject, i), searchText);
      }
    }

    private void HighlightText(Object itx, string searchText)
    {
      if (itx != null)
      {
        if (itx is TextBlock)
        {
          regex = new Regex("(" + searchText + ")", RegexOptions.IgnoreCase);
          TextBlock tb = itx as TextBlock;
          if (searchText.Length == 0)
          {
            
            string str = tb.Text;
            tb.Inlines.Clear();
            tb.Inlines.Add(str);
            return;
          }
          string[] substrings = regex.Split(tb.Text);
          tb.Inlines.Clear();
          foreach (var item in substrings)
          {
            if (regex.Match(item).Success)
            {
              Run runx = new Run(item);
              runx.Background = System.Windows.Media.Brushes.LightBlue;
              tb.Inlines.Add(runx);
            }
            else
            {
              tb.Inlines.Add(item);
            }
          }
          return;
        }
        else
        {
          for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
          {
            HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i), searchText);
          }
        }
      }
    }

    private string _testBlockText;
    public string TestBlockText
    {
      get { return _testBlockText; }
      set
      {
        if (_testBlockText != value)
        {
          _testBlockText = value;
          NotifyOfPropertyChange(() => TestBlockText);
        }
      }
    }


    //************************************************************** UI *****************************************8
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
  public class ConvertToFormatedRuns : IValueConverter
  {
    public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var tb = new TextBlock();

      tb.Inlines.Add(new Run() { Text = (string)values, Background = System.Windows.Media.Brushes.Yellow });

      string searchText = "as";

      Regex regex = new Regex("(" + searchText + ")", RegexOptions.IgnoreCase);
      
      if (searchText.Length == 0)
      {

        string str = (string)values;
        tb.Inlines.Clear();
        tb.Inlines.Add(str);
        return tb;
      }
      string[] substrings = regex.Split((string)values);
      tb.Inlines.Clear();
      foreach (var item in substrings)
      {
        if (regex.Match(item).Success)
        {
          Run runx = new Run(item);
          runx.Background = System.Windows.Media.Brushes.LightBlue;
          tb.Inlines.Add(runx);
        }
        else
        {
          tb.Inlines.Add(item);
        }
      }

      return tb;
    }
    
    public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    } 
  }
}
