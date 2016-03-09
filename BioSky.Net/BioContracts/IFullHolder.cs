using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IFullHolder<TValue>
  {
    void Init(Google.Protobuf.Collections.RepeatedField<TValue> list);

    void Update( Google.Protobuf.Collections.RepeatedField<TValue> requested
               , Google.Protobuf.Collections.RepeatedField<TValue> results   );

    AsyncObservableCollection<TValue> Data
    { get; }

    Dictionary<long, TValue> DataSet
    { get; }

    TValue GetValue(long Id);


    event DataChangedHandler                                                     DataChanged;
    event DataUpdatedHandler<Google.Protobuf.Collections.RepeatedField<TValue>>  DataUpdated;
 
  }
}
