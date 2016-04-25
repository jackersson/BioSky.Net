using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Holders
{
  public interface ICardHolder
  {
    void UpdateFromResponse(Person owner, Card requested, Card responded);
    void UpdateFromResponse(Person owner, IList<Card> requested, IList<Card> responded);

    Person GetPersonByCardNumber(string cardNumber);

    //void Remove(Person owner, Card requested, Card responded);

   // void Remove(Person owner, IList<Card> requested, IList<Card> responded);
  }
}
