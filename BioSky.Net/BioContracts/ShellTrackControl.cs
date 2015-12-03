using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public class ShellTrackControl
  {    
    public ShellTrackControl()
    {
      _trackItems = new ObservableCollection<ShellTrackItem>();
    }
    public ObservableCollection<ShellTrackItem> TrackItems { get { return _trackItems; } }
    public object ScreenViewModel { get; set; }

    private ObservableCollection<ShellTrackItem> _trackItems;

  }
}
