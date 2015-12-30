using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioData;
using BioContracts;

namespace BioModule.Model
{
  public class TrackLocation : IObserver<AccessDeviceActivity>
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
      //_accessDeviceEngine.Add(_location.Devices_IN_);
      Subscribe(this);
    }

    public void Subscribe( IObserver<AccessDeviceActivity> observer )
    {
      //_accessDeviceEngine.Subscribe(observer, _location.Devices_IN_);
    }

    public void Unsubscribe(IObserver<AccessDeviceActivity> observer)
    {

    }

    public void Stop()
    {
      //_accessDeviceEngine.Remove(_location.Devices_IN_);
    }

    public void OnNext(AccessDeviceActivity value)
    {
      throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
      throw new NotImplementedException();
    }

    public void OnCompleted()
    {
      throw new NotImplementedException();
    }

    public object ScreenViewModel  { get; set; }
  
    public string Caption
    {
      get { return _location.Location_Name;  }
    }

    private Location _location;
    private readonly IAccessDeviceEngine _accessDeviceEngine;
  }
}
