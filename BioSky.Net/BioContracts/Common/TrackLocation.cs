using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public class TrackLocation
  {

    public TrackLocation(IProcessorLocator locator, Location location)
    {
      _accessDeviceEngine  = locator.GetProcessor<IAccessDeviceEngine>();
      _database            = locator.GetProcessor<IBioSkyNetRepository>();
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      _bioService          = locator.GetProcessor<IServiceManager>();

      _database.AccessDevicesChanged += _database_AccessDevicesChanged;
     _database.CaptureDevicesChanged += _database_CaptureDevicesChanged;  

      Update(location);
    }

    private void _database_CaptureDevicesChanged(object sender, EventArgs e)
    {
      List<CaptureDevice> capture_devices = _database.CaptureDevices
                                            .Where(cap => cap.Locationid == _location.Id)
                                            .ToList();

      foreach (CaptureDevice cd in capture_devices)
      {
        _captureDeviceEngine.Add(cd.Devicename);
        CaptureDeviceName = cd.Devicename;
      }     
    }

    private void _database_AccessDevicesChanged(object sender, EventArgs e)
    {
      List<AccessDevice> access_devices = _database.AccessDevices
                                         .Where(ad => ad.Locationid == _location.Id)
                                         .ToList();

      foreach (AccessDevice ac in access_devices)
      {
        _accessDeviceEngine.Add(ac.Portname);
        AccessDeviceName = ac.Portname;
      }
    }

    private string _captureDeviceName;
    public string CaptureDeviceName
    {
      get { return _captureDeviceName; }
      set
      {
        if (_captureDeviceName != value )
        {
          _captureDeviceName = value;          
        }
      }
    }

    private string _accessDeviceName;
    public string AccessDeviceName
    {
      get { return _accessDeviceName; }
      set
      {
        if (_accessDeviceName != value)
        {
          _accessDeviceName = value;
        }
      }
    }


    public void Update(Location location)
    {
      _location = location;
    }

    public long LocationID
    {
      get { return _location.Id; }
    }

    public async void Start()
    {      
      
      if (_database.AccessDevices.Count <= 0)
        await _bioService.DatabaseService.AccessDeviceRequest(new CommandAccessDevice());
      else
        _database_AccessDevicesChanged(null, null);

      if (_database.CaptureDevices.Count <= 0)
        await _bioService.DatabaseService.CaptureDeviceRequest(new CommandCaptureDevice());
      else
        _database_CaptureDevicesChanged(null, null);
       

    }
   
    public void Stop()
    {
      //_accessDeviceEngine.Remove(_location.Devices_IN_);
    }
    
    public object ScreenViewModel { get; set; }

    public string Caption
    {
      get { return _location.LocationName; }
    }

    private Location _location;
    private readonly IAccessDeviceEngine  _accessDeviceEngine;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IServiceManager      _bioService;
    private readonly IBioSkyNetRepository _database;
    
  }
}
