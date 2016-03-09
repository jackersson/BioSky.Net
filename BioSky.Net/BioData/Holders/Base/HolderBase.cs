﻿using BioContracts;
using BioService;
using Caliburn.Micro;
using System.Collections.Generic;

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


    
    public void Update(IList<TValue> list)
    {
      //Data = new AsyncObservableCollection<TValue>(list);
      UpdateDataSet(list);      
    }
    

    protected virtual void UpdateDataSet(IList<TValue> list) {  }

    public virtual void UpdateItem(TValue obj, TKey key, EntityState state, Result result)
    {
      if (result != Result.Success)
        return;

      switch (state)
      {
        case EntityState.Added:
          Add(obj, key);
          break;

        case EntityState.Modified:
          Update(obj, key);
          break;

        case EntityState.Deleted:
          Remove(key);
          break;
      }

      NotifyOfPropertyChange(() => Data);
    }

    public virtual void Add(TValue obj, TKey key)
    {     
      //Data.Add(obj);
      Update(obj, key);
     // AddToDataSet(obj, key);   
    }

    /*
    protected virtual void AddToDataSet(TValue obj, TKey key)
    {
      if (!_dataSet.ContainsKey(key))
        _dataSet.Add(key, obj);
      else
        Update(obj, key);
    }
    */
    public virtual void Update(TValue obj, TKey key)
    {
      if (_dataSet.ContainsKey(key))
      {
        //_dataSet[key] = obj;      
        CopyFrom(obj, _dataSet[key]);   
      }
      else
      {
        Data.Add(obj);
        _dataSet.Add(key, obj);
      }
    }

    protected virtual void CopyFrom(TValue from, TValue to)
    {
      to = from;
    }

    public virtual void Remove(TKey key)
    {
      _dataSet.Remove(key);
    }

    protected void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }

    public virtual TValue GetValue(TKey id)
    {
      TValue value;
      bool valueExists = DataSet.TryGetValue(id, out value);
      return value;
    }
   

    public event DataChangedHandler         DataChanged;
   // public event DataUpdatedHandler<TValue> DataUpdated;
  }
}
