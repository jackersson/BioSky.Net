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
      _windowManager = locator.GetProcessor<IWindowManager>();

      _visitorsView = new VisitorsViewModel(_locator, _windowManager);

      Items.Add(new TrackControlItemViewModel(_locator));
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

      Items[0] = /*(TrackControlItemViewModel)*/location.ScreenViewModel;      
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

    private readonly IProcessorLocator _locator      ;
    private readonly IWindowManager    _windowManager;

  }
}
