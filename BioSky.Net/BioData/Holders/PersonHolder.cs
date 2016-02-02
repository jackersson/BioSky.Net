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

    public override void Update(IList<Person> list, Result result)
    {     
      foreach (ResultPair currentResult in result.Status)
      {
        Person person = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)          
            person = currentResult.Person;          
          else
            person = list.Where(x => x.Id == currentResult.Id).FirstOrDefault();

          if (person != null)
          {
            person.Dbstate = DbState.None;
            UpdateItem(person, person.Id, currentResult.State);
          }
        }            
      }
    
      base.Update(list, result);
    }

    

  }
}
