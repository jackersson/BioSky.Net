using BioData.Holders.Base;
using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders
{
  public class PhotoHolder : HolderBase<Photo, long>
  {
    public PhotoHolder() : base() { }

    protected override void UpdateDataSet(IList<Photo> list)
    {
      foreach (Photo photo in list)
        AddToDataSet(photo, photo.Id);
    }

    public override void Update(IList<Photo> list, Result result)
    {
      foreach (ResultPair currentResult in result.Status)
      {
        Photo photo = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
            photo = currentResult.Photo;
          else
            photo = list.Where(x => x.Id == currentResult.Id).FirstOrDefault();

          if (photo != null)
          {
            photo.Dbstate = DbState.None;
            UpdateItem(photo, photo.Id, currentResult.State);
          }
        }
      }
      base.Update(list, result);
    }

  }
}
