using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.Model
{
  public class TrackControl
  {
    public TrackControl()
    {
      _trackItems = new ObservableCollection<TrackItem>();
    }
    public ObservableCollection<TrackItem> TrackItems { get { return _trackItems; } }
    public object ScreenViewModel { get; set; }

    private ObservableCollection<TrackItem> _trackItems;

  }
}
