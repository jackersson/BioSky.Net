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

    /*
    public override void Update(IList<Location> list, Result result)
    {
      foreach (ResultPair currentResult in result.Status)
      {
        Location location = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
            location = currentResult.Location;
          else
            location = list.Where(x => x.Id == currentResult.Id).FirstOrDefault();

          if (location != null)
          {
            location.Dbstate = DbState.None;
            UpdateItem(location, location.Id, currentResult.State);
          }
        }
      }
      base.Update(list, result);
    }
    */
  }
}
