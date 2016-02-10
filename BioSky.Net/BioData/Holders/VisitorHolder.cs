using BioContracts;
using BioData.Holders.Base;
using BioService;
using Caliburn.Micro;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders
{

  public class VisitorHolder : HolderBase<Visitor, long>
  {
    public VisitorHolder() : base() { }

    protected override void UpdateDataSet(IList<Visitor> list)
    {
      foreach (Visitor visitor in list)
        Update(visitor, visitor.Id);
    }

    protected override void CopyFrom(Visitor from, Visitor to)
    {
      to.MergeFrom(from);
    }

    public override void Remove(Visitor obj, long key)
    {
      base.Remove(obj, key);
      var item = Data.Where(x => x.Id == obj.Id).FirstOrDefault();
      if (item != null)
      {
        Data.Remove(item);
      }
    }
    /*

    public override void Update(IList<Visitor> list, Result result)
    {
      foreach (ResultPair currentResult in result.Status)
      {
        Visitor visitor = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
            visitor = currentResult.Visitor;
          else
            visitor = list.Where(x => x.Id == currentResult.Id).FirstOrDefault();

          if (visitor != null)
          {
            visitor.Dbstate = DbState.None;
            UpdateItem(visitor, visitor.Id, currentResult.State);
          }
        }
      }
      base.Update(list, result);
    }
    */
  }  
}
