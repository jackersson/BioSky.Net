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
    public PageControllerViewModel()
    {    
      IsRightArrowEnabled = IsLeftArrowEnabled = false;
      Text = FormatPageText(0, 0, 0);     
    }

    public void UpdateData(IPageController controller)
    {
      if (controller == null)
        return;

      _controller = controller;
      controller.FilterChanged += UpdateMove;
      UpdateMove();
    }

    public void UpdateMove()
    {
      if (_controller == null)
        return;     

      int startIndex = (_controller.ItemsCount == 0) ? 0 : _controller.StartIndex + 1; 

      Text = FormatPageText(  startIndex
                           , _controller.EndIndex, _controller.ItemsCount);

      IsRightArrowEnabled = (_controller.EndIndex < _controller.ItemsCount);
      IsLeftArrowEnabled = (_controller.StartIndex > 1);
    }

    private string FormatPageText( int startIndex, int endIndex, int itemsCount )
    {
      return string.Format("{0} - {1} of {2}", startIndex, endIndex, itemsCount);
    }

    public void OnRightClick()
    {
      if (_controller == null)
        return;

      _controller.MoveToNextPage();
      UpdateMove();
    }

    public void OnLeftClick()
    {
      if (_controller == null)
        return;

      _controller.MoveToPreviousPage();
      UpdateMove();
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

    private IPageController _controller;
 
  }
}
