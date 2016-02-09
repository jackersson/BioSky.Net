using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioService;

namespace BioContracts.Holders
{
  public delegate void FullPhotoRequest(PhotoList list);
  public interface IPhotoHolder 
  {
    event FullPhotoRequest FullPhotoRequested;

    //IList<Photo> GetPersonPhoto(long id);

    
  }
}
