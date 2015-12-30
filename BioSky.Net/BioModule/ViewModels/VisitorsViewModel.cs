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
  public class VisitorsViewModel : PropertyChangedBase
  {
    private readonly ViewModelSelector _selector ;
    private readonly IBioEngine        _bioEngine;
    public VisitorsViewModel(IBioEngine bioEngine, ViewModelSelector selector, string filter = "")
    {
      _bioEngine = bioEngine;
      _filter    = filter;
      _selector  = selector;

      _selectedItems    = new ObservableCollection<bool>   ();
      _visitors         = new ObservableCollection<Visitor>();
      _filteredVisitors = new ObservableCollection<Visitor>();

      Visitors = _bioEngine.Database().GetAllVisitors();

      if (filter == "")
        FilteredVisitors = Visitors;    
     foreach(Visitor v in FilteredVisitors)
     {
       SelectedItems.Add(false);
     }
      
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
    private bool? _isAllItemsSelected;
    public bool? IsAllItemsSelected
    {
      get { return _isAllItemsSelected; }
      set
      {
        if (_isAllItemsSelected == value) return;

        _isAllItemsSelected = value;

        if (_isAllItemsSelected.HasValue)
          SelectAll(_isAllItemsSelected.Value, FilteredVisitors);

        NotifyOfPropertyChange(() => IsAllItemsSelected);
      }
    }

    private bool _isSelected;
    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected == value) return;
        _isSelected = value;
        NotifyOfPropertyChange(() => IsSelected);
      }
    }

    private void SelectAll(bool select, IEnumerable<Visitor> FilteredVisitors)
    {
      for (int i = 0; i != SelectedItems.Count; ++i)
        SelectedItems[i] = select;
    }
 

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

    private ObservableCollection<bool> _selectedItems;
    public ObservableCollection<bool> SelectedItems
    {
      get
      {
        return _selectedItems;
      }
      set
      {
        if (_selectedItems != value)
          _selectedItems = value;

        NotifyOfPropertyChange(() => SelectedItems);
      }
    }

    private bool _canOpenInNewTab;
    public bool CanOpenInNewTab
    {
      get { return _canOpenInNewTab; }
      set
      {
        if (_canOpenInNewTab != value)
        {
          _canOpenInNewTab = value;
          NotifyOfPropertyChange(() => CanOpenInNewTab);
        }
      }
    }
    public void OnMouseRightButtonDown(Visitor visitor)
    {
      CanOpenInNewTab = (visitor != null);
      SelectedItem = visitor;
    }

    public void ShowUserPage()
    {
      _selector.OpenTab(ViewModelsID.UserPage, new object[] { SelectedItem.User });
    }
    public void RemoveVisitor()
    {
      _visitors.Remove(SelectedItem);
    }

    public void OnSelect()
    {
    }

    public void SelectedRowsChangeEvent(SelectionChangedEventArgs e)
    {      
      var selected = e.AddedItems;

      for(int i=0; i != selected.Count; ++i)
      {        
        Console.WriteLine("Selected: " + selected[i]);
      }
      var unselected = e.RemovedItems;
      for (int i = 0; i != unselected.Count; ++i)
      {
        Console.WriteLine("Unselected: " + unselected[i]);
      }
    }

    //--------------------------------------------------- UI --------------------------------------
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
}
