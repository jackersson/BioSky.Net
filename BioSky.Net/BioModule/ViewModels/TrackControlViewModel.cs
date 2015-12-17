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
    public TrackControlViewModel(IBioEngine bioEngine)
    {
      _bioEngine = bioEngine;

      foreach (TrackLocation location in _bioEngine.TrackLocationEngine().TrackLocations())
        location.ScreenViewModel = new TrackControlItemViewModel(location);
    }

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
       
      }
    }
    
    public string Caption()
    {
      return "Tracking";
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
