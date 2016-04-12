using BioContracts;
using BioService;
using Caliburn.Micro;
using BioData.Holders.Grouped;
using BioData.Holders.Utils;
using BioData.Holders;
using BioContracts.Holders;
using BioContracts.Common;

namespace BioData
{
  public class BioSkyNetRepository : PropertyChangedBase, IBioSkyNetRepository
  {
    public BioSkyNetRepository(IProcessorLocator locator)
    {
      _localStorage = new BioLocalStorage();
      _ioUtils      = new IOUtils(_localStorage);

      _photoHolder       = new PhotoHolder(_ioUtils);
      _fullLocations     = new FullLocationHolder(locator);
      _fullPersons       = new FullPersonHolder  (locator, _ioUtils, _photoHolder);
      _fullVisitors      = new FullVisitorHolder (_photoHolder);
      _bioCultureSources = new BioCultureSources ();
    }

    private FullLocationHolder _fullLocations;
    public IFullLocationHolder Locations {
      get { return _fullLocations; }
    }

    private FullPersonHolder _fullPersons;
    public IFullPersonHolder Persons {
      get { return _fullPersons; }
    }

    private FullVisitorHolder _fullVisitors;
    public IFullHolder<Visitor> Visitors {
      get { return _fullVisitors; }
    }

    private ILocalStorage _localStorage;
    public ILocalStorage LocalStorage  {
      get { return _localStorage;  }
    }

    private IBioCultureSources _bioCultureSources;
    public IBioCultureSources BioCultureSources {
      get { return _bioCultureSources; }
    }

    private PhotoHolder _photoHolder;   
    private IOUtils     _ioUtils    ;
  }
}
