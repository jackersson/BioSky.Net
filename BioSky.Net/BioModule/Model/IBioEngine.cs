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
  public interface IBioEngine
  {

    IBioSkyNetRepository Database();

    IAccessDeviceEngine AccessDeviceEngine();

    ITrackLocationEngine TrackLocationEngine();
  }
}
