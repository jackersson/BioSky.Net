using BioService;
using Google.Protobuf.Collections;

namespace BioContracts
{
  public interface IFullPersonHolder : IFullHolder<Person>
  {
    Person GetPersonByCardNumber(string cardNumber);

    void RemoveCards( Person owner
                    , RepeatedField<long> requested
                    , RepeatedField<long> responsed);

    void AddCard   (  Person owner
                    , Card requested
                    , Card responsed );

    void AddPhoto( Person owner
                 , Photo requested
                 , Photo responsed);


    void RemovePhotos( Person owner
                     , RepeatedField<long> requested
                     , RepeatedField<long> responsed);
    
  }
}
