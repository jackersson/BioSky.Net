using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using BioAccessDevice.Interfaces;

using Castle.Facilities.TypedFactory;

using BioContracts;

namespace BioAccessDevice
{
  public class BioAccessDeviceInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      try
      {
        container.Register(Component.For<IAccessDeviceEngine>().ImplementedBy<AccessDevicesEngine>());
      }
      catch (Exception ex)
      {
        Console.WriteLine("BioAccessDevice.dll" + ex.Message);
      }
    }
  }
}
