namespace BioContracts.Locations
{
  public delegate void LocationsChangedEventHandler();
  public delegate void LocationsStateChangedEventHandler(bool state);

  public interface ITrackLocationEngine
  {
    event LocationsChangedEventHandler LocationsChanged;
    event LocationsStateChangedEventHandler LocationsStateChanged;

    AsyncObservableCollection<TrackLocation> TrackLocations { get; }
  }
}
