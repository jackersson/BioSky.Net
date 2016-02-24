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
using Grpc.Core;
using WPFLocalizeExtension.Extensions;

namespace BioModule.ViewModels
{
  public class VisitorsViewModel : Screen
  {
    public VisitorsViewModel(IProcessorLocator locator, IWindowManager windowManager)
    {
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Visitors_");

      _locator       = locator;
      _windowManager = windowManager;

      _bioEngine  = _locator.GetProcessor<IBioEngine>();
      _selector   = _locator.GetProcessor<ViewModelSelector>();
      _bioService = _locator.GetProcessor<IServiceManager>();
      _database   = _locator.GetProcessor<IBioSkyNetRepository>();

      _selectedItemIds = new ObservableCollection<long>();

      _sortDescriptionByTime = new SortDescription("Time", ListSortDirection.Descending);
      
      LocationId = -1;

      _database.PhotoHolder.DataChanged   += RefreshData;
      _database.Visitors.DataChanged += RefreshData;

/*
      if (Visitors != null)
        VisitorsCollectionView = new PagingCollectionView(Visitors, 5);*/

      VisitorsCollectionView = (PagingCollectionView)CollectionViewSource.GetDefaultView(Visitors);
    } 



    #region Database

    private void RefreshData()
    {
      Visitors = null;
      Visitors = _database.VisitorHolder.Data;
      GetLastVisitor();

      VisitorsCollectionView = CollectionViewSource.GetDefaultView(Visitors);
      VisitorsCollectionView.SortDescriptions.Add(SortDescriptionByTime);
/*
      if (Visitors != null)
      {
        VisitorsCollectionView = new PagingCollectionView(Visitors, 5);
        VisitorsCollectionView.AddSortDescription(SortDescriptionByTime);
      }*/


      
      
    }
    private void GetLastVisitor()
    {
      LastVisitor = null;     
      LastVisitor = Visitors.LastOrDefault();
    }

    #endregion

    #region BioService

    public async Task VisitorsDeletePerformer(EntityState state)
    {
      VisitorList visitorList = new VisitorList();

      foreach (long id in SelectedItemIds)
      {
        Visitor visitor = null;
        bool visitorExists = _bioEngine.Database().VisitorHolder.DataSet.TryGetValue(id, out visitor);
        if (visitorExists)
        {
          Visitor newVisitor = new Visitor()
          {
            Id = id
            , EntityState = EntityState.Deleted
            , Photoid = visitor.Photoid
          };
          visitorList.Visitors.Add(newVisitor);
        }
      }

      try
      {
        _database.Visitors.DataUpdated += UpdateData;
        await _bioService.DatabaseService.VisitorUpdate(visitorList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }
    }

    private void UpdateData(VisitorList list)
    {
      _database.Visitors.DataUpdated -= UpdateData;

      if (list != null)
      {
        Visitor visitor = list.Visitors.FirstOrDefault();
        if (visitor != null)
        {
          if (visitor.EntityState == EntityState.Deleted)
          {
            if (list.Visitors.Count > 1)
              MessageBox.Show(list.Visitors.Count + " visitors successfully Deleted");
            else
              MessageBox.Show("Visitor successfully Deleted");
          }
        }
      }
    }   

    #endregion

    #region Interface

    public async void OnDeleteVisitors()
    {
      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());

      if (result == true)
      {
        try
        {
          await VisitorsDeletePerformer(EntityState.Deleted);
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        }
      }
    }

    public void OnMouseDoubleClick(Visitor visitor)
    {
      if (visitor != null)
      {    
        if(visitor.Personid <= 0)        
          ShowVisitorAsUser(visitor);      
        else        
          ShowPerson(visitor.Id);        
      }
    }

    public void OnAddVisitorAsUser()
    {
      if (SelectedItem != null)
        ShowVisitorAsUser(SelectedItem);
    }

    public void ShowVisitorAsUser(Visitor visitor)
    {
      Photo photo = null;
      bool photoExists = _bioEngine.Database().PhotoHolder.DataSet.TryGetValue(visitor.Photoid, out photo);
      if (photoExists)
      {
        Person user = new Person()
        {
          Firstname = ""
          , Lastname = ""
          , Thumbnailid = 0
          , Gender = Person.Types.Gender.Male
          , Rights = Person.Types.Rights.Operator
          , EntityState = EntityState.Added
          , Thumbnail = photo
        };

        _selector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.UserPage
                   , new object[] { user }); 
      }
    }

