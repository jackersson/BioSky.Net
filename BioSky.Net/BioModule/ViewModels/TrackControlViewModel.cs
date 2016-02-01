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
using BioFaceService;

namespace BioModule.ViewModels
{
  public class TrackControlViewModel : Conductor<IScreen>.Collection.OneActive
  {
    public TrackControlViewModel( IProcessorLocator locator)
    {
      _locator = locator;

      _bioEngine = locator.GetProcessor<IBioEngine>();
      _selector  = locator.GetProcessor<ViewModelSelector>();

      _methodInvoker = new FastMethodInvoker();

      _visitorsView = new VisitorsViewModel(locator);

      DisplayName = "Tracking";

      _bioEngine.TrackLocationEngine().TrackLocations.CollectionChanged += TrackLocations_CollectionChanged;


      /*
            foreach (TrackLocation location in TrackControlItems)
            {
              if (location.ScreenViewModel == null)
                location.ScreenViewModel = new TrackControlItemViewModel(_locator, location);
            }*/

      Items.Add(new TrackControlItemViewModel(_locator));
      Items.Add(new VisitorsViewModel(_locator));

      ActiveItem = Items[1];
      OpenTab();

      //OnChecked();
    }



/*
    public void Update(TrackLocation trackLocation)
    {
      if (trackLocation == null)
        return;

      Items.Add(new TrackControlItemViewModel(_locator, trackLocation));
      Items.Add(new VisitorsViewModel(_locator));

      ActiveItem = Items[0];
      OpenTab();
    }*/
    public void OpenTab()
    {
      if ( ActiveItem != null)
        ActiveItem.Activate();
    }

    public void TrackLocations_CollectionChanged(object sender, EventArgs args)
    {
      foreach (TrackLocation location in TrackControlItems)
      {
        if (location.ScreenViewModel == null )
          location.ScreenViewModel = new TrackControlItemViewModel(_locator, location);         
      }
      
      Items[0] = (TrackControlItemViewModel)TrackControlItems[0].ScreenViewModel;
      ActiveItem = Items[0];
      OpenTab();
     // _methodInvoker.InvokeMethod(it.GetType(), "Update", it, new object[] { TrackControlItems[0] });

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
        NotifyOfPropertyChange(() => SelectedTrackLocation);       
      }
    }  
    
    private readonly VisitorsViewModel _visitorsView;
    public VisitorsViewModel VisitorsView
    {
      get { return _visitorsView; }
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

      //object it = Items[0];
     // _methodInvoker.InvokeMethod(it.GetType(), "Update", it, new object[] { SelectedTrackLocation });
    }

    public void ShowLocationFlayout()
    {
      Location location = null;
      _bioEngine.Database().LocationHolder.DataSet.TryGetValue(SelectedTrackLocation.LocationID, out location);
       
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
