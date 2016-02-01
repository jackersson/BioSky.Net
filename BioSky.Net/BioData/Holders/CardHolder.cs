using BioData.Holders.Base;
using BioFaceService;
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
        AddToDataSet(card, card.UniqueNumber);        
    } 

  }
}
