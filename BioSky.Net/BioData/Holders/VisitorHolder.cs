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

    public override void Remove(long key)
    {
      base.Remove(key);
      var item = Data.Where(x => x.Id == key).FirstOrDefault();
      if (item != null)
      {
        Data.Remove(item);
      }
    }

  }  
}
