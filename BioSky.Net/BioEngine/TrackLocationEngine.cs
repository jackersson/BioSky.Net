using System.Collections.Generic;

using BioContracts;
using BioService;

namespace BioEngine
{
  public class TrackLocationEngine : ITrackLocationEngine
  {
    public TrackLocationEngine(  IProcessorLocator locator )      
    {     
      _locator = locator;
      _trackLocationsSet = new Dictionary<long, TrackLocation>();
      _trackLocations    = new AsyncObservableCollection<TrackLocation>();

      _locator.GetProcessor<IBioSkyNetRepository>().Locations.DataChanged += RefreshData;        
    }  

    public void RefreshData()
    {
      IBioSkyNetRepository database            = _locator.GetProcessor<IBioSkyNetRepository>();
      AsyncObservableCollection<Location> data = database.LocationHolder.Data;
      foreach (Location location in data)
      {
        TrackLocation currentLocation = null;
        if (_trackLocationsSet.TryGetValue(location.Id, out currentLocation))
        {
          currentLocation.Update(location);
          continue;
        }
          
         TrackLocation trackLocation = new TrackLocation(_locator, location);
        _trackLocationsSet.Add(location.Id, trackLocation);
      }

      _trackLocations.Clear();
      Dictionary<long, Location> dict = database.LocationHolder.DataSet;
      foreach ( long locationID in _trackLocationsSet.Keys)
      {
        if (!dict.ContainsKey(locationID))
        {
          _trackLocationsSet[locationID].Stop();
          _trackLocationsSet.Remove(locationID);
        }
        else        
          _trackLocations.Add(_trackLocationsSet[locationID]);        
      }

      OnLocationsChanged();
    }

    private Dictionary<long, TrackLocation> _trackLocationsSet;
    private Dictionary<long, TrackLocation> TrackLocationsSet
    {
     get { return _trackLocationsSet; }
    }

    private AsyncObservableCollection<TrackLocation> _trackLocations;
    public AsyncObservableCollection<TrackLocation> TrackLocations
    {
      get { return _trackLocations; }
    }

    private void OnLocationsChanged()
    {
      if (LocationsChanged != null)
        LocationsChanged();
    }

    public event LocationsChangedEventHandler LocationsChanged;
    private readonly IProcessorLocator _locator;  
  }
}
