using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using BioContracts.FingerprintDevices;

namespace BioFingerprintDevices
{
  public class BioFingerprintDevicesInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      try {
        container.Register(Component.For<IFingerprintDeviceEngine>().ImplementedBy<FingerprintDeviceEngine>());
      }
      catch (Exception ex) {
        Console.WriteLine("BioFingerprintDevice.dll" + ex.Message);
      }
    }
  }
}
