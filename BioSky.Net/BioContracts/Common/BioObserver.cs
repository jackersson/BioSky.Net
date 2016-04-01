using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.Common
{
  public class BioObserver<T> : IBioObservable<T>
  {
    public BioObserver()
    {
      _observers = new List<T>();
    }

    public void Subscribe(T observer)
    {
      _observers.Add(observer);
    }

    public void Unsubscribe(T observer)
    {
      _observers.Remove(observer);
    }

    public void UnsubscribeAll()
    {
      _observers.RemoveAll(x => true);
    }

    public bool HasObserver(T observer)
    {
      return _observers.Contains(observer);
    }

    private List<T> _observers;
    public List<T> Observers { get { return _observers; } }
  }
}
