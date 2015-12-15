using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BioData;
using BioContracts;
using BioAccessDevice.Interfaces;

namespace BioModule.Model
{
  public class BioEngine : IBioEngine
  {
    public BioEngine( IBioSkyNetRepository data
                    , IAccessDeviceEngine  accessDeviceEngine
                    , ITrackLocationEngine trackLocationEngine )
    {
      _data                = data;
      _accessDeviceEngine  = accessDeviceEngine;
      _trackLocationEngine = trackLocationEngine;
    }

    public IBioSkyNetRepository Database()
    {
      return _data;
    }

    public IAccessDeviceEngine AccessDeviceEngine()
    {
      return _accessDeviceEngine;
    }

    public ITrackLocationEngine TrackLocationEngine()
    {
      return _trackLocationEngine;
    }

    private readonly IBioSkyNetRepository _data              ;
    private readonly IAccessDeviceEngine  _accessDeviceEngine;
    private readonly ITrackLocationEngine _trackLocationEngine;
  }
}
