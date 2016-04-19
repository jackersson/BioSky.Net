using System.Collections.Generic;
using BioContracts;
using BioService;
using BioContracts.Locations;
using System.Collections.Concurrent;
using BioContracts.Locations.Observers;
using BioContracts.Holders;
using System.Linq;
using BioContracts.CaptureDevices;
using BioContracts.AccessDevices;

namespace BioEngine
{
  public class TrackLocationEngine : ITrackLocationEngine
  {
    public TrackLocationEngine(IProcessorLocator locator)
    {
      _locator = locator;
      _trackLocationsSet = new ConcurrentDictionary<long, TrackLocation>();
      _trackLocations    = new AsyncObservableCollection<TrackLocation>();


      AccessDevices  = new HashSet<string>();
      CaptureDevices = new HashSet<string>();

      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      _accessDeviceEngine  = locator.GetProcessor<IAccessDeviceEngine>();

      _locationsHolder = _locator.GetProcessor<IBioSkyNetRepository>().Locations;
      _locationsHolder.DataChanged += RefreshData;
    }
           
    private void RefreshData()
    {      
      IBioSkyNetRepository database            = _locator.GetProcessor<IBioSkyNetRepository>();
      AsyncObservableCollection<Location> data = database.Locations.Data;
      
      foreach (Location location in data)
      {
        CheckDeviceObservers(location);

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

      RemoveDevices();
      OnLocationsChanged();
    }

    private void RemoveDevices()
    {
      foreach (string deviceName in AccessDevices)
      {
        AccessDevice accessDevice = _locationsHolder.AccessDevices.Where(x => x.Portname == deviceName).FirstOrDefault();
        if (accessDevice == null)
        {
          _accessDeviceEngine.Remove(deviceName);
          AccessDevices.Remove(deviceName);
        }
      }

      foreach (string deviceName in CaptureDevices)
      {
        CaptureDevice captureDevice = _locationsHolder.CaptureDevices.Where(x => x.Devicename == deviceName).FirstOrDefault();
        if (captureDevice == null)
        {
          _captureDeviceEngine.Remove(deviceName);
          CaptureDevices.Remove(deviceName);
        }
      }
    }

    private void CheckDeviceObservers(Location location)
    {
      if (location.AccessDevice != null && location.AccessDevice.Id > 0)
      {
        string deviceName = location.AccessDevice.Portname;
        if (!AccessDevices.Contains(deviceName))
        {
          _accessDeviceEngine.Add(deviceName);
          AccessDevices.Add(deviceName);
        }
      }

      if (location.CaptureDevice != null && location.CaptureDevice.Id > 0)
      {
        string deviceName = location.CaptureDevice.Devicename;
        if (!CaptureDevices.Contains(deviceName))
        {
          _captureDeviceEngine.Add(deviceName);
          CaptureDevices.Add(deviceName);
        }
      }
    }

    private HashSet<string> AccessDevices ;
    private HashSet<string> CaptureDevices;

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
  }
}
