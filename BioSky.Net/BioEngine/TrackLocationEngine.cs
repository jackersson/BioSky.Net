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
           
      _locator.GetProcessor<IBioSkyNetRepository>().LocationChanged += LocationViewModel_DataChanged;
        
    }

    public void LocationViewModel_DataChanged(object sender, EventArgs args)
    {
      Init();
    }

    public void Init()
    {
      IBioSkyNetRepository database           = _locator.GetProcessor<IBioSkyNetRepository>();     

      foreach (Location location in database.Locations.Locations)
      {
        if (_trackLocations.Where(x => x.LocationID == location.Id ).FirstOrDefault() != null)        
          continue;        
          
         TrackLocation trackLocation = new TrackLocation(_locator, location);
         trackLocation.Start();
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
