using BioContracts;
using BioContracts.AccessDevices;
using BioContracts.CaptureDevices;
using BioContracts.FingerprintDevices;
using BioContracts.IrisDevices;
using BioContracts.Locations;
using BioGRPC.Utils;
using BioService;

namespace BioEngine
{
  public class BioSkyEngine : IBioEngine
  {    
    public BioSkyEngine( IProcessorLocator locator )      
    {      
      _data                    = locator.GetProcessor<IBioSkyNetRepository>    ();
      _accessDeviceEngine      = locator.GetProcessor<IAccessDeviceEngine>     ();
      _trackLocationEngine     = locator.GetProcessor<ITrackLocationEngine>    ();
      _captureDeviceEngine     = locator.GetProcessor<ICaptureDeviceEngine>    ();
      _loginInformation        = locator.GetProcessor<ILoginInformation>       ();
      _fingerprintDeviceEngine = locator.GetProcessor<IFingerprintDeviceEngine>();
      _irisDeviceEngine        = locator.GetProcessor<IIrisDeviceEngine>       ();
      _permissionController    = locator.GetProcessor<IPermissionController>   ();

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

    public IFingerprintDeviceEngine FingerprintDeviceEngine()
    {
      return _fingerprintDeviceEngine;
    }
    public IIrisDeviceEngine IrisDeviceEngine()
    {
      return _irisDeviceEngine;
    }

    public void Stop()
    {
      _captureDeviceEngine.RemoveAll();
      _accessDeviceEngine.RemoveAll();
    }

    public bool IsActivityAllowed(Activity activity)
    {
      return _permissionController.isActivityAllowed(activity);
    }    

    private Person _authenticatedPerson;
    public Person AuthenticatedPerson
    {
      get { return _authenticatedPerson; }
      set
      {
        _authenticatedPerson = value;
        _loginInformation.UpdateUser(_authenticatedPerson);
        Rights rights = (_authenticatedPerson != null)
                                                                   ? _authenticatedPerson.Rights
                                                                   : Rights.Manager;
        
        _permissionController.UpdateAuthenticatedPersonRights(rights);
      }
    }

    private readonly IBioSkyNetRepository     _data                   ;
    private readonly ICaptureDeviceEngine     _captureDeviceEngine    ;
    private readonly IAccessDeviceEngine      _accessDeviceEngine     ;
    private readonly IFingerprintDeviceEngine _fingerprintDeviceEngine;
    private readonly ITrackLocationEngine     _trackLocationEngine    ;
    private          ILoginInformation        _loginInformation       ;
    private readonly IIrisDeviceEngine        _irisDeviceEngine       ;
    private readonly IPermissionController    _permissionController   ;
  }
}
