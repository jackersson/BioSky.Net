using BioService;

namespace BioContracts
{
  public interface ILoginInformation
  {
    void UpdateUser(Person user);

    object LoginInformation { get; }
  }
}
