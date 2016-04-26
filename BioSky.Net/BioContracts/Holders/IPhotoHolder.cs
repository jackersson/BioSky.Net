using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Holders
{

  public delegate void RequestPhotoEventHandler(QueryPhoto query);
  public interface IPhotoHolder
  {
    void UpdateFromResponse(Photo requested, Photo responded);
    void UpdateFromResponse(IList<Photo> requested, IList<Photo> responded);

    void UpdateFromQuery(QueryPhoto query, IList<Photo> responded);

    Photo GetValue(long Id);

    event RequestPhotoEventHandler RequestPhoto;
    event DataChangedHandler       DataChanged ;
  }
}
