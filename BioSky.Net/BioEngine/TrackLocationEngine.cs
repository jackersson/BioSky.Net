using System.Collections.Generic;
using BioContracts;
using BioService;
using BioContracts.Locations;
using System.Collections.Concurrent;
using BioContracts.Holders;
using System.Linq;
using BioContracts.CaptureDevices;
using BioContracts.AccessDevices;
using BioContracts.FingerprintDevices;

namespace BioEngine
{
  public class TrackLocationEngine : ITrackLocationEngine
  {
    public TrackLocationEngine(IProcessorLocator locator)
    {
      _locator = locator;
      _trackLocationsSet = new ConcurrentDictionary<long, TrackLocation>();
      _trackLocations    = new AsyncObservableCollection<TrackLocation>();

      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      _accessDeviceEngine = locator.GetProcessor<IAccessDeviceEngine>();
      _fingerDeviceEngine = locator.GetProcessor<IFingerprintDeviceEngine>();


      _locationsHolder = _locator.GetProcessor<IBioSkyNetRepository>().Locations;
      _locationsHolder.DataChanged += RefreshData;
    }
           
    private void RefreshData()
    {      
      IBioSkyNetRepository database      = _locator.GetProcessor<IBioSkyNetRepository>();
      IServiceManager     serviceManager = _locator.GetProcessor<IServiceManager>();
      IEnumerable <Location> data        = database.Locations.Data.Where(x => x.MacAddress == serviceManager.MacAddress);


     // UpdateDevicesEngines();

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

    private void UpdateDevicesEngines()
    {
      _captureDeviceEngine.UpdateFromSet(_locationsHolder.CaptureDevicesSet);
      _accessDeviceEngine .UpdateFromSet(_locationsHolder.AccessDevicesSet );
      _fingerDeviceEngine .UpdateFromSet(_locationsHolder.FingerDevicesSet );

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

    public event     LocationsChangedEventHandler LocationsChanged    ;   
    private readonly IProcessorLocator            _locator            ;
    private readonly IFullLocationHolder          _locationsHolder    ;
    private readonly ICaptureDeviceEngine         _captureDeviceEngine;
    private readonly IAccessDeviceEngine          _accessDeviceEngine ;
    private readonly IFingerprintDeviceEngine     _fingerDeviceEngine ;
  }
}
