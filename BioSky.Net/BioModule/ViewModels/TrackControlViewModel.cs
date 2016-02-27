using System;
using System.Linq;
using System.Threading.Tasks;

using BioContracts;
using Caliburn.Micro;
using System.Collections.ObjectModel;

using System.Windows;
using BioModule.Utils;
using System.Windows.Controls;
using BioService;
using Grpc.Core;
using WPFLocalizeExtension.Extensions;

namespace BioModule.ViewModels
{
  public class TrackControlViewModel : Screen
  {
    public TrackControlViewModel(IProcessorLocator locator, IWindowManager windowManager)
    {
      _locator       = locator      ;
      _windowManager = windowManager;

      _bioEngine  = locator.GetProcessor<IBioEngine>();
      _selector   = locator.GetProcessor<ViewModelSelector>();
      _database   = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioService = _locator.GetProcessor<IServiceManager>();


      _methodInvoker = new FastMethodInvoker();

      TrackTabControlView = new TrackTabControlViewModel(_locator);

      _visitorsView = new VisitorsViewModel(locator);

      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Tracking_");

      _bioEngine.TrackLocationEngine().TrackLocations.CollectionChanged += TrackLocations_CollectionChanged;
    }

    #region Update
    public void TrackLocations_CollectionChanged(object sender, EventArgs args)
    {
      foreach (TrackLocation location in TrackControlItems)
      {
        if (location.ScreenViewModel == null)
          location.ScreenViewModel = new TrackControlItemViewModel(_locator, location);
      }

      if (SelectedTrackLocation == null)
        TrackTabControlView.Update(TrackControlItems[0]);

      TrackTabControlView.Update(SelectedTrackLocation);
    }

    #endregion

    #region BioService

    public async Task LocationDeletePerformer(EntityState state)
    {
      LocationList locationList = new LocationList();

      if (SelectedTrackLocation == null)
        return;

      Location location = new Location() { Id = SelectedTrackLocation.LocationID
                                         , EntityState = EntityState.Deleted };
      locationList.Locations.Add(location);      

      try
      {
       // _database.Locations.DataUpdated += UpdateData;
       // await _bioService.DatabaseService.LocationUpdate(locationList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }
    }

    private void UpdateData(LocationList list)
    {
      _database.Locations.DataUpdated -= UpdateData;

      if (list != null)
      {
        Location location = list.Locations.FirstOrDefault();
        if (location != null)
        {
          if (location.EntityState == EntityState.Deleted)
          {
            if (list.Locations.Count > 1)
              MessageBox.Show(list.Locations.Count + " locations successfully Deleted");
            else
              MessageBox.Show("Location successfully Deleted");
          }
        }
      }
    }   

    #endregion

    #region Interface

    public async void OnDeleteLocation()
    {
      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());

      if (result == true)
      {
        try
        {
          await LocationDeletePerformer(EntityState.Deleted);
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        }
      }
    }

    protected override void OnActivate()
    {
      //TODO refresh
      //if (_visitorsView != null)
        //_visitorsView.Update();
    }
    public void OnMouseRightButtonDown(TrackLocation trackLocation)
    {
      CanOpenSettings = (trackLocation != null);
      SelectedTrackLocation = trackLocation;
    }

    public void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      if (SelectedTrackLocation != null)
      {
        TrackTabControlView.Update(SelectedTrackLocation);
        IsDeleteButtonEnabled = true;
      }
      else
        IsDeleteButtonEnabled = false;
    }

    public void OpenTabAddNewLocation()
    {
      _selector.ShowContent( ShowableContentControl.FlyoutControlContent
                           , ViewModelsID.LocationSettings
                           , new object[] { null });
    }

    public void ShowLocationFlayout()
    {
      if (SelectedTrackLocation != null)
        return;

      long id = SelectedTrackLocation.LocationID;
      Location location = _bioEngine.Database().LocationHolder.GetValue(id);

      _selector.ShowContent(ShowableContentControl.FlyoutControlContent
                           , ViewModelsID.LocationSettings
                           , new object[] { location });
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
    public ObservableCollection<TrackLocation> TrackControlItems
    {
      get { return _bioEngine.TrackLocationEngine().TrackLocations; }
    }

    private TrackLocation _selectedTrackLocation;
    public TrackLocation SelectedTrackLocation
    {
      get { return _selectedTrackLocation; }
      set
      {
        if (_selectedTrackLocation == value)
          return;
        _selectedTrackLocation = value;
        TrackTabControlView.Update(_selectedTrackLocation);
        NotifyOfPropertyChange(() => SelectedTrackLocation);
      }
    }

    private readonly VisitorsViewModel _visitorsView;
    public VisitorsViewModel VisitorsView
    {
      get { return _visitorsView; }
    }

    private TrackTabControlViewModel _trackTabControlView;
    public TrackTabControlViewModel TrackTabControlView
    {
      get { return _trackTabControlView; }
      set
      {
        if (_trackTabControlView != value)
        {
          _trackTabControlView = value;
          NotifyOfPropertyChange(() => TrackTabControlView);
        }
      }
    }

    private bool _canOpenSettings;
    public bool CanOpenSettings
    {
      get { return _canOpenSettings; }
      set
      {
        if (_canOpenSettings != value)
        {
          _canOpenSettings = value;
          NotifyOfPropertyChange(() => CanOpenSettings);
        }
      }
    }

    #endregion

    #region Global Vatiables
    private readonly IProcessorLocator    _locator      ;    
    private readonly IBioEngine           _bioEngine    ;
    private readonly ViewModelSelector    _selector     ;
    private readonly FastMethodInvoker    _methodInvoker;
    private readonly IWindowManager       _windowManager;
    private readonly IBioSkyNetRepository _database     ;
    private readonly IServiceManager      _bioService   ;


    #endregion

   
    private string _selectedItems;
    public string SelectedItems
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
  }   
}
