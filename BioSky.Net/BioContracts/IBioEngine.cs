using BioService;


namespace BioContracts
{
  public enum Activity : long
  {
     None
   , UserAdd        = 1 << 0
   , UserUpdate     = 1 << 1
   , UserRemove     = 1 << 2

   , LocationAdd    = 1 << 3
   , LocationUpdate = 1 << 4
   , LocationRemove = 1 << 5


  }

  public interface IBioEngine
  {
    void Stop();

    IBioSkyNetRepository Database();

    IAccessDeviceEngine AccessDeviceEngine();

    ICaptureDeviceEngine CaptureDeviceEngine();

    ITrackLocationEngine TrackLocationEngine();
   
    bool IsActivityAllowed(Activity permissionRule);
    
    Person AuthenticatedPerson { get; set; }
    
  }
}
