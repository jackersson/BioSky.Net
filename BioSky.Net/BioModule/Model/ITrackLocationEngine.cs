using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.Model
{
  public interface ITrackLocationEngine
  {
    void Init();

    ObservableCollection<TrackLocation> TrackLocations();
  }
}
