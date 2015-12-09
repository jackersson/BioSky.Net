using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;

namespace BioData
{
  class BioDataImpl : IBioModule
  {
    private readonly IEntityFrameworkConnectionBuilder _shell;
    private readonly IBioSkyNetRepository _repo ;

    public BioDataImpl( IEntityFrameworkConnectionBuilder shell
                      , IBioSkyNetRepository repo)
    {
      _repo = repo;
      _shell = shell;
    }

    public void Init()
    {
    
    }
  }
}
