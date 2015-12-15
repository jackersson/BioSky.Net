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
using BioModule.Model;

using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace BioModule.ViewModels
{
  public class VisitorsViewModel : PropertyChangedBase
  {

    private ObservableCollection<Visitor> _visitors;
    public ObservableCollection<Visitor> Visitors
    {
      get { return _visitors; }
      set
      {
        if (_visitors != value)
        {
          _visitors = value;
          NotifyOfPropertyChange(() => Visitors);
        }
      }
    }

    IBioEngine _bioEngine;
    public VisitorsViewModel(IBioEngine bioEngine)
    {
      _bioEngine    = bioEngine;
      _visitors     = new ObservableCollection<Visitor>();
    
      
      List <Visitor> visitors = (List<Visitor>)_bioEngine.Database().getAllVisitors();
      foreach (Visitor visitor in visitors)
        _visitors.Add(visitor);

      NotifyOfPropertyChange(() => Visitors);
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
//**********************************************************Context Menu*****************************************************
    
  
    private Visitor _selectedItem;
    public Visitor SelectedItem
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

   
  }
}
