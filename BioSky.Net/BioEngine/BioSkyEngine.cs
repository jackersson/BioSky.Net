using BioContracts;
using BioService;

namespace BioEngine
{
  public class BioSkyEngine : IBioEngine
  {    
    public BioSkyEngine( IProcessorLocator locator )      
    {      
      _data                = locator.GetProcessor<IBioSkyNetRepository>();
      _accessDeviceEngine  = locator.GetProcessor<IAccessDeviceEngine>() ;
      _trackLocationEngine = locator.GetProcessor<ITrackLocationEngine>();
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      _loginInformation    = locator.GetProcessor<ILoginInformation>()   ;      

      AuthenticatedPerson = null;
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

    public bool IsActivityAllowed(Activity activity)
    {
      return PermissionController.Instance.isActivityAllowed(activity);
    }    

    private Person _authenticatedPerson;
    public Person AuthenticatedPerson
    {
      get { return _authenticatedPerson; }
      set
      {
        _authenticatedPerson = value;
        _loginInformation.UpdateUser(_authenticatedPerson);
        Person.Types.Rights rights = (_authenticatedPerson != null)
                                                                   ? _authenticatedPerson.Rights
                                                                   : Person.Types.Rights.Custom;

        PermissionController.Instance.UpdateAuthenticatedPersonRights(rights);
      }
    }

    private readonly IBioSkyNetRepository _data               ;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IAccessDeviceEngine  _accessDeviceEngine ;
    private readonly ITrackLocationEngine _trackLocationEngine;
    private          ILoginInformation    _loginInformation   ;
  }
}
