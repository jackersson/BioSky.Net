using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.ViewModels
{
  public class PhotoInformationViewModel : Screen
  {
    public PhotoInformationViewModel()
    {
      
    }

    public void Update(Photo photo)
    {
      CurrentPhoto = photo;
    }

    public float Confidence
    {
      get
      {
        /*
        bool canShow = (_currentPhoto != null
                       && _currentPhoto.PortraitCharacteristic != null);

        FaceCharacteristic fc = canShow ? _currentPhoto.PortraitCharacteristic.Faces.FirstOrDefault() : null; */
        return /*(fc != null && fc.Location != null) ? fc.Location.Confidence : */0.0f;
      }
    }

    public string Resolution
    {
      get
      {
        long width = 0, height = 0;
        if (_currentPhoto != null)
        {
          width = _currentPhoto.Width;
          height = _currentPhoto.Height;
        }

        return string.Format("{0}x{1}", width, height);
      }
    }

    public int FacesCount
    {
      get
      {
       // bool canShow = (_currentPhoto != null && _currentPhoto.PortraitCharacteristic != null);
        return /* canShow ? _currentPhoto.PortraitCharacteristic.FacesCount : */ 0;
      }
    }

    public int Age
    {
      get
      {
      //  bool canShow = (_currentPhoto != null && _currentPhoto.PortraitCharacteristic != null);
        return /*canShow ? _currentPhoto.PortraitCharacteristic.Age : */ 0;
      }
    }

    public string Size
    {
      get { return (_currentPhoto != null) ? _currentPhoto.SizeType.ToString() : ""; }
    }

    public string Origin
    {

      get { return (_currentPhoto != null) ? _currentPhoto.OriginType.ToString() : ""; }
    }

    private Photo _currentPhoto;
    public Photo CurrentPhoto
    {
      get { return _currentPhoto; }
      set
      {
        if (_currentPhoto != value)
        {
          _currentPhoto = value;
          NotifyOfPropertyChange(() => Confidence);
          NotifyOfPropertyChange(() => Age);
          NotifyOfPropertyChange(() => FacesCount);
          NotifyOfPropertyChange(() => Size);
          NotifyOfPropertyChange(() => Origin);
          NotifyOfPropertyChange(() => Resolution);
        }

      }
    }
  }
}
