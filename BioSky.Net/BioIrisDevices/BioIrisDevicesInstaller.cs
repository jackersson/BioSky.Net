using BioContracts.IrisDevices;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioIrisDevices
{
  public class BioIrisDevicesInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      try
      {
        container.Register(Component.For<IIrisDeviceEngine>().ImplementedBy<IrisDeviceEngine>());
      }
      catch (Exception ex)
      {
        Console.WriteLine("BioFingerprintDevice.dll" + ex.Message);
      }
    }
  }
}
