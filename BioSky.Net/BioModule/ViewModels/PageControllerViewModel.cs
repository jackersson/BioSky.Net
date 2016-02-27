using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.Reflection;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class PageControllerViewModel : Screen
  {
    public string _startIndex   = "0";
    public string _endIndex     = "0";
    public string _onePageCount = "0";
    public PageControllerViewModel(IScreen screen)
    {
      _screen = screen;
      IsRightArrowEnabled = true;
      IsLeftArrowEnabled  = true;
    }

    public void UpdateData(PagingData data)
    {
      IsRightArrowEnabled = true;
      IsLeftArrowEnabled  = true;

      Text = data.startIndex + " - " + data.endIndex + " of " + data.count;

      if(data.endIndex == data.count)
        IsRightArrowEnabled = false;

      if(data.startIndex == 1)
        IsLeftArrowEnabled = false;      
    }

    public void OnRightClick()
    {
      MethodInfo method = _screen.GetType().GetMethod("MovePage");
      if (method != null)
        method.Invoke(_screen, new object[] { true});
    }

    public void OnLeftClick()
    {
      MethodInfo method = _screen.GetType().GetMethod("MovePage");
      if (method != null)
        method.Invoke(_screen, new object[] { false });
    }



    private string _text;
    public string Text
    {
      get { return _text; }
      set
      {
        if(value == null || value == string.Empty)
        {
           _text = _startIndex + "-" + _onePageCount + "of " + _endIndex;
        }
        else if (_text != value)
        {
          _text = value;
          NotifyOfPropertyChange(() => Text);
        }
      }
    }

    private bool _isLeftArrowEnabled;
    public bool IsLeftArrowEnabled
    {
      get { return _isLeftArrowEnabled; }
      set
      {
        if (_isLeftArrowEnabled != value)
        {
          _isLeftArrowEnabled = value;
          NotifyOfPropertyChange(() => IsLeftArrowEnabled);
        }
      }
    }

    private bool _isRightArrowEnabled;
    public bool IsRightArrowEnabled
    {
      get { return _isRightArrowEnabled; }
      set
      {
        if (_isRightArrowEnabled != value)
        {
          _isRightArrowEnabled = value;
          NotifyOfPropertyChange(() => IsRightArrowEnabled);
        }
      }
    }

    private readonly IScreen _screen;
 
  }
}
