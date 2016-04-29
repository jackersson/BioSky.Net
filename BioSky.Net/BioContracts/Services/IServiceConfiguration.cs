using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public interface IServiceConfiguration
  {
    string FacialService    { get; set; }

    string DatabaseService  { get;  set; }

    string FingerprintService { get; set; }
  }
}
