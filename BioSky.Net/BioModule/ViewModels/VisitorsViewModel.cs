using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using BioData;
using System.Collections.ObjectModel;
using BioModule.Model;

namespace BioModule.ViewModels
{
  public class VisitorsViewModel : PropertyChangedBase
  {

    private ObservableCollection<Visitor> _visitors;
    public ObservableCollection<Visitor> Visitors
    {
      get { return _visitors; }
      set
      {
        if (_visitors != value)
        {
          _visitors = value;
          NotifyOfPropertyChange(() => Visitors);
        }
      }
    }

    IBioEngine _bioEngine;
    public VisitorsViewModel(IBioEngine bioEngine)
    {
      _bioEngine = bioEngine;
      _visitors = new ObservableCollection<Visitor>();

      byte[] detection_time = new byte[]{125};
      Visitor user2 = new Visitor { UID = 2, Detection_Time = detection_time, Status = "Ok" };
      _visitors.Add(user2);

      List <Visitor> visitors = (List<Visitor>)_bioEngine.Database().getAllVisitors();
      foreach (Visitor visitor in visitors)
        _visitors.Add(visitor);

      NotifyOfPropertyChange(() => Visitors);
    }

    public BitmapSource AddIconSource
    {
      get { return ResourceLoader.AddIconSource; }
    }

    public BitmapSource RemoveIconSource
    {
      get { return ResourceLoader.RemoveIconSource; }
    }

    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }
  }
}
