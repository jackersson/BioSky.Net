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
    void UpdateImage(ref Bitmap img);

    void UpdateImage(Photo photo, string path);

    Photo UploadPhotoFromFile();

    void Clear();
  }
}
