using BioFaceService;
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
  public delegate void DataChangedHandler();
  public interface IHolder<TValue, TKey>
  {
    event DataChangedHandler DataChanged;

    ObservableCollection<TValue> Data
    { get; }

    Dictionary<TKey, TValue> DataSet
    { get; }

    void Update(IList<TValue> objects);

    void Update(IEnumerable objects, Result result);
  }

}
