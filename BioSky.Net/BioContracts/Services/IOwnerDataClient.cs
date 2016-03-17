using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public interface IOwnerDataClient<TOwnerType, TValueType>
  {
    Task Add(TOwnerType owner, TValueType value);

    //Task Update(TOwnerType owner, TValueType item);

    Task Remove(TOwnerType owner, TValueType item);

    Task Remove(TOwnerType owner, IList<TValueType> items);
  }
}
