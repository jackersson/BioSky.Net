using BioData.Holders.Base;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders
{
  public class PersonHolder : HolderBase<Person, long>
  {
    public PersonHolder() : base() { }

    protected override void UpdateDataSet(IList<Person> list)
    {
      foreach (Person person in list)
        Update(person, person.Id);
    }

    protected override void CopyFrom(Person from, Person to)
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
