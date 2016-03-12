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
      PhotoInformation = new PhotoInformationViewModel();
    }

    public void Update(Photo photo)
    {
      PhotoInformation.Update(photo);
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

    private PhotoInformationViewModel _photoInformation;
    public PhotoInformationViewModel PhotoInformation
    {
      get { return _photoInformation; }
      set
      {
        if (_photoInformation != value)
        {
          _photoInformation = value;
          NotifyOfPropertyChange(() => PhotoInformation);
        }
      }
    }




  }

}
