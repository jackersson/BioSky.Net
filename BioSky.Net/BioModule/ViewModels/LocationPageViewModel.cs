using Caliburn.Micro;
using BioService;
using BioModule.Utils;
using BioContracts;
using Google.Protobuf.Collections;
using Grpc.Core;
using System;
using BioModule.Validation;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BioModule.ViewModels
{
  enum LocationPageMode
  {
     New
   , Existing
  }

  public class LocationPageViewModel : Conductor<IScreen>.Collection.OneActive, IDataErrorInfo
  {
    public event ValidationStateEventHandler ValidationStateChanged;
    public LocationPageViewModel(IProcessorLocator locator)
    {
      _locator       = locator;
      _methodInvoker = new FastMethodInvoker();
      _validator = new  BioValidator();

      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioService    = _locator.GetProcessor<IServiceManager>();
      _notifier      = _locator.GetProcessor<INotifier>();
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();

      _locationDevicesListViewModel = new DevicesListViewModel(_locator);
      _locationDevicesListViewModel.DisplayName = "Devices";
      _locationDevicesListViewModel.PropertyChanged += DevicesPropertyChanged;

      _locationPermissionViewModel = new LocationPermissionViewModel(_locator);
      _locationPermissionViewModel.PermissionsChanged += _locationPermissionViewModel_PermissionsChanged;

      Items.Add(_locationDevicesListViewModel);     
      Items.Add(_locationPermissionViewModel );

      ActiveItem = Items[0];
      OpenTab();

      _database.Locations.DataChanged += Locations_DataChanged;

      DisplayName = "LocationSettings";
    }

    private void Locations_DataChanged()
    {
      if (_locationPageMode == LocationPageMode.Existing)
      {
        Location location = _database.Locations.GetValue(CurrentLocation.Id);
        Update(location);
      }
      else
      {
        Location location = _database.Locations.GetValue(CurrentLocation);
        Update(location);
      }
    }

    private void Refresh()
    {
      
      NotifyOfPropertyChange(() => CanApply);
      NotifyOfPropertyChange(() => CanRevert);
      NotifyOfPropertyChange(() => CanDelete);
    }

    protected override void OnActivate()
    {
      Refresh();
      base.OnActivate();
    }
    
    private void _locationPermissionViewModel_PermissionsChanged(object sender, EventArgs e)
    {
      Refresh();
    }

    private void DevicesPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Refresh();
    }

    #region Update
    public void Update(Location location)
    {
      if (location != null)
      {
        CurrentLocation = location.Clone();
        _revertLocation = location.Clone();

        _locationPageMode = LocationPageMode.Existing;
      }
      else
      {
        Location temp = new Location() { LocationName = "", Description = "" };
        //temp.CaptureDevices.Add(new CaptureDevice() { Devicename = "USB Camera" });
        CurrentLocation = temp;
        _locationPageMode = LocationPageMode.New;
      }

      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { CurrentLocation });

      Refresh();
    }

    #endregion

    #region Validation    
    public string this[string columnName]
    {
      get {
        string temp = string.Join(Environment.NewLine, _validator.Validate(this, columnName).Select(x => x.Message));
        Error = string.IsNullOrEmpty(temp) ? string.Join(Environment.NewLine, _validator.Validate(this).Select(x => x.Message))
                                            : temp;
        return temp;
      }
    }

    public string _error;
    public string Error
    {
      get { return _error; }
      set
      {
        if (_error != value)
        {
          _error = value;
          OnValidationStateChanged(string.IsNullOrEmpty(Error));
        }
      }
    }

    private void OnValidationStateChanged(bool state)
    {
      if (ValidationStateChanged != null)
        ValidationStateChanged(state);

      NotifyOfPropertyChange(() => CanApply);
    }
    #endregion

    private Location GetLocation()
    {
      Location location = new Location();

      if (CurrentLocation.Id > 0)
        location.Id = CurrentLocation.Id;

      if (CurrentLocation.Description != _revertLocation.Description)
        location.Description  = CurrentLocation.Description;

      if (CurrentLocation.LocationName != _revertLocation.LocationName)
        location.LocationName = CurrentLocation.LocationName;

      AccessDevice  accessDevice  = LocationDevicesListViewModel.AccessDevices .GetDevice();
      CaptureDevice captureDevice = LocationDevicesListViewModel.CaptureDevices.GetDevice();
      
      if (accessDevice != null)        
        location.AccessDevice = accessDevice;      

      if (captureDevice != null)  
        location.CaptureDevice = captureDevice;      

      if (CurrentLocation.AccessType != _locationPermissionViewModel.SelectedState)
        location.AccessType = _locationPermissionViewModel.SelectedState;

      RepeatedField<Person> persons = _locationPermissionViewModel.GetResult();
      if (persons.Count > 0)
        location.Persons.Add(persons);    
     

      return location;
    }

    #region Interface
    public async void Apply()
    {
      var result = _dialogsHolder.AreYouSureDialog.Show();

      if (!result.Value)
        return;
    
      Location location = GetLocation();      
      try
      {
        if (_locationPageMode == LocationPageMode.New)          
          await _bioService.DatabaseService.LocationDataClient.Add(location);        
        else              
          await _bioService.DatabaseService.LocationDataClient.Update(location);        
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }
      
    }

    public void Revert()
    {   
      var result = _dialogsHolder.AreYouSureDialog.Show();

      if (result.Value)
        Update(_revertLocation);
    }

    public async void Delete()
    {
      var result = _dialogsHolder.AreYouSureDialog.Show();
      
      if (!result.Value || CurrentLocation == null)
        return;

      try {       
        await _bioService.DatabaseService.LocationDataClient.Remove(CurrentLocation);       
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }
    }

    public void OpenTab()
    {
      ActiveItem.Activate();
    }    
    #endregion

    #region UI

    public bool CanApply
    {
      get {
        return IsActive && string.IsNullOrEmpty(Error) && _locationDevicesListViewModel.CanApply
                        && ( ( _locationPageMode == LocationPageMode.New )
                          || ( _locationPageMode == LocationPageMode.Existing && CanRevert) );
      }
    }

    public bool CanRevert
    {
      get {
        return IsActive && _revertLocation != null
                        && _currentLocation != null 
                        && _locationPermissionViewModel.IsAccessChanged;
                         
      }
    }

    public bool CanDelete
    {
      get { return IsActive && _currentLocation != null && _currentLocation.Id > 0; }
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
          NotifyOfPropertyChange(() => LocationName   );
          NotifyOfPropertyChange(() => Description    );

          NotifyOfPropertyChange(() => CanApply);
          NotifyOfPropertyChange(() => CanRevert);
          NotifyOfPropertyChange(() => CanDelete);

        }
      }
    }

    [Required(ErrorMessage = "You must enter Location Name.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The Location Name must only contain letters (a-z, A-Z).")]
    public string LocationName
    {
      get { return (_currentLocation != null) ? _currentLocation.LocationName : string.Empty; }
      set
      {
        _currentLocation.LocationName = value;
        NotifyOfPropertyChange(() => LocationName);
      }
    }

    [Required(ErrorMessage = "You must enter a Location Description.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The Description must only contain letters (a-z, A-Z).")]
    public string Description
    {
      get { return (_currentLocation != null) ? _currentLocation.Description : string.Empty; }
      set
      {
        _currentLocation.Description = value;
        NotifyOfPropertyChange(() => Description);
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

    private LocationPermissionViewModel _locationPermissionViewModel;
    public LocationPermissionViewModel LocationPermissionViewModel
    {
      get { return _locationPermissionViewModel; }
      set
      {
        if (_locationPermissionViewModel != value)
        {
          _locationPermissionViewModel = value;
          NotifyOfPropertyChange(() => LocationPermissionViewModel);
        }
      }
    }

    #endregion

    #region GlobalVariables


    private Location _revertLocation;

    private readonly IProcessorLocator    _locator         ;
    private readonly FastMethodInvoker    _methodInvoker   ;
    private          IBioSkyNetRepository _database        ;
    private          LocationPageMode     _locationPageMode;
    private readonly IServiceManager      _bioService      ;
    private readonly INotifier            _notifier        ;
    private readonly DialogsHolder        _dialogsHolder   ;
    private readonly IValidator           _validator       ;

    #endregion
  }
}
