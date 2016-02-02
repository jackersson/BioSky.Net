using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioFaceService;

namespace BioContracts.Holders
{
  public interface IPhotoHolder 
  {
    IList<Photo> GetPersonPhoto(long id);
  }
}
