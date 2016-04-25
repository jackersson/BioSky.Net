using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public interface IOwnerDataClient<TOwnerType, TValueType>
  {
    Task Add( long ownerID, TValueType value);

    //Task Update(TOwnerType owner, TValueType item);

    Task Remove(long ownerID, TValueType item);

    Task Remove(long ownerID, IList<TValueType> items);
  }
}
