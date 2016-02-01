using BioContracts;
using BioData.Holders.Base;
using BioFaceService;
using Caliburn.Micro;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders
{

  public class VisitorHolder : HolderBase<Visitor, long>
  {
    public VisitorHolder() : base() { }

    protected override void UpdateDataSet(IList<Visitor> list)
    {
      foreach (Visitor visitor in list)
        AddToDataSet(visitor, visitor.Id);
    }

  }
  /*

  //public delegate void VisitorHolderChanged(string message);
  public class VisitorHolde :  //: PropertyChangedBase, IHolder<Visitor, long>
  {
    public VisitorHolder()
    {
      _data = new ObservableCollection<IMessage>();
      _dataSet = new Dictionary<long, Visitor>();
    }
    
    private ObservableCollection<IMessage> _data;
    public ObservableCollection<IMessage> Data
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

    private Dictionary<long, Visitor> _dataSet;
    public Dictionary<long, Visitor> DataSet
    {
      get { return _dataSet; }
      private set
      {
        if (_dataSet != value)        
          _dataSet = value;             
      }
    }
    
    
    public void Update(IEnumerable list, Result result)
    {    
      foreach (ResultPair currentResult in result.Status)
      {
        Visitor visitor = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
            visitor = currentResult.Visitor;

          //foreach (Visitor v in list)
          //{
          //  list.
         // }
          //else
          //visitor = list.Where(x => x.Id == currentResult.Id).FirstOrDefault();

          UpdateItem(visitor);
        }
        else        
          Console.WriteLine("Failed to update");      
      }     
    }  
    
    public void Update(IList<Visitor> list)
    {
      Data = new ObservableCollection<IMessage>(list);

      foreach (Visitor pers in list)
      {        
        if (!_dataSet.ContainsKey(pers.Id))
          _dataSet.Add(pers.Id, pers);
      }
    }    
  
    private void Add(Visitor visitor)
    {
      long id = visitor.Id;
      Data.Add(visitor);
      if (!_dataSet.ContainsKey(id))
        _dataSet.Add(id, visitor);
    }

    private void Update(Visitor visitor)
    {
      long id = visitor.Id;
      if (_dataSet.ContainsKey(id))
        _dataSet[id] = visitor;
    }

    private void Remove(Visitor visitor)
    {
      _dataSet.Remove(visitor.Id);
      Data.Remove(visitor);
    }

    private void UpdateItem(Visitor visitor)
    {
      switch (visitor.Dbstate)
      {
        case DbState.Insert:
          Add(visitor);
          break;

        case DbState.Update:
          Update(visitor);
          break;

        case DbState.Remove:
          Remove(visitor);
          break;
      }

      NotifyOfPropertyChange(() => Data);
    }
    */
  //}
}
