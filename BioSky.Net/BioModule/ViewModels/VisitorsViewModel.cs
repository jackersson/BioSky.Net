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

using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class VisitorsViewModel : Screen
  {  
    private readonly IBioEngine _bioEngine;
    public VisitorsViewModel(IBioEngine bioEngine, ViewModelSelector viewModelsSelector )
    {
      _bioEngine = bioEngine;
      _filter = "";

      _viewModelsSelector = viewModelsSelector;

      _visitors         = new ObservableCollection<Visitor>();
      _filteredVisitors = new ObservableCollection<Visitor>();

      Visitors = _bioEngine.Database().GetAllVisitors();

      if (_filter == "")
        FilteredVisitors = Visitors;

      DisplayName = "Visitors";
    }

    public void Update()
    {
      NotifyOfPropertyChange(() => FilteredVisitors);
    }

    private IEnumerable<Visitor> _filteredVisitors;
    public IEnumerable<Visitor> FilteredVisitors
    {
      get
      {
        if (_filter != "")
          _filteredVisitors = _visitors.Where(x => x.Location != null && x.Location.Location_Name == _filter);
       
        return _filteredVisitors;
      }
      set
      {
        if (_filteredVisitors != value)
        {
          _filteredVisitors = value;
          NotifyOfPropertyChange(() => FilteredVisitors);
        }
      }
    }

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

    public string Caption()
    {
      return "Visitors";
    }

    private string _filter;
  
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

    ViewModelSelector _viewModelsSelector;
  }
}
