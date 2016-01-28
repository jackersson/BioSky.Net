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

      _locator = locator;
      _bioEngine = locator.GetProcessor<IBioEngine>();
      _selector = locator.GetProcessor<ViewModelSelector>();
      _bioService = _locator.GetProcessor<IServiceManager>();

      _visitors         = new ObservableCollection<Visitor>();
      _selectedItemIds  = new ObservableCollection<long>();

      _bioEngine.Database().VisitorChanged += VisitorsViewModel_DataChanged;

      LocationId = -1;

      //LastVisitor = new Visitor() { Id = 1, Locationid = 1, Personid = 1, Photoid = 1, Status = 0, Time = 11243141421 };
/*
      VisitorList visitors = _bioEngine.Database().Visitors;

      foreach (Visitor item in visitors.Visitors)
      {
        if (Visitors.Contains(item))
          return;

        Visitors.Add(item);
      }*/

    }

    /*
        public void Update(long locationId)
        {
          if (locationId == null)
            return;

          LocationId = locationId;
        }*/

    protected async override void OnActivate()
    {
      if (_bioEngine.Database().Visitors.Visitors.Count <= 0)
        await _bioService.DatabaseService.VisitorRequest(new CommandVisitor());
      else
        VisitorsViewModel_DataChanged(null, null);
    }


    public void VisitorsViewModel_DataChanged(object sender, EventArgs args)
    {
      OnPersonsChanged(_bioEngine.Database().Visitors);
    }

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

    }

    public void Update()
    {
      VisitorsViewModel_DataChanged(null, null);
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
                             , new object[] { _bioEngine.Database().Persons.Persons.Where(x => x.Id == loc.Personid).FirstOrDefault() });
      }
    }

    private readonly IProcessorLocator _locator   ;
    private readonly ViewModelSelector _selector  ;
    private readonly IBioEngine        _bioEngine ;
    private readonly IServiceManager   _bioService;
  }
}
