using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IBioObservable<T>
  {
    void Subscribe(T observer);

    void Unsubscribe(T observer);

    void UnsubscribeAll();
  }
}
