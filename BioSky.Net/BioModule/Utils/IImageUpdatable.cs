using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioService;

namespace BioModule.Utils
{
  public interface IImageUpdatable
  {

    //TODO not here - as separate element
    void ShowProgress(int progress, bool status);

    void UpdateImage(ref Bitmap img);

    void UpdateImage(Photo photo, string path);

    Photo UploadPhoto();

    void Clear();
  }
}
