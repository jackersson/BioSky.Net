using BioService;

namespace BioContracts
{
  public interface IFullPersonHolder : IFullHolder<Person>
  {
    Person GetPersonByCardNumber(string cardNumber);
  }
}
