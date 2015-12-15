using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using BioModule.ResourcesLoader;
using BioModule.Model;

using System.Windows;
using System.Windows.Media.Imaging;

namespace BioModule.ViewModels
{
  public class TrackControlViewModel : PropertyChangedBase
  {
    //private TrackControl _trackControl;
    //public TrackControl TrackControl
    //{
      //get { return _trackControl; }
   // }
    //
    public ObservableCollection<TrackLocation> TrackControlItems
    {
      get {  return _bioEngine.TrackLocationEngine().TrackLocations();  }      
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
        NotifyOfPropertyChange(() => CurrentTrackLocation);
      }
    }

    public object CurrentTrackLocation
    {
      get { return _bioEngine.TrackLocationEngine().TrackLocations()[0].ScreenViewModel; }
    }

    //public object CurrentTabControl
    //{
     // get;// { return _trackControl.ScreenViewModel; }
   // }



    public TrackControlViewModel( IBioEngine bioEngine )
    {

      _bioEngine = bioEngine;

      foreach (TrackLocation location in _bioEngine.TrackLocationEngine().TrackLocations() )      
        location.ScreenViewModel = new TrackControlItemViewModel(location);      

      //_trackControl = new TrackControl();
      //_trackControl.TrackItems.Add(new TrackItem(_bioEngine, "COM5") { Caption = "MainDoors", ScreenViewModel = new TrackControlItemViewModel() });

      //NotifyOfPropertyChange(() => TrackControlItems);
      //NotifyOfPropertyChange(() => CurrentViewTab);
    }

    private readonly IBioEngine _bioEngine;


    //**************************************** UI *******************************************
    public BitmapSource AddIconSource
    {
      get { return ResourceLoader.AddIconSource; }
    }

    public BitmapSource RemoveIconSource
    {
      get { return ResourceLoader.RemoveIconSource; }
    }

    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }
  }
}
