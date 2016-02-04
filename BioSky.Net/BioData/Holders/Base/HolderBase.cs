using BioContracts;
using BioFaceService;
using Caliburn.Micro;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders.Base
{
  public class HolderBase<TValue, TKey> : PropertyChangedBase, IHolder<TValue, TKey>
  {    
    public HolderBase()
    {
      _data    = new AsyncObservableCollection<TValue>();
      _dataSet = new Dictionary<TKey, TValue>();
    }

    private AsyncObservableCollection<TValue> _data;
    public AsyncObservableCollection<TValue> Data
    {
      get { return _data; }
      private set
      {
        if (_data != value)
        {
          _data = value;
          NotifyOfPropertyChange(() => Data);
        }
      }
    }

    private Dictionary<TKey, TValue> _dataSet;
    public Dictionary<TKey, TValue> DataSet
    {
      get { return _dataSet; }
      private set
      {
        if (_dataSet != value)
          _dataSet = value;
      }
    }


    public virtual void Update(IList<TValue> list, Result result)
    {
      Console.WriteLine("On update");  
      OnDataChanged();
      OnDataUpdated(list, result);
    }

    public void Update(IList<TValue> list)
    {
      Data = new AsyncObservableCollection<TValue>(list);
      UpdateDataSet(list);
      OnDataChanged();

      Console.WriteLine(Data.Count + " " + DataSet.Count);
    }

    protected virtual void UpdateDataSet(IList<TValue> list) {  }

    protected void UpdateItem(TValue obj, TKey key, DbState state)
    {      
      switch (state)
      {
        case DbState.Insert:
          Add(obj, key);
          break;

        case DbState.Update:
          Update(obj, key);
          break;

        case DbState.Remove:
          Remove(obj, key);
          break;
      }

      NotifyOfPropertyChange(() => Data);
    }

    protected virtual void Add(TValue obj, TKey key)
    {     
      Data.Add(obj);
      AddToDataSet(obj, key);   
    }

    protected virtual void AddToDataSet(TValue obj, TKey key)
    {
      if (!_dataSet.ContainsKey(key))
        _dataSet.Add(key, obj);
    }

    protected virtual void Update(TValue obj, TKey key)
    {      
      if (_dataSet.ContainsKey(key))
        _dataSet[key] = obj;
    }

    protected virtual void Remove(TValue obj, TKey key)
    {
      _dataSet.Remove(key);
      Data.Remove(obj);
    }

    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }

    protected void OnDataUpdated(IList<TValue> list, Result result)
    {
      if (DataUpdated != null)
        DataUpdated(list, result);
    }

    public event DataChangedHandler         DataChanged;
    public event DataUpdatedHandler<TValue> DataUpdated;
  }
}
