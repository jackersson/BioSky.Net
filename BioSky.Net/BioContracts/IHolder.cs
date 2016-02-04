using BioService;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public delegate void DataUpdatedHandler<T>( IList<T> list, Result result);
  public delegate void DataChangedHandler();
  public interface IHolder<TValue, TKey>
  {
    event DataUpdatedHandler<TValue> DataUpdated;
    event DataChangedHandler         DataChanged;

    AsyncObservableCollection<TValue> Data
    { get; }

    Dictionary<TKey, TValue> DataSet
    { get; }

    void Update(IList<TValue> objects);

    void Update(IList<TValue> objects, Result result);
  }

}
