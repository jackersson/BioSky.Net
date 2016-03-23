using BioContracts;

using BioService;
using Caliburn.Micro;
using BioData.Holders.Grouped;
using BioData.Holders.Utils;

namespace BioData
{
  public class BioSkyNetRepository : PropertyChangedBase, IBioSkyNetRepository
  {
    FullLocationHolder _fullLocations;
    FullPersonHolder   _fullPersons  ;
    FullVisitorHolder  _fullVisitors ;

    ILocalStorage       _localStorage;
    IBioCultureSources  _bioCultureSources;


    IOUtils _ioUtils;

    public BioSkyNetRepository()
    {
      _localStorage = new BioLocalStorage();
      _ioUtils = new IOUtils(_localStorage);


      _fullLocations     = new FullLocationHolder();
      _fullPersons       = new FullPersonHolder  (_ioUtils);
      _fullVisitors      = new FullVisitorHolder ();
      _bioCultureSources = new BioCultureSources ();
    }

    public IFullLocationHolder Locations
    {
      get { return _fullLocations; }
    }

    public IFullPersonHolder Persons
    {
      get { return _fullPersons; }
    }

    public IFullHolder<Visitor> Visitors
    {
      get { return _fullVisitors; }
    }
      
    public ILocalStorage LocalStorage
    {
      get { return _localStorage;  }
    }
    public IBioCultureSources BioCultureSources
    {
      get { return _bioCultureSources; }
    }
  }
}
