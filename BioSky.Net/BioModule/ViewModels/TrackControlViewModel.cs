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

namespace BioModule.ViewModels
{
  public class TrackControlViewModel : Conductor<IScreen>.Collection.AllActive
  {
    public TrackControlViewModel(IProcessorLocator locator)
    {
      _locator       = locator      ;
     
      _bioEngine  = locator.GetProcessor<IBioEngine>();
      _selector   = locator.GetProcessor<ViewModelSelector>();
      _database   = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioService = _locator.GetProcessor<IServiceManager>().DatabaseService;
      _notifier   = _locator.GetProcessor<INotifier>();
      
      TrackTabControlView = new TrackTabControlViewModel(_locator);

      _visitorsView = new VisitorsViewModel(locator);

      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Tracking_");

      _bioEngine.TrackLocationEngine().LocationsChanged += OnLocationsChanged;
    }

    #region Update
    public void OnLocationsChanged()
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

    #region Interface

    public async void OnDeleteLocation()
    {
      var result = false; // _windowManager.ShowDialog(DialogsHolder.AreYouSureDialog);

      if (!result)
        return;
     
      try
      {
        await _bioService.LocationDataClient.Delete(SelectedTrackLocation.CurrentLocation);
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }
      
    }

    protected override void OnActivate()
    {
      if (_visitorsView != null)
        ActivateItem(_visitorsView);
      base.OnActivate();    
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

    public void ShowLocationFlyout()
    {
      if (SelectedTrackLocation != null)
        return;

      _selector.ShowContent(ShowableContentControl.FlyoutControlContent
                           , ViewModelsID.LocationSettings
                           , new object[] { SelectedTrackLocation.CurrentLocation });
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
    public AsyncObservableCollection<TrackLocation> TrackControlItems
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
      
    private readonly IBioSkyNetRepository _database     ;
    private readonly IDatabaseService     _bioService   ;
    private readonly INotifier            _notifier     ;

    #endregion

  }   
}
