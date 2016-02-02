﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using BioModule.ResourcesLoader;
//using BioModule.Model;

using System.Windows;
using System.Windows.Media.Imaging;

using System.Windows.Data;
using BioModule.Utils;
using System.Windows.Controls;
using Google.Protobuf.Collections;
using System.Reflection;
using BioFaceService;

namespace BioModule.ViewModels
{
  public class TrackControlViewModel : Screen
  {
    public TrackControlViewModel( IProcessorLocator locator)
    {
      _locator = locator;

      _bioEngine = locator.GetProcessor<IBioEngine>();
      _selector  = locator.GetProcessor<ViewModelSelector>();

      _methodInvoker = new FastMethodInvoker();

      TrackTabControlView = new TrackTabControlViewModel(_locator);

      _visitorsView = new VisitorsViewModel(locator);


      DisplayName = "Tracking";

      _bioEngine.TrackLocationEngine().TrackLocations.CollectionChanged += TrackLocations_CollectionChanged;
    }

    protected override void OnActivate()
    {
      _visitorsView.Update();      
    }
    public void TrackLocations_CollectionChanged(object sender, EventArgs args)
    {
      foreach (TrackLocation location in TrackControlItems)
      {
        if (location.ScreenViewModel == null )
          location.ScreenViewModel = new TrackControlItemViewModel(_locator, location);         
      }

      if (SelectedTrackLocation == null)      
        TrackTabControlView.Update(TrackControlItems[0]);

      TrackTabControlView.Update(SelectedTrackLocation);
    }

    public ObservableCollection<TrackLocation> TrackControlItems
    {
      get {  return _bioEngine.TrackLocationEngine().TrackLocations;  }      
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

    public void OnMouseRightButtonDown(TrackLocation trackLocation)
    {
      CanOpenSettings = (trackLocation != null);
      SelectedTrackLocation = trackLocation;
    }

    public void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      if (SelectedTrackLocation == null)
        return;

      TrackTabControlView.Update(SelectedTrackLocation);
    }

    public void ShowLocationFlayout()
    {
      long id = SelectedTrackLocation.LocationID;
      Location location = null;
      bool visitorFound = _bioEngine.Database().LocationHolder.DataSet.TryGetValue(id, out location);

      _selector.ShowContent( ShowableContentControl.FlyoutControlContent
                           , ViewModelsID.LocationSettings
                           , new object[] { location });
    }    

    private readonly IProcessorLocator _locator      ;    
    private readonly IBioEngine        _bioEngine    ;
    private readonly ViewModelSelector _selector     ;
    private readonly FastMethodInvoker _methodInvoker;

    //******************************************ComboBoxLocationCheck**************************

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

    /*
    public void OnChecked()
    {
      string caption = "";

      int count = 0;
      foreach (TrackLocation pair in _bioEngine.TrackLocationEngine().TrackLocations())
      {
        caption += pair.IsChecked ? (pair.Caption + ";") : "";
        if (pair.IsChecked)
          ++count;
      }

      if (count >= _bioEngine.TrackLocationEngine().TrackLocations().Count)
        caption = "All";

      if (count == 0)
        caption = "None";

      SelectedItems = caption;
    }
    */
  }   
}
