﻿using BioData.Holders.Base;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders
{
  public class CardHolder : HolderBase<Card, string>
  {
    public CardHolder() : base() { }

    protected override void UpdateDataSet(IList<Card> list)
    {
      foreach (Card card in list)
        Update(card, card.UniqueNumber);        
    }

    public override void Remove(string key)
    {
      base.Remove(key);
      var item = Data.Where(x => x.UniqueNumber == key).FirstOrDefault();
      if (item != null)
      {
        Data.Remove(item);
      }
    }

    protected override void CopyFrom(Card from, Card to)
    {
      to.MergeFrom(from);
    }

    public Card GetValue(string id)
    {
      Card card = null;
      bool cardExists = DataSet.TryGetValue(id, out card);
      return card;
    }

    /*
    public override void Update(IList<Card> list, Result result)
    {
      foreach (ResultPair currentResult in result.Status)
      {
        Card card = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
            card = currentResult.Card;
          else
            card = list.Where(x => x.Id == currentResult.Id).FirstOrDefault();

          if (card != null)
          {
            card.Dbstate = DbState.None;
            UpdateItem(card, card.UniqueNumber, currentResult.State);
          }
        }
      }
      base.Update(list, result);
    }
    */

  }
}
