using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface ILocalStorage
  {   
    string LocalStoragePath { get; }

    string FaceServiceStoragePath { get; }

    string DatabaseServiceStoragePath { get; }

    string Language { get; }

    void SaveGeneralSettings(string local, string face, string service, string language);
  }
}
