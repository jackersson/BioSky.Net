using BioService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.Utils
{
  public enum BioImageModelType
  {
     Faces
   , Irises
   , Fingers
   , Photo
  }

  public interface IBioImageModel
  {
    void Activate();
    void Deactivate();
    void UpdateController(IUserBioItemsController controller);
    void UploadPhoto(Photo photo);
    void UpdateFrame(Bitmap frame);
    void ShowDetails(bool state);
    bool IsActive { get;}
    BioImageModelType BioType { get; }
    IUserBioItemsController Controller { get; }
  }

}
