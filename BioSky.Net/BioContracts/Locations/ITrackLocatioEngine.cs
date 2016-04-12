namespace BioContracts.Locations
{
  public delegate void LocationsChangedEventHandler();

  public interface ITrackLocationEngine
  {
    event LocationsChangedEventHandler LocationsChanged;

    AsyncObservableCollection<TrackLocation> TrackLocations { get; }
  }
}
