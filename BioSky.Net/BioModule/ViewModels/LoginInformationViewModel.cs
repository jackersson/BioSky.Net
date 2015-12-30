using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;


namespace BioModule.ViewModels
{
  public class LoginInformationViewModel : PropertyChangedBase
  {
    public LoginInformationViewModel()
    {

    }

    public BitmapSource LoginImageSource
    {
      get { return ResourceLoader.UserDefaultImageIconSource; }
    }
  }
}
