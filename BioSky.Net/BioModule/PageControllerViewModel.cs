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
    public PageControllerViewModel(int onePageCount = 25, int all = 0)
    {
      Text = "1 -" + onePageCount + "of " + all;
      IsRightArrowEnabled = true;
    }

    public void OnRightClick()
    {

    }

    public void OnLeftClick()
    {

    }

    private string _text;
    public string Text
    {
      get { return _text; }
      set
      {
        if (_text != value)
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
