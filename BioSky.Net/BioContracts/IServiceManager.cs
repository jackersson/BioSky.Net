using BioContracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IServiceManager
  {
    void Start(string server_address);
    void Stop();

    IFaceService FaceService
    {
      get;
    }

    IDatabaseService DatabaseService
    {
      get;
    }
  }
}
