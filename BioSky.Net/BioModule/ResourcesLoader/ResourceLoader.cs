using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;

namespace BioModule.ResourcesLoader
{

  public static class BitmapConversion
  {
    public static BitmapSource BitmapToBitmapSource(Bitmap source)
    {
      return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    source.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
    }
  }
  public static class ResourceLoader
  {
    private static BitmapSource _addIconSource   ;
    private static BitmapSource _removeIconSource;
    private static BitmapSource _deleteIconSource;

    public static BitmapSource AddIconSource
    {
      get
      {
        if (_addIconSource == null)
          _addIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.add);
        return _addIconSource;
      }
    }

    public static BitmapSource RemoveIconSource
    {
      get
      {
        if (_removeIconSource == null)
          _removeIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.remove);
        return _removeIconSource;
      }
    }


    public static BitmapSource DeleteIconSource
    {
      get
      {
        if (_deleteIconSource == null)
          _deleteIconSource = BitmapConversion.BitmapToBitmapSource(BioModule.Properties.Resources.delete);
        return _deleteIconSource;
      }
    }   
  }
}
