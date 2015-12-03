using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;
using System.Windows;

namespace BioModule.ViewModels
{
  public class TrackControlItemViewModel : PropertyChangedBase
  {   
    public BitmapSource OkIconSource
    {
      get { return ResourceLoader.OkIconSource; }
    }

    public BitmapSource VerificationIconSource
    {
      get { return ResourceLoader.VerificationIconSource; }
    }

    public BitmapSource CardIconSource
    {
      get { return ResourceLoader.CardIconSource; }
    }
  }
}
