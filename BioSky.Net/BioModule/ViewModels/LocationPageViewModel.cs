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
  enum LocationPageMode
  {
     New
   , Existing
  }

  public class LocationPageViewModel : Conductor<IScreen>.Collection.OneActive
  {
    public LocationPageViewModel(IProcessorLocator locator)
    {
      _locator       = locator;
      _methodInvoker = new FastMethodInvoker();
     
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioService    = _locator.GetProcessor<IServiceManager>();
      _notifier      = _locator.GetProcessor<INotifier>();
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();

      _locationDevicesListViewModel = new DevicesListViewModel(_locator);
      _locationDevicesListViewModel.DisplayName = "Devices";

      Items.Add(_locationDevicesListViewModel);     
      Items.Add(new LocationUsersNotifyViewModel   (_locator));

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

        _locationPageMode = LocationPageMode.Existing;
      }
      else
      {
        CurrentLocation = new Location() { LocationName = "", Description = "" };
        _locationPageMode = LocationPageMode.New;
      }

      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { CurrentLocation });

    }
   
    #endregion

    private bool CheckLocationModified()
    {
      return RevertLocation.Description != CurrentLocation.Description
            || RevertLocation.LocationName != CurrentLocation.LocationName;
    }
    
    private void UpdateLocationDevices(ref Location location)
    {    
      RepeatedField<AccessDevice> accessDevices   = LocationDevicesListViewModel.AccessDevices.GetAccessDevices();
      RepeatedField<CaptureDevice> captureDevices = LocationDevicesListViewModel.CaptureDevices.GetCaptureDevices();

      foreach (AccessDevice item in accessDevices)
        location.AccessDevices.Add(item);

      foreach (CaptureDevice item in captureDevices)
        location.CaptureDevices.Add(item);
    }

    #region Interface
    public async void Apply()
    {
      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (!result)
        return;

      Location location = CurrentLocation;
      try
      {
        if (_locationPageMode == LocationPageMode.New)
        {
          UpdateLocationDevices(ref location);
          await _bioService.DatabaseService.LocationDataClient.Add(location);
        }
        else
        {
          if (!CheckLocationModified())
            location = new Location() { Id = CurrentLocation.Id };
          await _bioService.DatabaseService.LocationDataClient.Update(location);
        }
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public void Revert()
    {
      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (result)
        Update(RevertLocation);
    }

    public async void Delete()
    {
      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (!result || CurrentLocation == null)
        return;

      try
      {       
        await _bioService.DatabaseService.LocationDataClient.Delete(CurrentLocation);       
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
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
    private DevicesListViewModel _locationDevicesListViewModel;
    public DevicesListViewModel LocationDevicesListViewModel
    {
      get { return _locationDevicesListViewModel; }
      set
      {
        if (_locationDevicesListViewModel != value)
        {
          _locationDevicesListViewModel = value;
          NotifyOfPropertyChange(() => LocationDevicesListViewModel);
        }
      }
    } 

    #endregion

    #region GlobalVariables

    private readonly IProcessorLocator    _locator         ;
    private readonly FastMethodInvoker    _methodInvoker   ;
    private          IBioSkyNetRepository _database        ;
    private          LocationPageMode     _locationPageMode;
    private readonly IServiceManager      _bioService      ;
    private readonly INotifier            _notifier        ;
    private readonly DialogsHolder        _dialogsHolder   ;


    #endregion
  }
}
