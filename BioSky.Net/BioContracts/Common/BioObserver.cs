using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BioContracts.Common
{
  public class BioObserver<T> : IBioObservable<T>
  {
    public BioObserver()  {
      _observers = new ConcurrentDictionary<int, T>();
    }

    public void Subscribe(T observer)
    {
      Task tsk = Task.Run(() => {
        _observers.TryAdd(observer.GetHashCode(), observer);
      });
      tsk.Wait();
    }

    public void Unsubscribe(T observer)
    {
      Task tsk = Task.Run(() => {
        T removed;
        _observers.TryRemove(observer.GetHashCode(), out removed);
      });
      tsk.Wait();
    }

    public void UnsubscribeAll()
    {
      Task tsk = Task.Run(() =>
      {
        foreach (KeyValuePair<int, T> observer in _observers)
        {
          T removed;
          _observers.TryRemove(observer.Value.GetHashCode(), out removed);
        }
      });
      tsk.Wait();
    }

    public bool HasObserver(T observer) {
      return _observers.ContainsKey(observer.GetHashCode());
    }

    private ConcurrentDictionary<int, T> _observers;    
    public ConcurrentDictionary<int, T> Observers { get { return _observers; } }
  }
}
