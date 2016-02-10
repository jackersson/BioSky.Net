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
        
        //foreach (Photo photo in person.Photos)
         // _photos.Add(photo, photo.Id);

        /*
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

      bool success = false;
      foreach (Person person in data)
      {       
        foreach (Card card in person.Cards)        
          _cards.UpdateItem(card, card.UniqueNumber, card.EntityState, card.Dbresult);

        Photo thumbnail = person.Thumbnail;
        if (thumbnail != null)
          _photos.UpdateItem(thumbnail, thumbnail.Id, thumbnail.EntityState, thumbnail.Dbresult);

        foreach (Photo photo in person.Photos)
          _photos.UpdateItem(photo, photo.Id, photo.EntityState, photo.Dbresult);

        success = person.Dbresult == ResultStatus.Success;

        _persons.UpdateItem(person, person.Id, person.EntityState, person.Dbresult);        
      }

      if (success)
        OnDataUpdated(results);

      try
      {
        OnDataChanged();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      _photos.CheckPhotos();
    
    }

    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }

    private void OnDataUpdated(PersonList list)
    {
      if (DataUpdated != null)
        DataUpdated(list);
    }


    public event DataChangedHandler             DataChanged;
    public event DataUpdatedHandler<PersonList> DataUpdated;

    private readonly PersonHolder _persons;
    private readonly PhotoHolder  _photos ;
    private readonly CardHolder   _cards  ;
  }
}
