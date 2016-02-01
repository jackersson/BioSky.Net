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

  }
}
