using BioData.Holders.Base;
using BioService;
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
        Update(location, location.Id);      
    }

    public override void Remove( long key)
    {
      base.Remove(key);
      var item = Data.Where(x => x.Id == key).FirstOrDefault();
      if ( item != null)
      {
        Data.Remove(item);
      }
    }

    protected override void CopyFrom(Location from, Location to)
    {
      to.MergeFrom(from);
    }    

  }
}
