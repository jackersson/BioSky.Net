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
  public class UserPageViewModel : PropertyChangedBase
  {
    public BitmapSource UserDefaultImageIconSource
    {
      get { return ResourceLoader.UserDefaultImageIconSource; }
    }
    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }
    public BitmapSource OkIconSource
    {
      get { return ResourceLoader.OkIconSource; }
    }

    public BitmapSource CancelIconSource
    {
      get { return ResourceLoader.CancelIconSource; }
    }

    public BitmapSource UserInformationIconSource
    {
      get { return ResourceLoader.UserInformationIconSource; }
    }
    public BitmapSource UserFacesIconSource
    {
      get { return ResourceLoader.UserFacesIconSource; }
    }
    public BitmapSource UserFingerprintIconSource
    {
      get { return ResourceLoader.UserFingerprintIconSource; }
    }
    public BitmapSource UserIricesIconSource
    {
      get { return ResourceLoader.UserIricesIconSource; }
    }
    public BitmapSource UserContactlessCardsIconSource
    {
      get { return ResourceLoader.UserContactlessCardsIconSource; }
    }

  }
}
