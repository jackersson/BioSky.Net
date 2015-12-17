using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;
using BioData;

namespace BioModule.ViewModels
{
  public class UserContactlessCardViewModel : PropertyChangedBase
  {

    public void Update(User user)
    {
      _user = user;
    }

    public BitmapSource AddIconSource
    {
      get { return ResourceLoader.AddIconSource; }
    }

    public BitmapSource RemoveIconSource
    {
      get { return ResourceLoader.RemoveIconSource; }
    }

    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }

    public BitmapSource OkIconSource
    {
      get { return ResourceLoader.OkIconSource; }
    }

    private User _user;
  }
}
