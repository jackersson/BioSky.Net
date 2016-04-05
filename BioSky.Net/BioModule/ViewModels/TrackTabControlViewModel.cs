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
      _locator       = locator      ;
     
      _visitorsView    = new VisitorsViewModel(_locator);
      FullTrackControl = new FullTrackControlItemViewModel(_locator);
      Items.Add(FullTrackControl);
      Items.Add(_visitorsView);

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

     // FullTrackControl.Update((TrackControlItemViewModel)location.ScreenViewModel);

      //Items[0] = location.ScreenViewModel;      
      ActiveItem = Items[0];
      OpenTab();
    }

    private VisitorsViewModel _visitorsView;
    public VisitorsViewModel VisitorsView
    {
      get { return _visitorsView; }
      set
      {
        if (_visitorsView != value)
        {
          _visitorsView = value;
          NotifyOfPropertyChange(() => VisitorsView);
        }
      }
    }

    private FullTrackControlItemViewModel _fullTrackControl;
    public FullTrackControlItemViewModel FullTrackControl
    {
      get { return _fullTrackControl; }
      set
      {
        if (_fullTrackControl != value)
        {
          _fullTrackControl = value;
          NotifyOfPropertyChange(() => FullTrackControl);
        }
      }
    }

    private readonly IProcessorLocator _locator      ;
  }
}
