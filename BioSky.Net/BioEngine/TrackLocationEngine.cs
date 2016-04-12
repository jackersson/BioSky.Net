using System.Collections.Generic;
using BioContracts;
using BioService;
using BioContracts.Locations;
using System.Collections.Concurrent;

namespace BioEngine
{
  public class TrackLocationEngine : ITrackLocationEngine
  { 
    public TrackLocationEngine(  IProcessorLocator locator )      
    {     
      _locator = locator;
      _trackLocationsSet = new ConcurrentDictionary<long, TrackLocation>();
      _trackLocations    = new AsyncObservableCollection<TrackLocation>();   
    
      _locator.GetProcessor<IBioSkyNetRepository>().Locations.DataChanged += RefreshData;        
    }
           
    private void RefreshData()
    {      
      IBioSkyNetRepository database            = _locator.GetProcessor<IBioSkyNetRepository>();
      AsyncObservableCollection<Location> data = database.Locations.Data;
      
      foreach (Location location in data)
      {
        TrackLocation currentLocation = null;
        if (_trackLocationsSet.TryGetValue(location.Id, out currentLocation))
        {
          currentLocation.Update(location);
          continue;
        }
          
         TrackLocation trackLocation = new TrackLocation(_locator, location);
         _trackLocationsSet.TryAdd(location.Id, trackLocation);       
      }
      
      _trackLocations.Clear();
      Dictionary<long, Location> dict = database.Locations.DataSet;
      foreach ( long locationID in _trackLocationsSet.Keys)
      {
        if (!dict.ContainsKey(locationID))
        {
          _trackLocationsSet[locationID].Stop();
          TrackLocation removed = null;
          _trackLocationsSet.TryRemove(locationID, out removed);
        }
        else        
          _trackLocations.Add(_trackLocationsSet[locationID]);        
      }
            
      OnLocationsChanged();
    }

    private ConcurrentDictionary<long, TrackLocation> _trackLocationsSet;
    private ConcurrentDictionary<long, TrackLocation> TrackLocationsSet
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

    public event     LocationsChangedEventHandler LocationsChanged;   
    private readonly IProcessorLocator _locator;  
  }
}
