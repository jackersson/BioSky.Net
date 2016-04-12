using BioContracts;
using BioService;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using BioData.Holders.Utils;
using BioContracts.Holders;

namespace BioData.Holders.Grouped
{
  public class FullPersonHolder : PropertyChangedBase, IFullPersonHolder
  {
    public FullPersonHolder(IOUtils ioutils, PhotoHolder photoHolder)
    {
      DataSet         = new Dictionary<long, Person>();
      CardsDataSet    = new Dictionary<string, long>();
      Data            = new AsyncObservableCollection<Person>();

      _photoHolder = photoHolder;
      _ioUtils     = ioutils;
    }

    public void Init(RepeatedField<Person> data)
    {
      Data = new AsyncObservableCollection<Person>(data);

      foreach (Person person in data)
      {
        long personId = person.Id; 
        _dataSet.Add(person.Id, person);

        foreach (Card card in person.Cards)
          _cardsDataSet.Add(card.UniqueNumber, personId);

        _photoHolder.CheckPhotosIfFileExisted(person);
      }
     
      OnDataChanged();
    }

    public void Add(Person requested, Person responded)
    {
      if (responded.Dbresult == Result.Success && !_dataSet.ContainsKey(responded.Id))
      {
        requested.Id = responded.Id;
        Data.Add(requested);
        _dataSet.Add(requested.Id, requested);

        if (responded.Thumbnail != null)
        {
          requested.Thumbnail.Id       = responded.Thumbnail.Id;        
          responded.Thumbnail.Dbresult = Result.Success       ;

          AddPhoto(requested, requested.Thumbnail, responded.Thumbnail, false);
          SetThumbnail(requested, requested.Thumbnail, new Response() { Good = Result.Success }, false);
        }

        OnDataChanged();
      }      
    }

    public void Update( Person requested
                      , Person responded )
    {
      if (responded.Dbresult == Result.Success)
      {
        Person oldItem = GetValue(requested.Id);

        if (oldItem != null)         
          CopyFrom(responded, oldItem);              
      }     
    }

    private void CopyFrom(Person from, Person to)
    {
      if (from.Firstname != "")
        to.Firstname = from.Firstname;

      if (from.Lastname != "")
        to.Lastname = from.Lastname;

      Console.WriteLine(_dataSet);
    }

    public void Remove( Person requested
                      , Person responded)
    {
      if (responded.Dbresult == Result.Success)
      {
        _dataSet.Remove(requested.Id);
        var item = Data.Where(x => x.Id == requested.Id).FirstOrDefault();
        if (item != null)        
          Data.Remove(item);
        
      }
    }
    
    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }

    private void OnDataUpdated(RepeatedField<Person> list)
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

    private Dictionary<string, long> _cardsDataSet;
    public Dictionary<string, long> CardsDataSet
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
      long personid;
      CardsDataSet.TryGetValue(cardNumber, out personid);

      Person person = null;
      DataSet.TryGetValue(personid, out person);

      return person;
    }

    public Person GetValue(long Id)
    {
      Person person = null;
      DataSet.TryGetValue(Id, out person);

      return person;
    }

    public Person GetValue(Person person)
    {
      if (person == null)
        return null;

      Person response = Data.Where(x => x.Firstname == person.Firstname && x.Lastname == person.Lastname).FirstOrDefault();    
      return response;
    }

    public void RemoveCards(Person owner, RepeatedField<long> requested, RepeatedField<long> responsed)
    {
      foreach (long index in responsed)
      {
        RepeatedField<Card> ownerCards = _dataSet[owner.Id].Cards;
        IEnumerable<Card> cards = ownerCards.Where(x => responsed.Contains(x.Id));
        foreach (Card card in cards)        
          ownerCards.Remove(card);      
      }
      OnDataChanged();    
    }

    public void AddCard(Person owner, Card requested, Card responsed)
    {
      if ( responsed.Dbresult == Result.Success )
      {
        requested.Id = responsed.Id;
        _dataSet[owner.Id].Cards.Add(requested);
        
        OnDataChanged();
      }      
    }

    public void AddPhoto(Person owner, Photo requested, Photo responsed, bool refresh = true)
    {
      if (responsed.Dbresult == Result.Success)
      {
        requested.Id       = responsed.Id;
        requested.PhotoUrl = responsed.PhotoUrl;
        requested.Personid = owner.Id;

        _dataSet[owner.Id].Photos.Add(requested);

        _ioUtils.SaveFile(requested.PhotoUrl, requested.Bytestring.ToArray());

        requested.Bytestring = Google.Protobuf.ByteString.Empty;

        if (refresh)
          OnDataChanged();
      }  
    }

    public void SetThumbnail(Person owner, Photo requested, Response responsed, bool refresh = true)
    {
      if (responsed.Good == Result.Success)
      {        
        Photo newThumbnail = _dataSet[owner.Id].Photos.Where(x => x.Id == requested.Id).FirstOrDefault();
        if (newThumbnail != null)
        {
          _dataSet[owner.Id].Photoid   = requested.Id;
          _dataSet[owner.Id].Thumbnail = newThumbnail;
        }
              
        if (refresh)
          OnDataChanged();
      }
    }   

    public void RemovePhotos(Person owner, RepeatedField<long> requested, RepeatedField<long> responsed)
    {
      foreach (long index in responsed)
      {
        RepeatedField<Photo> ownerPhotos = _dataSet[owner.Id].Photos;
        IEnumerable<Photo> cards = ownerPhotos.Where(x => responsed.Contains(x.Id));
        foreach (Photo photo in cards)
          ownerPhotos.Remove(photo);
      }
      OnDataChanged();
    }
    
    public event DataChangedHandler             DataChanged;
    public event DataUpdatedHandler<RepeatedField<Person>> DataUpdated;

    public readonly PhotoHolder _photoHolder;
    public readonly IOUtils     _ioUtils    ;
    
  }
}
