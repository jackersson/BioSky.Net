using BioData;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioContracts;
using BioService;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class LocationUsersNotifyViewModel : Screen, IUpdatable
  {
    public LocationUsersNotifyViewModel(IProcessorLocator locator, IWindowManager windowManager)
    {
      DisplayName = "UsersNotification";

      _locator = locator;
      _bioService = _locator.GetProcessor<IServiceManager>();
      _bioEngine = _locator.GetProcessor<IBioEngine>();           
    }

   
    public void Update(Location location)
    {
      _location = location;
    }

    public void Apply()
    {

    } 

    private          Location          _location  ;
    private readonly IProcessorLocator _locator   ;
    private readonly IBioEngine        _bioEngine ;
    private readonly IServiceManager   _bioService;
  }
}
