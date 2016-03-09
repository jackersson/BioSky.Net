using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioModule.Utils
{
  public class BitmapUtils
  {
    public BitmapImage GetImageSource(string fileName)
    {
      if (!File.Exists(fileName))
        return null;

      BitmapImage image = new BitmapImage();

      image.BeginInit();
      image.UriSource = new Uri(fileName);
      image.CacheOption = BitmapCacheOption.OnLoad;
      image.EndInit();

      return image;
    }
  }
}
