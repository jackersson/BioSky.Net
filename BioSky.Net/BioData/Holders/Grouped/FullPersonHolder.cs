using BioContracts;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;

namespace BioData.Holders.Grouped
{
  public class FullPersonHolder : PropertyChangedBase, IFullPersonHolder
  {
    public FullPersonHolder()
    {
      DataSet      = new Dictionary<long, Person>();
      CardsDataSet = new Dictionary<string, Person>();
      Data         = new AsyncObservableCollection<Person>();
    }

    public void Init(Google.Protobuf.Collections.RepeatedField<Person> data)
    {
     
      Data = new AsyncObservableCollection<Person>(data);

      foreach (Person person in data)
      {
        _dataSet.Add(person.Id, person);
        
      }     

      OnDataChanged();      
    }

    public void Update( Google.Protobuf.Collections.RepeatedField<Person> requested
                      , Google.Protobuf.Collections.RepeatedField<Person> results )
    {
     
      bool success = false;
      foreach (Person person in results)
      {    
        /*   
        foreach (Card card in person.Cards)        
          _cards.UpdateItem(card, card.UniqueNumber, card.EntityState, card.Dbresult);

        Photo thumbnail = person.Thumbnail;
        if (thumbnail != null)
          _photos.UpdateItem(thumbnail, thumbnail.Id, thumbnail.EntityState, thumbnail.Dbresult);

        foreach (Photo photo in person.Photos)
          _photos.UpdateItem(photo, photo.Id, photo.EntityState, photo.Dbresult);

        success = person.Dbresult == Result.Success;

        _persons.UpdateItem(person, person.Id, person.EntityState, person.Dbresult);   
        */     
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

      //_photos.CheckPhotos();
    
    }

    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }

    private void OnDataUpdated(Google.Protobuf.Collections.RepeatedField<Person> list)
    {
      if (DataUpdated != null)
        DataUpdated(list);
    }

    private AsyncObservableCollection<Person> _data;
    public AsyncObservableCollection<Person> Data
    {
      get { return _data; }
      private set
      {
        if (_data != value)
        {
          _data = value;
          NotifyOfPropertyChange(() => Data);
        }
      }
    }

    private Dictionary<long, Person> _dataSet;
    public Dictionary<long, Person> DataSet
    {
      get { return _dataSet; }
      private set
      {
        if (_dataSet != value)
          _dataSet = value;
      }
    }

    private Dictionary<string, Person> _cardsDataSet;
    public Dictionary<string, Person> CardsDataSet
    {
      get { return _cardsDataSet; }
      private set
      {
        if (_cardsDataSet != value)
          _cardsDataSet = value;
      }
    }


    public Person GetPersonByCardNumber(string cardNumber)
    {
      Person person = null;
      CardsDataSet.TryGetValue(cardNumber, out person);

      return person;
    }

    public Person GetValue(long Id)
    {
      Person person = null;
      DataSet.TryGetValue(Id, out person);

      return person;
    }

    public event DataChangedHandler             DataChanged;
    public event DataUpdatedHandler<Google.Protobuf.Collections.RepeatedField<Person>> DataUpdated;


    
  }
}
