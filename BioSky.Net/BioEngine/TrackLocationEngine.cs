using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using BioData;
using System.Collections.ObjectModel;
using BioFaceService;

namespace BioEngine
{
  public class TrackLocationEngine : ITrackLocationEngine
  {

    public TrackLocationEngine(  IProcessorLocator locator )      
    {     
      _locator = locator;
      _trackLocations = new ObservableCollection<TrackLocation>();


      //TODO init only when data comes
      Init();
    }

    public void Init()
    {
      IBioSkyNetRepository database           = _locator.GetProcessor<IBioSkyNetRepository>();
      IAccessDeviceEngine  accessDeviceEngine = _locator.GetProcessor<IAccessDeviceEngine> ();

      foreach (Location location in database.Locations.Locations)
      {
        TrackLocation trackLocation = new TrackLocation(_locator, location);
        _trackLocations.Add(trackLocation);
      }     
    }

    private ObservableCollection<TrackLocation> _trackLocations;
    public ObservableCollection<TrackLocation> TrackLocations
    {
     get { return _trackLocations; }
    }

    private readonly IProcessorLocator _locator;
   
  }
}
