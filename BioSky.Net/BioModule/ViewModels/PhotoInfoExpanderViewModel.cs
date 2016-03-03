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
      string width = "256";
      string height = "512";

      Age        = "34567";
      Size       = "348"  ;
      FacesCount = "2"    ;
      Confidence = "77%"  ;
      WidthHeight = width + "*" + height;
    }

    public void OnExpanded(bool isExpanded)
    {
      OnExpanderChanged(isExpanded);
    }

    public delegate void ExpanderChangedEventHandler(bool isExpanded);
    public event ExpanderChangedEventHandler ExpanderChanged;

    protected virtual void OnExpanderChanged(bool isExpanded)
    {
      if (ExpanderChanged != null)
        ExpanderChanged(isExpanded);
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

    private string _widthHeight;
    public string WidthHeight
    {
      get { return _widthHeight; }
      set
      {
        if (_widthHeight != value)
        {
          _widthHeight = value;
          NotifyOfPropertyChange(() => WidthHeight);
        }
      }
    }

    private string _facesCount;
    public string FacesCount
    {
      get { return _facesCount; }
      set
      {
        if (_facesCount != value)
        {
          _facesCount = value;
          NotifyOfPropertyChange(() => FacesCount);
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
