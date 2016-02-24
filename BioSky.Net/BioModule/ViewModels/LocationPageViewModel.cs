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
using BioService;
using BioModule.Utils;
using BioContracts;
using System.IO;
using Google.Protobuf.Collections;
using Grpc.Core;

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

      _locationAccessDevicesView  = new LocationAccessDevicesViewModel(_locator, _windowManager);
      //_locationCaptureDevicesView = new LocationCaptureDevicesViewModel(_locator, _windowManager);

      Items.Add(LocationAccessDevicesView);
      //Items.Add(LocationCaptureDevicesView);
      Items.Add(new LocationUsersNotifyViewModel   (_locator, _windowManager));

      ActiveItem = Items[0];
      OpenTab();


      DisplayName = "LocationSettings";
    }

    #region Update
    public void Update(Location location)
    {
      if (location != null)
      {
        CurrentLocation = location.Clone();
        RevertLocation = location.Clone();

        CurrentLocation.EntityState = EntityState.Modified;
      }
      else
        CurrentLocation = new Location() { LocationName = "", Description = "", EntityState = EntityState.Added };


      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { CurrentLocation });

    }
    public void UpdateData(LocationList list)
    {
      _database.Locations.DataUpdated -= UpdateData;

      if (list != null)
      {
        Location location = list.Locations.FirstOrDefault();
        if (location != null)
        {
          if (location.EntityState == EntityState.Deleted)
          {
            location = null;
            MessageBox.Show("Location successfully Deleted");
          }
          else if (location.EntityState == EntityState.Added)
            MessageBox.Show("Location successfully Added");
          else if (location.EntityState == EntityState.Unchanged)
          {
            location.LocationName = RevertLocation.LocationName;
            location.Description = RevertLocation.Description;
            MessageBox.Show("Location successfully Updated");
          }
          else
            MessageBox.Show("Location successfully Updated");

          Update(location);
        }
      }
    }
    #endregion

    #region BioService
    public async Task LocationUpdatePerformer(EntityState state)
    {
      CurrentLocation.EntityState = state;

      Location location = null;

      if (state == EntityState.Modified)
      {
        if (RevertLocation.Description != CurrentLocation.Description || RevertLocation.LocationName != CurrentLocation.LocationName)
        {
          location = CurrentLocation;
          location.EntityState = EntityState.Modified;
        }
        else
        {
          location = new Location() { Id = CurrentLocation.Id };
          location.EntityState = EntityState.Unchanged;
        }
      }
      else
        location = CurrentLocation;


      LocationList locationList = new LocationList();

      RepeatedField<AccessDevice> accessDevices = LocationAccessDevicesView.GetAccessDevices();
      RepeatedField<CaptureDevice> captureDevices = LocationAccessDevicesView.GetCaptureDevices();

      foreach (AccessDevice item in accessDevices)
      {
        AccessDevice accessDevice = null;
        bool accessDeviceExist = _database.AccessDeviceHolder.DataSet.TryGetValue(item.Id, out accessDevice);
        if (accessDeviceExist)
          item.EntityState = EntityState.Added;
        else
          item.EntityState = EntityState.Added;

        location.AccessDevices.Add(item);
      }

      foreach (CaptureDevice item in captureDevices)
      {
        CaptureDevice captureDevice = null;
        bool captureDeviceExist = _database.CaptureDeviceHolder.DataSet.TryGetValue(item.Id, out captureDevice);
        if (captureDeviceExist)
          item.EntityState = EntityState.Added;
        else
          item.EntityState = EntityState.Added;

        location.CaptureDevices.Add(item);
      }

      locationList.Locations.Add(location);

      try
      {
        _database.Locations.DataUpdated += UpdateData;

        await _bioService.DatabaseService.LocationUpdate(locationList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }
    }
    #endregion

    #region Interface
    public async void Apply()
    {
      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());

      if (result == true)
      {
        try
        {
          await LocationUpdatePerformer(CurrentLocation.EntityState);
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        }
      }
    }

    public void Revert()
    {
      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());

      if (result == true)
        Update(RevertLocation);
    }

    public void Delete()
    {

    }

    public void OpenTab()
    {
      ActiveItem.Activate();
    }
    #endregion

    #region UI
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

    private Location _revertLocation;
    public Location RevertLocation
    {
      get { return _revertLocation; }
      set
      {
        if (_revertLocation != value)
        {
          _revertLocation = value;
          NotifyOfPropertyChange(() => RevertLocation);
        }
      }
    }
    private LocationAccessDevicesViewModel _locationAccessDevicesView;
    public LocationAccessDevicesViewModel LocationAccessDevicesView
    {
      get { return _locationAccessDevicesView; }
      set
      {
        if (_locationAccessDevicesView != value)
        {
          _locationAccessDevicesView = value;
          NotifyOfPropertyChange(() => LocationAccessDevicesView);
        }
      }
    }

  

    #endregion

    #region GlobalVariables

    private readonly IProcessorLocator    _locator      ;
    private readonly FastMethodInvoker    _methodInvoker;
    private          IBioSkyNetRepository _database     ;
    private          IWindowManager       _windowManager;
    private readonly IServiceManager      _bioService;

    #endregion
  }
}
