using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public interface IDataClient<TValue, TCommand>
  {
    Task Select(TCommand command);

    Task Add(TValue dataItem);

    Task Update(TValue dataItem);

    Task Remove(IList<TValue> targetItems);

    Task Remove(TValue targetItem);
  }
}
