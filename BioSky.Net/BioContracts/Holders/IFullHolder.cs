using System.Collections.Generic;

namespace BioContracts.Holders
{
  public delegate void DataUpdatedHandler<T>(T list); 
  public delegate void DataChangedHandler();

  public interface IFullHolder<TValue>
  {
    void Init(Google.Protobuf.Collections.RepeatedField<TValue> list);

    void Add(TValue requested, TValue responded);
    void Remove(TValue requested, TValue responded);
    void Update( TValue requested
               , TValue responded );

    //void Update( Google.Protobuf.Collections.RepeatedField<TValue> requested
          //     , Google.Protobuf.Collections.RepeatedField<TValue> results   );

    AsyncObservableCollection<TValue> Data
    { get; }

    Dictionary<long, TValue> DataSet
    { get; }

    TValue GetValue(long Id);


    event DataChangedHandler                                                     DataChanged;
    event DataUpdatedHandler<Google.Protobuf.Collections.RepeatedField<TValue>>  DataUpdated;
 
  }
}
