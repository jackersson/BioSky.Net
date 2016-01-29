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

using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

using BioModule.Utils;
using BioContracts;
using BioFaceService;
using Google.Protobuf.Collections;
using System.Collections;

namespace BioModule.ViewModels
{
  public class VisitorsViewModel : Screen
  {      
    public VisitorsViewModel(IProcessorLocator locator )
    {
      DisplayName = "Visitors";

      _locator    = locator;
      _bioEngine  = _locator.GetProcessor<IBioEngine>();
      _selector   = _locator.GetProcessor<ViewModelSelector>();
      _bioService = _locator.GetProcessor<IServiceManager>();
      _database   = _locator.GetProcessor<IBioSkyNetRepository>();

      _visitors         = new ObservableCollection<Visitor>();
      _selectedItemIds  = new ObservableCollection<long>();

      LocationId = -1;

      Visitors = _database.Visitors;

      if (Visitors.Count != 0)
        LastVisitor = Visitors[Visitors.Count - 1];    
    }

/*
    private void OnPersonsChanged(VisitorList visitors)
    {
      Visitors.Clear();

      foreach (Visitor item in visitors.Visitors)
      {
        if (LocationId != -1 && LocationId != item.Locationid)
          continue;

        if (Visitors.Contains(item))
          return;
        
        Visitors.Add(item);
      }

      if(Visitors.Count != 0)
      {
        LastVisitor = Visitors[Visitors.Count - 1];
      }

    }*/

    public void Update()
    {
      NotifyOfPropertyChange(() => Visitors);
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

    private Visitor _lastVisitor;
    public Visitor LastVisitor
    {
      get { return _lastVisitor; }
      set
      {
        if (_lastVisitor != value)
        {
          _lastVisitor = value;
          NotifyOfPropertyChange(() => LastVisitor);
        }
      }
    }

    private long _locationId;
    public long LocationId
    {
      get { return _locationId; }
      set
      {
        if (_locationId != value)
        {
          _locationId = value;
          NotifyOfPropertyChange(() => LocationId);
        }
      }
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

    private ObservableCollection<long> _selectedItemIds;
    public ObservableCollection<long> SelectedItemIds
    {
      get { return _selectedItemIds; }
      set
      {
        if (_selectedItemIds != value)
        {
          _selectedItemIds = value;
          NotifyOfPropertyChange(() => SelectedItemIds);
        }
      }
    }
    public void OnSelectionChanged(SelectionChangedEventArgs e)
    {

      IList selectedRecords = e.AddedItems as IList;
      IList unselectedRecords = e.RemovedItems as IList;

      foreach (Visitor currentUser in selectedRecords)
      {
        SelectedItemIds.Add(currentUser.Id);
      }

      foreach (Visitor currentUser in unselectedRecords)
      {
        SelectedItemIds.Remove(currentUser.Id);
      }

      foreach (long item in SelectedItemIds)
      {
        Console.WriteLine(item);
      }
    }
    public void OnMouseRightButtonDown(Visitor visitor)
    {
      MenuOpenStatus = (visitor != null);
      SelectedItem = visitor;
    }
    public void ShowUserPage()
    {      
      foreach (long item in SelectedItemIds)
      {
        Visitor loc = _bioEngine.Database().GetVisitorByID(item);
        _selector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.UserPage
                             , new object[] { _bioEngine.Database().GetPersonByID(loc.Personid) });
      }       
    }

    private readonly IProcessorLocator    _locator   ;
    private readonly ViewModelSelector    _selector  ;
    private readonly IBioEngine           _bioEngine ;
    private readonly IServiceManager      _bioService;
    private readonly IBioSkyNetRepository _database  ;
  }
}
