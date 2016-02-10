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
using BioService;
using Google.Protobuf.Collections;
using System.Collections;
using System.ComponentModel;
using System.Windows.Data;


namespace BioModule.ViewModels
{
  public class VisitorsViewModel : Screen
  {      
    public VisitorsViewModel(IProcessorLocator locator )
    {
      DisplayName = "Visitors_";

      _locator    = locator;
      _bioEngine  = _locator.GetProcessor<IBioEngine>();
      _selector   = _locator.GetProcessor<ViewModelSelector>();
      _bioService = _locator.GetProcessor<IServiceManager>();
      _database   = _locator.GetProcessor<IBioSkyNetRepository>();

      _selectedItemIds = new ObservableCollection<long>();

      _sortDescriptionByTime = new SortDescription("Time", ListSortDirection.Descending);

      LocationId = -1;

      _database.PhotoHolder.DataChanged   += RefreshData;
      _database.Visitors.DataChanged += RefreshData;

      VisitorsCollectionView = CollectionViewSource.GetDefaultView(Visitors);
    } 



    #region Database

    private void RefreshData()
    {
      Visitors = null;
      Visitors = _database.VisitorHolder.Data;
      GetLastVisitor();
      VisitorsCollectionView = CollectionViewSource.GetDefaultView(Visitors);
      VisitorsCollectionView.SortDescriptions.Add(SortDescriptionByTime);
    }
    private void GetLastVisitor()
    {
      LastVisitor = null;     
      LastVisitor = Visitors.LastOrDefault();
    }

    #endregion

    #region Interface

    public void OnDataContextChanged()
    {
      ImageView = new ImageViewModel();
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      RefreshData();
    }
    public void Update()
    {
      NotifyOfPropertyChange(() => Visitors);
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

      if (SelectedItemIds.Count == 1)
      {
        if (ImageView == null)
          return;

        Visitor visitor = null;
        bool visitorExists = _bioEngine.Database().VisitorHolder.DataSet.TryGetValue(SelectedItemIds[0], out visitor);
        if (visitorExists)
        {
          Photo photo = null;
          bool photoExists = _bioEngine.Database().PhotoHolder.DataSet.TryGetValue(visitor.Photoid, out photo);
          if (photoExists)            
            ImageView.UpdateImage(photo, _bioEngine.Database().LocalStorage.LocalStoragePath);
          else
            ImageView.UpdateImage(null, null);
        }
      }
    }

    public void OnSearchTextChanged(string s)
    {
      SearchText = s;

      VisitorsCollectionView.Filter = item =>
      {
        if (String.IsNullOrEmpty(SearchText))
          return true;

        Visitor vitem = item as Visitor;       

        Person person = null;
        bool personFound = _database.PersonHolder.DataSet.TryGetValue((long)vitem.Personid, out person);

        if (person == null)
          return false;

        if (person.Firstname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
            person.Lastname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)
        {
          return true;
        }
        return false;
      };     
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
    #endregion

    #region UI

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

    private SortDescription _sortDescriptionByTime;
    public SortDescription SortDescriptionByTime
    {
      get { return _sortDescriptionByTime; }
      set
      {
        if (_sortDescriptionByTime != value)
        {
          _sortDescriptionByTime = value;
          NotifyOfPropertyChange(() => SortDescriptionByTime);
        }
      }
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

    private ICollectionView _visitorsCollectionView;
    public ICollectionView VisitorsCollectionView
    {
      get { return _visitorsCollectionView; }
      set
      {
        if (_visitorsCollectionView != value)
        {
          _visitorsCollectionView = value;

          NotifyOfPropertyChange(() => VisitorsCollectionView);
        }
      }
    }

    private string _searchText;
    public string SearchText
    {
      get { return _searchText; }
      set
      {
        if (_searchText != value)
        {
          _searchText = value;

          NotifyOfPropertyChange(() => SearchText);
        }
      }
    }


    #endregion

    #region Global Variables
    private readonly IProcessorLocator    _locator   ;
    private readonly ViewModelSelector    _selector  ;
    private readonly IBioEngine           _bioEngine ;
    private readonly IServiceManager      _bioService;
    private readonly IBioSkyNetRepository _database;
    #endregion
  }
}
