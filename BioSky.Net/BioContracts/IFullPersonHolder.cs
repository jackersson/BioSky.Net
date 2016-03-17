using BioService;
using Google.Protobuf.Collections;
using System.Collections.Generic;

namespace BioContracts
{
  public interface IFullPersonHolder : IFullHolder<Person>
  {
    Person GetPersonByCardNumber(string cardNumber);

    void RemoveCards(Person owner
                    , RepeatedField<long> requested
                    , RepeatedField<long> responsed);

    void AddCard(Person owner
                    , Card requested
                    , Card responsed);

    void AddPhoto( Person owner
                 , Photo requested
                 , Photo responsed
                 , bool refresh = true );


    void RemovePhotos(Person owner
                     , RepeatedField<long> requested
                     , RepeatedField<long> responsed);

    void SetThumbnail(Person owner, Photo requested, Response responsed, bool refresh = true);

    Person GetValue(Person person);

    HashSet<long> PhotosIndexesWithoutExistingFile { get; }
  }
}
