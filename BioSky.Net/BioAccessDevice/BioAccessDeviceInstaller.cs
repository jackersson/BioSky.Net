using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using BioContracts;
using BioContracts.AccessDevices;

namespace BioAccessDevice
{
  public class BioAccessDeviceInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      try {
        container.Register(Component.For<IAccessDeviceEngine>().ImplementedBy<AccessDevicesEngine>());
      }
      catch (Exception ex) {
        Console.WriteLine("BioAccessDevice.dll" + ex.Message);
      }
    }
  }
}
