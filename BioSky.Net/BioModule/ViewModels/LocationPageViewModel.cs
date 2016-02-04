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
using BioModule.Utils;
using BioContracts;
using System.IO;

namespace BioModule.ViewModels
{
  public class LocationPageViewModel : Conductor<IScreen>.Collection.OneActive
  {
    public LocationPageViewModel(IProcessorLocator locator, IWindowManager windowManager)
    {
      _locator       = locator;
      _methodInvoker = new FastMethodInvoker();
      _windowManager = windowManager;
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioService    = _locator.GetProcessor<IServiceManager>();
 
      Items.Add(new LocationAccessDevicesViewModel (_locator));
      Items.Add(new LocationCaptureDevicesViewModel(_locator));
      Items.Add(new LocationUsersNotifyViewModel   (_locator));

      ActiveItem = Items[0];
      OpenTab();


      DisplayName = "Location Settings";
    }

    public void Update(Location location)
    {
      if (location != null)
      {
        CurrentLocation = location.Clone();
      }      
      else
        CurrentLocation = new Location() { LocationName = "", Description = "", Dbstate = DbState.Insert };

      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { CurrentLocation });    

    }
    public async Task LocationUpdatePerformer(DbState state)
    {
      CurrentLocation.Dbstate = state;

      LocationList locationList = new LocationList();
      locationList.Locations.Add(CurrentLocation);

      _database.LocationHolder.DataUpdated += LocationHolder_DataUpdated;

      await _bioService.DatabaseService.LocationUpdateRequest(locationList);
    }
    private void LocationHolder_DataUpdated(IList<Location> list, Result result)
    {
      _database.LocationHolder.DataUpdated -= LocationHolder_DataUpdated;
      foreach (ResultPair currentResult in result.Status)
      {
        Location location = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State != DbState.Remove)
            location = currentResult.Location;
          if (location != null)
          {
            CurrentLocation = location;
          }
        }

        MessageBox.Show("Location: " + location.LocationName + " " + currentResult.Status.ToString());
      }
    }
    public async void Apply()
    {
      foreach (IUpdatable updatableScreen in Items)
        updatableScreen.Apply();

      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());

      if (result == true)
      {
        await LocationUpdatePerformer(DbState.Insert);
        /*await GetCurrentImage();
        await UserUpdatePerformer(DbState.Update);*/
      }
    }

    public void Revert()
    {

    }

    public void Delete()
    {

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

    private readonly IProcessorLocator    _locator      ;
    private readonly FastMethodInvoker    _methodInvoker;
    private          IBioSkyNetRepository _database     ;
    private          IWindowManager       _windowManager;
    private readonly IServiceManager      _bioService   ;
  }
}
