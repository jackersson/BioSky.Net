using BioService;
using Google.Protobuf.Collections;
using System.Collections.Generic;

namespace BioContracts.Holders
{
  public interface IFullPersonHolder : IFullHolder<Person>
  {
   
    void SetThumbnail(Person owner, Photo requested, Response responsed, bool refresh = true);

    Person GetValue(Person person);

    ICardHolder  CardDataHolder { get; }   

  }
}
