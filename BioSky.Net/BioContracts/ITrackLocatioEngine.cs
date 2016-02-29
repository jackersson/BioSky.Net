using System.Collections.Generic;

namespace BioContracts
{
  public delegate void LocationsChangedEventHandler();

  public interface ITrackLocationEngine
  {
    event LocationsChangedEventHandler LocationsChanged;

    AsyncObservableCollection<TrackLocation> TrackLocations { get; }
  }
}
