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
  public class PageControllerViewModel : Screen
  {
    public string _startIndex   = "0";
    public string _endIndex = "0";
    public string _onePageCount = "0";
    public PageControllerViewModel()
    {
      //Text = _startIndex + "-" + _onePageCount + "of " + _endIndex;
      IsRightArrowEnabled = true;
    }

    public void UpdateData(int startIndex, int endIndex, int onePageCount)
    {
      Text = startIndex + " - " + onePageCount + " of " + endIndex;
    }

    public void OnRightClick()
    {
      OnPageChanged(true);
    }

    public void OnLeftClick()
    {
      OnPageChanged(false);
    }

    public delegate void OnPageChangedHandler(bool pageChangeOnRight);
    public event OnPageChangedHandler PageChanged;

    public void OnPageChanged(bool pageChangeOnRight)
    {
      if (PageChanged != null)
        PageChanged(pageChangeOnRight);
    }

    private string _text;
    public string Text
    {
      get { return _text; }
      set
      {
        if(value == null)
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
 
  }
}
