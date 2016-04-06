using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioService;
using BioModule.ViewModels;

namespace BioModule.Utils
{
  public interface IUserBioItemsUpdatable
  {
    void UpdateBioItemsController(IUserBioItemsController controller);
    void UpdateFromPhoto(Photo photo);
    void ChangeBioImageModel(BioImageModelEnum state);
    void Clear();

    Photo CurrentPhoto { get; }
  }
}
