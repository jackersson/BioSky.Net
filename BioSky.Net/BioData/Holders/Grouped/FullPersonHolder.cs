using BioContracts;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders.Grouped
{
  public class FullPersonHolder : IFullHolder<PersonList>
  {

    public FullPersonHolder(PersonHolder persons, PhotoHolder photos, CardHolder cards)
    {
      _persons = persons;
      _photos  = photos;
      _cards   = cards;
    }


    public void Init(PersonList list)
    {
      Google.Protobuf.Collections.RepeatedField<Person> data = list.Persons;

      foreach (Person person in data)
      {
        foreach ( Card card in person.Cards )        
          _cards.Add(card, card.UniqueNumber);
        /*
        foreach (Photo photo in person.Photos)
          _photos.Add(photo, photo.Id);

        Photo thumbnail = person.Thumbnail;
        if (thumbnail != null)
          _photos.Add(thumbnail, thumbnail.Id); */

        //TODO clear person list

        _persons.Add(person, person.Id);
      }

      OnDataChanged();
      
    }

    public void Update( PersonList updated, PersonList results )
    {
      Google.Protobuf.Collections.RepeatedField<Person> data = results.Persons;

      foreach (Person person in data)
      {
        foreach (Card card in person.Cards)        
          _cards.UpdateItem(card, card.UniqueNumber, card.EntityState);

        foreach (Photo photo in person.Photos)
          _photos.UpdateItem(photo, photo.Id, photo.EntityState);        

        _persons.UpdateItem(person, person.Id, person.EntityState);
      }
    }

    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }


    public event DataChangedHandler DataChanged;

    private readonly PersonHolder _persons;
    private readonly PhotoHolder  _photos ;
    private readonly CardHolder   _cards  ;
  }
}
