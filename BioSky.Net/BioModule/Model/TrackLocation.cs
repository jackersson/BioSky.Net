using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioData;
using BioContracts;

namespace BioModule.Model
{
  public class TrackLocation 
  {

    public TrackLocation( IAccessDeviceEngine accessDeviceEngine, Location location)
    {
      _accessDeviceEngine = accessDeviceEngine;
      Update(location);

    }

    public void Update( Location location )
    {
      _location = location;
    }

    public void Start()
    {
      _accessDeviceEngine.Add(_location.Devices_IN_);
    }

    public void Subscribe( IObserver<AccessDeviceActivity> observer )
    {
      _accessDeviceEngine.Subscribe(observer, _location.Devices_IN_);
    }

    public void Unsubscribe(IObserver<AccessDeviceActivity> observer)
    {

    }

    public void Stop()
    {
      _accessDeviceEngine.Remove(_location.Devices_IN_);
    }


    public object ScreenViewModel  { get; set; }
  
    public string Caption
    {
      get { return _location.Location_Name;  }
    }

    private bool _isChecked;

    public bool IsChecked
    {
      get { return _isChecked;  }
      set
      {
        if (_isChecked == value)
          return;
        _isChecked = value;
      }
    }

    private Location _location;
    private readonly IAccessDeviceEngine _accessDeviceEngine;
  }
}
