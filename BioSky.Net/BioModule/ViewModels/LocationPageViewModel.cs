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
using BioGRPC.Utils;

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
      _validator     = new BioValidator();    

      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioService    = _locator.GetProcessor<IServiceManager>();
      _notifier      = _locator.GetProcessor<INotifier>();
      _dialogsHolder = _locator.GetProcessor<DialogsHolder>();
      _bioEngine     = _locator.GetProcessor<IBioEngine>();

      _locationPermissionViewModel = new LocationPermissionViewModel(_locator);
     // _locationPermissionViewModel.PermissionsChanged += _locationPermissionViewModel_PermissionsChanged;

      LocationAccessDevices  = new LocationAccessDevicesViewModel (_locator);
      LocationCaptureDevices = new LocationCaptureDevicesViewModel(_locator);
      LocationFingerDevices  = new LocationFingerDevicesViewModel (_locator);

      LocationAccessDevices .PropertyChanged += DevicesPropertyChanged;
      LocationCaptureDevices.PropertyChanged += DevicesPropertyChanged;
      LocationFingerDevices .PropertyChanged += DevicesPropertyChanged;

      Items.Add(LocationAccessDevices ); 
      Items.Add(LocationCaptureDevices);
      Items.Add(LocationFingerDevices );
      Items.Add(_locationPermissionViewModel );

      ActiveItem = Items[0];
      OpenTab();      

      _database.Locations.DataChanged += Locations_DataChanged;

      DisplayName = "LocationSettings";
    }

    private void Locations_DataChanged()
    {
      Location location;
      if (_locationPageMode == LocationPageMode.Existing)      
        location = _database.Locations.GetValue(CurrentLocation.Id);      
      else      
        location = _database.Locations.GetValue(CurrentLocation);      
      Update(location);
    }

    private void RefreshUI()
    {
      
      NotifyOfPropertyChange(() => CanApply);
      NotifyOfPropertyChange(() => CanRevert);
      NotifyOfPropertyChange(() => CanDelete);
    }

    protected override void OnActivate()
    {
      RefreshUI();
      base.OnActivate();
    }
   /* 
    private void _locationPermissionViewModel_PermissionsChanged(object sender, EventArgs e)
    {
      //RefreshUI();
    }
*/
    private void DevicesPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      RefreshUI();
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
        CurrentLocation = temp;
        _revertLocation = null;
        _locationPageMode = LocationPageMode.New;
      }

      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { CurrentLocation });

      RefreshUI();
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

      bool flag = _revertLocation == null;
      
      if (flag || CurrentLocation.Description != _revertLocation.Description)
          location.Description = CurrentLocation.Description;

      if (flag || CurrentLocation.LocationName != _revertLocation.LocationName)
          location.LocationName = CurrentLocation.LocationName;
           
      location.MacAddress = MacAddress;

      CaptureDevice captureDevice = LocationCaptureDevices.GetDevice();
      if (captureDevice != null)
        location.CaptureDevice = captureDevice;
     
      AccessDevice accessDevice   = LocationAccessDevices.GetDevice();
      if (accessDevice != null)
        location.AccessDevice = accessDevice;
        
      FingerprintDevice fingerprintDevice   = LocationFingerDevices.GetDevice();
      if (fingerprintDevice != null)
        location.FingerprintDevice = fingerprintDevice;
      
      AccessInfo accessInfo = _locationPermissionViewModel.GetResult();
      if (accessInfo != null)
        location.AccessInfo = accessInfo;

     
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
        return IsActive && string.IsNullOrEmpty(Error) 
                        &&(  LocationAccessDevices .CanApply
                          || LocationCaptureDevices.CanApply
                          || LocationFingerDevices .CanApply)
                        && ( ( _locationPageMode == LocationPageMode.New )
                          || ( _locationPageMode == LocationPageMode.Existing && CanRevert) );
      }
    }

    public bool CanRevert
    {
      get
      {
        return IsActive && _revertLocation != null
                        && _currentLocation != null
                        && (  _locationPermissionViewModel.IsAccessChanged()
                           || LocationAccessDevices .IsDeviceChanged 
                           || LocationCaptureDevices.IsDeviceChanged 
                           || LocationFingerDevices .IsDeviceChanged
                           || _currentLocation.GetHashCode() != _revertLocation.GetHashCode());
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
        Refresh();     
      }
    }

    [Required(ErrorMessage = "You must enter a Location Description.")]
    [RegularExpression(@"^[a-zA-Zа-яА-яїі `\-]+$", ErrorMessage = "The Description must only contain letters (a-z, A-Z, а-я, А-я).")]
    public string Description
    {
      get { return (_currentLocation != null) ? _currentLocation.Description : string.Empty; }
      set
      {
        _currentLocation.Description = value;
        NotifyOfPropertyChange(() => Description);
        Refresh();
      }
    }

    public string MacAddress
    {
      get
      {       
        return (_currentLocation != null && !string.IsNullOrEmpty(_currentLocation.MacAddress))
                   ? _currentLocation.MacAddress
                   : _bioService.MacAddress;
      }
    }

    private LocationAccessDevicesViewModel _locationAccessDevices;
    public LocationAccessDevicesViewModel LocationAccessDevices
    {
      get { return _locationAccessDevices; }
      set
      {
        if (_locationAccessDevices != value)
        {
          _locationAccessDevices = value;
          NotifyOfPropertyChange(() => LocationAccessDevices);
          Refresh();
        }
      }
    }

    private LocationCaptureDevicesViewModel _locationCaptureDevices;
    public LocationCaptureDevicesViewModel LocationCaptureDevices
    {
      get { return _locationCaptureDevices; }
      set
      {
        if (_locationCaptureDevices != value)
        {
          _locationCaptureDevices = value;
          NotifyOfPropertyChange(() => LocationCaptureDevices);
          Refresh();
        }
      }
    }

    private LocationFingerDevicesViewModel _locationFingerDevices;
    public LocationFingerDevicesViewModel LocationFingerDevices
    {
      get { return _locationFingerDevices; }
      set
      {
        if (_locationFingerDevices != value)
        {
          _locationFingerDevices = value;
          NotifyOfPropertyChange(() => LocationFingerDevices);
          Refresh();
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
    private readonly IBioEngine           _bioEngine       ;

    #endregion
  }
}
