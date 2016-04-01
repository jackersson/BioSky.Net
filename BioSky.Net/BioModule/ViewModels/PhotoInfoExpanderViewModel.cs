using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using BioService;

namespace BioModule.ViewModels
{
  public class PhotoInfoExpanderViewModel : Screen
  {
    public PhotoInfoExpanderViewModel()
    {

    }

    public void Update(object informationModel)
    {
      InformationItem = informationModel;
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

    private object _informationItem;
    public object InformationItem
    {
      get { return _informationItem; }
      set
      {
        if (_informationItem != value)
        {
          _informationItem = value;
          NotifyOfPropertyChange(() => InformationItem);
        }
      }
    }
  }
}
