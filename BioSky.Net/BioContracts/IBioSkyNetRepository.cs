using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using Google.Protobuf;

using BioContracts.Holders;

namespace BioContracts
{  
  public interface IBioSkyNetRepository
  {
    ILocalStorage LocalStorage  { get; }

    IFullHolder<Location> Locations  { get; }

    IFullPersonHolder Persons { get; }

    IFullHolder<Visitor> Visitors { get; }


  }
}
