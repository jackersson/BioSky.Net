using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioService;

namespace BioModule.Utils
{
  public interface IPhotoUpdatable
  {
    void UpdateFromPhoto(Photo photo, string filePrefix);
    void Clear();
  }
}
