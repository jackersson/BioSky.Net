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
  public class PhotoInfoExpanderViewModel : Screen
  {
    public PhotoInfoExpanderViewModel()
    {
      Age        = "34567";
      Gender     = "Male" ;
      Size       = "348"  ;
      Faces      = "2"    ;
      Count      = "3"    ;
      Width      = "256"  ;
      Height     = "512"  ;
      Origin     = "full" ;
      Confidence = "77%"  ;
    }

    private string _confidence;
    public string Confidence
    {
      get { return _confidence; }
      set
      {
        if (_confidence != value)
        {
          _confidence = value;
          NotifyOfPropertyChange(() => Confidence);
        }
      }
    }

    private string _origin;
    public string Origin
    {
      get { return _origin; }
      set
      {
        if (_origin != value)
        {
          _origin = value;
          NotifyOfPropertyChange(() => Origin);
        }
      }
    }

    private string _height;
    public string Height
    {
      get { return _height; }
      set
      {
        if (_height != value)
        {
          _height = value;
          NotifyOfPropertyChange(() => Height);
        }
      }
    }

    private string _width;
    public string Width
    {
      get { return _width; }
      set
      {
        if (_width != value)
        {
          _width = value;
          NotifyOfPropertyChange(() => Width);
        }
      }
    }

    private string _count;
    public string Count
    {
      get { return _count; }
      set
      {
        if (_count != value)
        {
          _count = value;
          NotifyOfPropertyChange(() => Count);
        }
      }
    }

    private string _faces;
    public string Faces
    {
      get { return _faces; }
      set
      {
        if (_faces != value)
        {
          _faces = value;
          NotifyOfPropertyChange(() => Faces);
        }
      }
    }

    private string _age;
    public string Age
    {
      get { return _age; }
      set
      {
        if (_age != value)
        {
          _age = value;
          NotifyOfPropertyChange(() => Age);
        }
      }
    }

    private string _gender;
    public string Gender
    {
      get { return _gender; }
      set
      {
        if (_gender != value)
        {
          _gender = value;
          NotifyOfPropertyChange(() => Gender);
        }
      }
    }

    private string _size;
    public string Size
    {
      get { return _size; }
      set
      {
        if (_size != value)
        {
          _size = value;
          NotifyOfPropertyChange(() => Size);
        }
      }
    }
  }
}
