using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IBioCultureSources
  {
    List<string> GenderSources { get; }

    List<string> RightsSources { get; }

    string[] CountriesNames { get; }

  }
}
