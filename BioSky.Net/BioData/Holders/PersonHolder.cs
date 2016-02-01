using BioData.Holders.Base;
using BioFaceService;
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
        AddToDataSet(person, person.Id);
    }

  }
}
