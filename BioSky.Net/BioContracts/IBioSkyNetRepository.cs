using BioContracts.Common;
using BioContracts.Holders;
using BioService;

namespace BioContracts
{
  public interface IBioSkyNetRepository
  {
    ILocalStorage LocalStorage  { get; }

    IFullLocationHolder Locations  { get; }

    IFullPersonHolder Persons { get; }

    IFullHolder<Visitor> Visitors { get; }
    IBioCultureSources BioCultureSources { get; }
  }
}
