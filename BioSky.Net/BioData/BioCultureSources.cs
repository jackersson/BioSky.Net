using BioContracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BioData
{
  public class BioCultureSources : IBioCultureSources
  {
    public BioCultureSources(){
      CountriesNames = GetCountryNames();
    }

    private List<string> _genderSources;
    public List<string> GenderSources
    {
      get
      {
        if(_genderSources == null)
          return _genderSources = Enum.GetNames(typeof(BioService.Person.Types.Gender)).ToList();
        return _genderSources;
      }
    }

    private List<string> _rightsSources;
    public List<string> RightsSources
    {
      get
      {
        if (_rightsSources == null)
          return _rightsSources = Enum.GetNames(typeof(BioService.Person.Types.Rights)).ToList();
        return _rightsSources;
      }
    }

    private string[] GetCountryNames()
    {
      Dictionary<string, string> CountryNameDictonary = new Dictionary<string, string>();

      foreach (System.Globalization.CultureInfo ci in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures))
      {
        System.Globalization.RegionInfo ri = new System.Globalization.RegionInfo(ci.Name);
        if (!CountryNameDictonary.ContainsKey(ri.NativeName))        
          CountryNameDictonary.Add(ri.NativeName, ri.TwoLetterISORegionName);        
      }

      var OrderedNames = CountryNameDictonary.OrderBy(p => p.Key);

      Dictionary<string, string> Countries = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> val in OrderedNames)      
        Countries.Add(val.Key, val.Value);      

      return Countries.Keys.ToArray();
    }

    private string[] _countriesNames;
    public string[] CountriesNames
    {
      get { return _countriesNames; }
      private set
      {
        if (_countriesNames != value)        
          _countriesNames = value;        
      }
    }

  }
}
