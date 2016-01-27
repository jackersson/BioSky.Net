using BioContracts;
using BioEngine.CaptureDevices;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BioEngine
{
  public class BioEngineInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      try
      {
     
        container.Register(Component.For<ICaptureDeviceEngine>().ImplementedBy<CaptureDeviceEngine>())
                 .Register(Component.For<ITrackLocationEngine>().ImplementedBy<TrackLocationEngine>())
                 .Register(Component.For<IBioEngine>().ImplementedBy<BioSkyEngine>())
                 .Register(Component.For<IBioStarter>().ImplementedBy<BioStarter>());
      }
      catch (Exception ex)
      {
        Console.WriteLine("BioEngine.dll" + ex.Message);
      }
    }
  } 
}
