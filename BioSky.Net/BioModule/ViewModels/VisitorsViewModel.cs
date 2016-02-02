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

      _selectedItemIds = new ObservableCollection<long>();

      LocationId = -1;

      _database.PhotoHolder.DataChanged   += RefreshData;
      _database.VisitorHolder.DataChanged += RefreshData;   
    } 

    public void OnDataContextChanged()
    {
      ImageView = new ImageViewModel();
    }
    protected override void OnActivate()
    {      
      base.OnActivate();
      RefreshData();
    }
    
    private void RefreshData()
    {
      Visitors = null;
      Visitors = _database.VisitorHolder.Data;    
      GetLastVisitor();
    }
    private void GetLastVisitor()
    {
      if (Visitors.Count != 0)
        LastVisitor = Visitors[Visitors.Count - 1];
    }

    private ImageViewModel _imageView;
    public ImageViewModel ImageView
    {
      get { return _imageView; }
      set
      {
        if (_imageView != value)
        {
          _imageView = value;
          NotifyOfPropertyChange(() => ImageView);
        }
      }
    }


    public void Update()
    {
      NotifyOfPropertyChange(() => Visitors);
    }

    private AsyncObservableCollection<Visitor> _visitors;
    public AsyncObservableCollection<Visitor> Visitors
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

      if(SelectedItemIds.Count == 1)
      {
        Visitor visitor = null;
        bool visitorExists = _bioEngine.Database().VisitorHolder.DataSet.TryGetValue(SelectedItemIds[0], out visitor);
        if (visitorExists)
        {
          Person person = null;
          bool personExists = _bioEngine.Database().PersonHolder.DataSet.TryGetValue(visitor.Personid, out person);
          if ( personExists )
          {
            Photo photo = null;
            bool photoExists = _bioEngine.Database().PhotoHolder.DataSet.TryGetValue(person.Thumbnail, out photo);
            if ( photoExists )
            {
              /*
              string personFolder = _bioEngine.Database().PersonsFolderAddress + "\\" + person.Id;

              Uri uri = new Uri(personFolder + "\\" + photo.FileLocation);
              if (uri == null)
                return;
              ImageView.UpdateImage(uri);
              */
            }
          }
        }   
      }
    }
    public void OnMouseRightButtonDown(Visitor visitor)
    {
      MenuOpenStatus = (visitor != null);
      SelectedItem = visitor;
    }
    public void ShowUserPage()
    {
      foreach (long id in SelectedItemIds)
      {

        Visitor visitor = null;
        bool visitorFound = _bioEngine.Database().VisitorHolder.DataSet.TryGetValue(id, out visitor);

        if (visitorFound)
        {
          Person person = null;
          bool personFound = _bioEngine.Database().PersonHolder.DataSet.TryGetValue(visitor.Personid, out person);
          _selector.ShowContent(ShowableContentControl.TabControlContent
                               , ViewModelsID.UserPage
                               , new object[] { person });
        }
      }
    }

    private readonly IProcessorLocator    _locator   ;
    private readonly ViewModelSelector    _selector  ;
    private readonly IBioEngine           _bioEngine ;
    private readonly IServiceManager      _bioService;
    private readonly IBioSkyNetRepository _database  ;
  }
}