    public void OnDataContextChanged()
    {
      ImageView      = new ImageViewModel(_locator, _windowManager)         ;
      PageController = new PageControllerViewModel();
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
        SelectedItemIds.Add(currentUser.Id);
      

      foreach (Visitor currentUser in unselectedRecords)      
        SelectedItemIds.Remove(currentUser.Id);

      if (SelectedItemIds.Count >= 1)
        IsDeleteButtonEnabled = true;
      else
        IsDeleteButtonEnabled = false;
      
      
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
      
/*
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
      };*/



      
    }
    public void OnMouseRightButtonDown(Visitor visitor)
    {
      MenuOpenStatus = (visitor != null);
      SelectedItem = visitor;
    }

    public void ShowPerson(long visitorId)
    {
      Visitor visitor = null;
      bool visitorFound = _bioEngine.Database().VisitorHolder.DataSet.TryGetValue(visitorId, out visitor);

      if (visitorFound)
      {
        Person person = null;
        bool personFound = _bioEngine.Database().PersonHolder.DataSet.TryGetValue(visitor.Personid, out person);
        _selector.ShowContent(ShowableContentControl.TabControlContent
                             , ViewModelsID.UserPage
                             , new object[] { person });
      }
    }

    public void ShowUserPage()
    {
      foreach (long id in SelectedItemIds)      
        ShowPerson(id);      
    }
    #endregion

    #region UI

    private bool _isDeleteButtonEnabled;
    public bool IsDeleteButtonEnabled
    {
      get { return _isDeleteButtonEnabled; }
      set
      {
        if (_isDeleteButtonEnabled != value)
        {
          _isDeleteButtonEnabled = value;
          NotifyOfPropertyChange(() => IsDeleteButtonEnabled);
        }
      }
    }

    private IList _list;
    public IList List
    {
      get { return _list; }
      set
      {
        if (_list != value)
        {
          _list = value;
          NotifyOfPropertyChange(() => List);
        }
      }
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

    private PageControllerViewModel _pageController;
    public PageControllerViewModel PageController
    {
      get { return _pageController; }
      set
      {
        if (_pageController != value)
        {
          _pageController = value;
          NotifyOfPropertyChange(() => PageController);
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
    private readonly IWindowManager       _windowManager;
    private readonly IProcessorLocator    _locator      ;
    private readonly ViewModelSelector    _selector     ;
    private readonly IBioEngine           _bioEngine    ;
    private readonly IServiceManager      _bioService   ;
    private readonly IBioSkyNetRepository _database     ;
    #endregion
  }

  public class PagingCollectionView : CollectionView
  {
    private readonly IList _innerList;
    private readonly int _itemsPerPage;

    private int _currentPage = 1;

    public PagingCollectionView(IList innerList, int itemsPerPage)
      : base(innerList)
    {
      this._innerList = innerList;
      this._itemsPerPage = itemsPerPage;
    }

    /*
            public override int Count
            {
                get 
                { 
                    if (this._currentPage < this.PageCount) // page 1..n-1
                    {
                        return this._itemsPerPage;
                    }
                    else // page n
                    {
                        var itemsLeft = this._innerList.Count % this._itemsPerPage;
                        if (0 == itemsLeft)
                        {
                            return this._itemsPerPage; // exactly itemsPerPage left
                        }
                        else
                        {
                            // return the remaining items
                            return itemsLeft;
                        }
                    }
                }
            }*/

    public override int Count
    {
      get
      {
        //all pages except the last
        if (CurrentPage < PageCount)
          return this._itemsPerPage;

        //last page
        int remainder = _innerList.Count % this._itemsPerPage;

        return remainder == 0 ? this._itemsPerPage : remainder;
      }
    }

    public int CurrentPage
    {
      get { return this._currentPage; }
      set
      {
        this._currentPage = value;
        this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentPage"));
      }
    }

    public int ItemsPerPage { get { return this._itemsPerPage; } }

    public int PageCount
    {
      get
      {
        return (this._innerList.Count + this._itemsPerPage - 1)
            / this._itemsPerPage;
      }
    }

    private int EndIndex
    {
      get
      {
        var end = this._currentPage * this._itemsPerPage - 1;
        return (end > this._innerList.Count) ? this._innerList.Count : end;
      }
    }

    private int StartIndex
    {
      get { return (this._currentPage - 1) * this._itemsPerPage; }
    }

    public override object GetItemAt(int index)
    {
      var offset = index % (this._itemsPerPage);
      return this._innerList[this.StartIndex + offset];
    }

    public void MoveToNextPage()
    {
      if (this._currentPage < this.PageCount)
      {
        this.CurrentPage += 1;
      }
      this.Refresh();
    }

    public void MoveToPreviousPage()
    {
      if (this._currentPage > 1)
      {
        this.CurrentPage -= 1;
      }
      this.Refresh();
    }

    public void AddSortDescription(SortDescription sortDescription)
    {      
      this.SortDescriptions.Add(sortDescription);
    }
  }
}
