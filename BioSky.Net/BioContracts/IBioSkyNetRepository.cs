using BioFaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using Google.Protobuf;

namespace BioContracts
{  
  public interface IBioSkyNetRepository
  {
    IHolder<Visitor, long> VisitorHolder {  get;  }

    IHolder<Location, long> LocationHolder { get; }

    IHolder<Photo, long> PhotoHolder { get; }

    IHolder<CaptureDevice, long> CaptureDeviceHolder  { get; }

    IHolder<AccessDevice, long> AccessDeviceHolder  { get;  }

    IHolder<Card, string> CardHolder  { get; }

    IHolder<Person, long> PersonHolder { get; }

    ILocalStorage LocalStorage  { get; }


  }
}
