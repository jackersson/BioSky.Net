using BioContracts.CaptureDevices;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;

namespace BioCaptureDevices
{
  public class BioCaptureDevicesInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      try
      {
        container.Register(Component.For<ICaptureDeviceEngine>().ImplementedBy<CaptureDeviceEngine>());
      }
      catch (Exception ex)
      {
        Console.WriteLine("BioCaptureDevice.dll" + ex.Message);
      }
    }
  }
}
