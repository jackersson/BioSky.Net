using BioContracts.Services;

namespace BioContracts
{
  public interface IServiceManager
  {
    void Start(IServiceConfiguration configuration);
    void Stop();
    IFaceService FaceService  { get; }
    IDatabaseService DatabaseService { get; }

    string MacAddress { get; }
  }
}
