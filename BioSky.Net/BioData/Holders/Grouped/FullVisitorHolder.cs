using BioContracts;
using BioService;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BioData.Holders.Grouped
{
  public class FullVisitorHolder : PropertyChangedBase, IFullHolder<Visitor>
  {
    public FullVisitorHolder(  )
    {
      DataSet = new Dictionary<long, Visitor>();
      Data = new AsyncObservableCollection<Visitor>();
    }


    public void Init(Google.Protobuf.Collections.RepeatedField<Visitor> list)
    {
     ;

      //foreach (Visitor visitor in data)      
       // _visitors.Add(visitor, visitor.Id);      

      OnDataChanged();
    }

    public void Update( Google.Protobuf.Collections.RepeatedField<Visitor> updated
                      , Google.Protobuf.Collections.RepeatedField<Visitor> results)
    {
      bool success = false;
      foreach (Visitor visitor in updated)
      {
        Visitor resultedVisitor = results.FirstOrDefault();
        Visitor updatedVisitor = new Visitor(visitor);
        if (resultedVisitor != null)
          updatedVisitor.Id = resultedVisitor.Id;

        /*
        if(resultedVisitor.EntityState != EntityState.Deleted)
        {
          Photo resultedPhoto = resultedVisitor.Photo;
          Photo updatedPhoto = new Photo(updatedVisitor.Photo);
          if (resultedPhoto != null && updatedPhoto != null)
          {
            updatedPhoto.Id = resultedPhoto.Id;

            updatedPhoto.FileLocation = resultedPhoto.FileLocation;
            updatedPhoto.FirLocation = resultedPhoto.FirLocation;

            _photos.UpdateItem(updatedPhoto, updatedPhoto.Id, updatedPhoto.EntityState, updatedVisitor.Dbresult);

            updatedPhoto.EntityState = EntityState.Unchanged;
            updatedPhoto.Dbresult = ResultStatus.Success;
          }
        }
        else 
        {         
          _photos.Remove(resultedVisitor.Photoid);          
        }

        
        _visitors.UpdateItem(updatedVisitor, updatedVisitor.Id, updatedVisitor.EntityState, updatedVisitor.Dbresult);
        visitor.EntityState = EntityState.Unchanged;
        visitor.Dbresult = ResultStatus.Success;
        success = visitor.Dbresult == ResultStatus.Success;
        */
      }

      if (success)
        OnDataUpdated(results);


      OnDataChanged();
    }

    public Visitor GetValue(long Id)
    {
      Visitor visitor = null;
      DataSet.TryGetValue(Id, out visitor);

      return visitor;
    }

    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }
    private void OnDataUpdated(Google.Protobuf.Collections.RepeatedField<Visitor> list)
    {
      if (DataUpdated != null)
        DataUpdated(list);
    }

    public void Add(Visitor requested, Visitor responded)
    {
      throw new NotImplementedException();
    }

    public void Update(Visitor requested, Visitor responded)
    {
      throw new NotImplementedException();
    }

    public void Remove(Visitor requested, Visitor responded)
    {
      throw new NotImplementedException();
    }

    private AsyncObservableCollection<Visitor> _data;
    public AsyncObservableCollection<Visitor> Data
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

    public event DataChangedHandler DataChanged;
    public event DataUpdatedHandler<Google.Protobuf.Collections.RepeatedField<Visitor>> DataUpdated;
    
    
  }
}
