using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

using System.Windows.Input;
using System.Windows.Media;
using BioData;
using BioFaceService;

using BioContracts;

namespace BioModule.ViewModels
{
  public class LocationPageViewModel : Conductor<IScreen>.Collection.OneActive
  {
    public LocationPageViewModel(IProcessorLocator locator)
    {
      _locator = locator;

      Items.Add(new LocationAccessDevicesViewModel (_locator));
      Items.Add(new LocationCaptureDevicesViewModel(_locator));
      Items.Add(new LocationUsersNotifyViewModel   (_locator));

      ActiveItem = Items[0];
      OpenTab();

      DisplayName = "Location Settings";
    }
    //************************************************Drag & Drop*****************************************************************

    public void Update(Location location)
    {
      CurrentLocation = location;
    }

    public void OpenTab()
    {
      ActiveItem.Activate();
    }

    private Location _currentLocation;
    public Location CurrentLocation
    {
      get { return _currentLocation; }
      set
      {
        if (_currentLocation != value)
        {
          _currentLocation = value;
          NotifyOfPropertyChange(() => CurrentLocation);
        }
      }
    }

    private readonly IProcessorLocator _locator;
  }
}
