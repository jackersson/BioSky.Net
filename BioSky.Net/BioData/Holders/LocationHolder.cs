using BioData.Holders.Base;
using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders
{
  public class LocationHolder : HolderBase<Location, long>
  {
    public LocationHolder() : base() { }

    protected override void UpdateDataSet(IList<Location> list)
    {
      foreach (Location location in list)             
        AddToDataSet(location, location.Id);      
    }

  }
}
