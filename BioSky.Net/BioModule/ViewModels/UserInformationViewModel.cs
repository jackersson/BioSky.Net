using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;

using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

namespace BioModule.ViewModels
{
  public class UserInformationViewModel : PropertyChangedBase
  {

    public BitmapSource OkIconSource
    {
      get { return ResourceLoader.OkIconSource; }
    }

    public BitmapSource CancelIconSource
    {
      get { return ResourceLoader.CancelIconSource; }
    }

    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }

    public void Resize()
    {

    }

  }
}
