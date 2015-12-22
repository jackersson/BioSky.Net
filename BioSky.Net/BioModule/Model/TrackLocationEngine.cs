using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using BioData;
using System.Collections.ObjectModel;

namespace BioModule.Model
{
  public class TrackLocationEngine : ITrackLocationEngine
  {

    public TrackLocationEngine( IBioSkyNetRepository database, IAccessDeviceEngine accessDeviceEngine )
    {
      _database           = database          ;
      _accessDeviceEngine = accessDeviceEngine;

      _trackLocations = new ObservableCollection<TrackLocation>();

      Init();
    }

    public void Init()
    {      
      foreach (Location location in _database.GetAllLocations())
      {
        TrackLocation trackLocation = new TrackLocation(_accessDeviceEngine, location);
        _trackLocations.Add(trackLocation);
      }     
    }  

    public ObservableCollection<TrackLocation> TrackLocations()
    {
      return _trackLocations;
    }

    private ObservableCollection<TrackLocation> _trackLocations;   

    private readonly IBioSkyNetRepository _database;
    private readonly IAccessDeviceEngine  _accessDeviceEngine;
  }
}
