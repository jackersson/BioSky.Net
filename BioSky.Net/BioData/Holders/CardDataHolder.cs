using BioContracts.Holders;
using BioService;
using System.Collections.Generic;
using System.Linq;

namespace BioData.Holders
{

  public interface IPersonGetAble
  {
    Person GetValue(long id);

    void OnDataChanged();
  }

  internal class CardDataHolder : ICardHolder
  {
     public CardDataHolder(IPersonGetAble personHolder)
    {
      _personHolder = personHolder;

      DataSet = new Dictionary<string, long>();
    }

    public void UpdateFromResponse(Person owner, IList<Card> requested, IList<Card> responded)
    {
      foreach (Card respondedCard in responded )      
        UpdateFromResponse(owner, null, respondedCard);      
    }

    public void UpdateFromResponse(Person owner, Card requested, Card responded)
    {    
      if (responded == null || responded.Dbresult != Result.Success)
        return;
      
      switch (responded.EntityState)
      {
        case EntityState.Added:
          Card card = new Card() { Id = responded.Id, UniqueNumber = requested.UniqueNumber };
          owner.Cards.Add(card);
          Add(owner.Id, card);
          break;

        case EntityState.Deleted:
          Remove(requested);
          Card existingCard = owner.Cards.Where(x => x.Id == responded.Id).FirstOrDefault();
          if (existingCard != null)
            owner.Cards.Remove(existingCard);
          break;
      }

      _personHolder.OnDataChanged();
      
    }

    private void Remove(Card card)
    {
      if (card == null)
        return;

      DataSet.Remove(card.UniqueNumber);
    }

    public void Add(long personID, Card card)
    {
      if (card == null)
        return;

      string cardNumber = card.UniqueNumber;
      if (!ContainesKey(cardNumber))
        DataSet.Add(cardNumber, personID);
      else
      {
        Person person = _personHolder.GetValue(DataSet[cardNumber]);
        Card item = person.Cards.Where(x => string.Equals(cardNumber, x.UniqueNumber)).FirstOrDefault();
        if (item != null)
          person.Cards.Remove(item);      
        DataSet[cardNumber] = personID;
      }
    }

    public void Add(long personId, IEnumerable<Card> cards)
    {
      foreach (Card card in cards)
        Add(personId, card);

      
    }

    private bool ContainesKey(string key)
    {
      long result;
      return DataSet.TryGetValue(key, out result);
    }

    public Person GetPersonByCardNumber(string cardNumber)
    {
      long personid;
      if (!DataSet.TryGetValue(cardNumber, out personid))
        return null;

      return _personHolder.GetValue(personid);
    }
    
    private Dictionary<string, long> _dataSet;
    public Dictionary<string, long> DataSet
    {
      get { return _dataSet; }
      private set
      {
        if (_dataSet != value)
          _dataSet = value;
      }
    }

    private readonly IPersonGetAble _personHolder;
  }
}
