using BioContracts;

namespace BioEngine
{
  public class BioSkyEngine : IBioEngine
  {
    public BioSkyEngine( IProcessorLocator locator )      
    {      
      _data                = locator.GetProcessor<IBioSkyNetRepository>();
      _accessDeviceEngine  = locator.GetProcessor<IAccessDeviceEngine> ();
      _trackLocationEngine = locator.GetProcessor<ITrackLocationEngine>();
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();      
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

    public ICaptureDeviceEngine CaptureDeviceEngine()
    {
      return _captureDeviceEngine;
    }

    public void Stop()
    {
      _captureDeviceEngine.Stop();
      _accessDeviceEngine.Stop();
    }
    
    private readonly IBioSkyNetRepository _data              ;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IAccessDeviceEngine  _accessDeviceEngine;
    private readonly ITrackLocationEngine _trackLocationEngine;
  }
}
