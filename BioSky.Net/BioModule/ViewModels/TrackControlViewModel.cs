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
using BioContracts.Services;
using System.Windows.Threading;

namespace BioModule.ViewModels
{
  public class SimpleScreenController : Conductor<IScreen>.Collection.OneActive
  {

  }

  public delegate void LocationEventHandler(TrackLocation location);
  public class TrackItemsShortViewModel : Conductor<IScreen>.Collection.OneActive
  {
    public event LocationEventHandler SelectedLocationChanged;
    public TrackItemsShortViewModel(IProcessorLocator locator)
    {
      _bioEngine = locator.GetProcessor<IBioEngine>();
      _selector = locator.GetProcessor<ViewModelSelector>();
    }

    private void OnSelectedLocationChanged(TrackLocation location)
    {
      if (SelectedLocationChanged != null)
        SelectedLocationChanged(location);
    }

    public void SelectDefault()
    {
      if (SelectedTrackLocation == null)
        SelectedTrackLocation = TrackControlItems.FirstOrDefault();
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
        _selectedTrackLocation.ScreenViewModel.Activate();
        OnSelectedLocationChanged(_selectedTrackLocation);
        //FullTrackTabContro.Update(_selectedTrackLocation);
        NotifyOfPropertyChange(() => SelectedTrackLocation);
      }
    }

    public void OnMouseRightButtonDown(TrackLocation trackLocation)
    {
      CanOpenSettings = (trackLocation != null);
      SelectedTrackLocation = trackLocation;
    }

    public void OnSelectionChanged(SelectionChangedEventArgs e)  {
      IsDeleteButtonEnabled = SelectedTrackLocation != null ? true : false;    
    }

    public void OpenTabAddNewLocation()
    {
      _selector.ShowContent(ShowableContentControl.FlyoutControlContent
                           , ViewModelsID.LocationSettings
                           , new object[] { null });
    }

    public void ShowLocationFlyout()
    {
      if (SelectedTrackLocation == null)
        return;

      _selector.ShowContent(ShowableContentControl.FlyoutControlContent
                           , ViewModelsID.LocationSettings
                           , new object[] { SelectedTrackLocation.CurrentLocation });
    }

    public ObservableCollection<TrackLocation> TrackControlItems {
      get { return _bioEngine.TrackLocationEngine().TrackLocations; }
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

    private readonly ViewModelSelector _selector ;
    private readonly IBioEngine        _bioEngine;
  } 

  public class TrackControlViewModel : Conductor<IScreen>.Collection.AllActive
  {
    public TrackControlViewModel(IProcessorLocator locator)
    {
      _locator       = locator      ;
     
      _bioEngine     = _locator.GetProcessor<IBioEngine>();
      _selector      = _locator.GetProcessor<ViewModelSelector>();
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioService    = _locator.GetProcessor<IServiceManager>().DatabaseService;
      _notifier      = _locator.GetProcessor<INotifier>();
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();

     

      TrackItemsShort = new TrackItemsShortViewModel(locator);
      TrackTabControl = new TrackTabControlViewModel(locator);
      

      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Tracking_");

      _bioEngine.TrackLocationEngine().LocationsChanged += RefreshData;
      _uiDispatcher = _locator.GetProcessor<Dispatcher>();
    }

    #region Update
    public void RefreshData()
    {
      if (!IsActive)
        return;

      //Console.WriteLine(_uiDispatcher.GetHashCode());
     // Console.WriteLine(Dispatcher.CurrentDispatcher.GetHashCode());

       NotifyOfPropertyChange(() => AnyLocationExists);

       if (!AnyLocationExists)
          return;
      //TrackTabControl.Update(null);
    //  return;
      ObservableCollection<TrackLocation> locations = TrackItemsShort.TrackControlItems; 
      foreach (TrackLocation location in locations)
      {
        if (location.ScreenViewModel == null)
          location.ScreenViewModel = new TrackControlItemViewModel(_locator, location);
      }

      TrackItemsShort.SelectDefault();
      NotifyOfPropertyChange(() => TrackItemsShort);
    }

 

    #endregion

    #region Interface

    public void OnAddNewLocation()
    {
      _selector.ShowContent( ShowableContentControl.FlyoutControlContent
                           , ViewModelsID.LocationSettings
                           , new object[] { null });
    }

    protected override void OnActivate()
    {
      TrackItemsShort.SelectedLocationChanged += TrackItemsShort_SelectedLocationChanged;
      base.OnActivate();
      ShowLocations();
      RefreshData();
    }

    protected override void OnDeactivate(bool close)
    {
      TrackItemsShort.SelectedLocationChanged -= TrackItemsShort_SelectedLocationChanged;
      base.OnDeactivate(close);
    }

    private void TrackItemsShort_SelectedLocationChanged(TrackLocation location)
    {
      TrackTabControl.Update(location);
      //VisitorsView update by location
    }




    #endregion

    #region UI

    public bool CanShowLocations {
      get { return !TrackItemsShort.IsActive; }
    }

    public bool CanShowVisitors {
      get { return !VisitorsView.IsActive; }
    }

    private void RefreshUI()
    {
      NotifyOfPropertyChange(() => CanShowLocations);
      NotifyOfPropertyChange(() => CanShowVisitors );
    }

    public void ShowLocations()
    {
      Object = TrackItemsShort;
      RefreshUI();
    }

    public void ShowVisitors()
    {
      Object = VisitorsView;
      ActivateItem(VisitorsView);
      RefreshUI();
    }

    private object _object;
    public object Object
    {
      get { return _object; }
      set
      {
        if( _object != value)
        {
          _object = value;
          NotifyOfPropertyChange(() => Object);
        }
      }
    } 
    
    public VisitorsViewModel VisitorsView {
      get { return TrackTabControl.VisitorsView; }
    }
    

    private TrackTabControlViewModel _trackTabControl;
    public TrackTabControlViewModel TrackTabControl
    {
      get { return _trackTabControl; }
      set
      {
        if (_trackTabControl != value)
        {
          _trackTabControl = value;
          NotifyOfPropertyChange(() => TrackTabControl);
          NotifyOfPropertyChange(() => VisitorsView   );
        }
      }
    }

    private TrackItemsShortViewModel _trackItemsShort;
    public TrackItemsShortViewModel TrackItemsShort
    {
      get { return _trackItemsShort; }
      private set
      {
        if ( _trackItemsShort != value )
        {
          _trackItemsShort = value;
          NotifyOfPropertyChange(() => TrackItemsShort);
        }
      }
    }
        
    public bool AnyLocationExists {
      get { return !(TrackItemsShort.TrackControlItems == null || TrackItemsShort.TrackControlItems.Count < 1) ; }     
    }
  
    #endregion

    #region Global Vatiables

    private readonly IProcessorLocator    _locator      ;    
    private readonly IBioEngine           _bioEngine    ;
    private readonly ViewModelSelector    _selector     ;
      
    private readonly IBioSkyNetRepository _database     ;
    private readonly IDatabaseService     _bioService   ;
    private readonly INotifier            _notifier     ;
    private readonly DialogsHolder        _dialogsHolder;
    private readonly System.Windows.Threading.Dispatcher _uiDispatcher;

    #endregion

  }
}
