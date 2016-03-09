using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.Utils
{
  public class BioCultureSources
  {
    public List<string> GenderSources
    {
      get { return Enum.GetNames(typeof(BioService.Person.Types.Gender)).ToList(); }
    }
    public List<string> RightsSources
    {
      get { return Enum.GetNames(typeof(BioService.Person.Types.Rights)).ToList(); }
    }

    public string[] GetCountryNames()
    {
      Dictionary<string, string> CountryNameDictonary = new Dictionary<string, string>();

      foreach (System.Globalization.CultureInfo ci in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures))
      {
        System.Globalization.RegionInfo ri = new System.Globalization.RegionInfo(ci.Name);
        if (!CountryNameDictonary.ContainsKey(ri.NativeName))
        {
          CountryNameDictonary.Add(ri.NativeName, ri.TwoLetterISORegionName);
        }
      }

      var OrderedNames = CountryNameDictonary.OrderBy(p => p.Key);

      Dictionary<string, string> Countries = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> val in OrderedNames)
      {
        Countries.Add(val.Key, val.Value);
      }

      return Countries.Keys.ToArray();
    }

  }
}
