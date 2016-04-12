using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using BioContracts;

namespace BioModule.ViewModels
{
  public class TrackTabControlViewModel : Conductor<IScreen>.Collection.OneActive
  {
    public TrackTabControlViewModel(IProcessorLocator locator)
    {
      VisitorsView      = new VisitorsViewModel            (locator);
      FullTrackLocation = new FullTrackControlItemViewModel(locator);

      Items.Add(FullTrackLocation);
      Items.Add(VisitorsView     );

      ActiveItem = Items[0];
      OpenTab();
    }
    public void OpenTab()
    {
      if (ActiveItem != null)
        ActiveItem.Activate();
    }

    public void Update(TrackLocation location)
    {
      if (location == null)
        return;

      FullTrackLocation.Update(location);
     
      ActiveItem = Items[0];
      OpenTab();
    }

    private VisitorsViewModel _visitorsView;
    public VisitorsViewModel VisitorsView
    {
      get { return _visitorsView; }
      private set
      {
        if (_visitorsView != value)
        {
          _visitorsView = value;
          NotifyOfPropertyChange(() => VisitorsView);
        }
      }
    }

    private FullTrackControlItemViewModel _fullTrackControl;
    public FullTrackControlItemViewModel FullTrackLocation
    {
      get { return _fullTrackControl; }
      private set
      {
        if (_fullTrackControl != value)
        {
          _fullTrackControl = value;
          NotifyOfPropertyChange(() => FullTrackLocation);
        }
      }
    }    
  }
}
