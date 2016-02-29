using BioContracts;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData.Holders.Grouped
{
  public class FullVisitorHolder : IFullHolder<VisitorList>
  {


    public FullVisitorHolder( VisitorHolder visitors
                            , PhotoHolder   photos )
    {
      _visitors = visitors;
      _photos   = photos  ;      
    }


    public void Init(VisitorList list)
    {
      Google.Protobuf.Collections.RepeatedField<Visitor> data = list.Visitors;

      foreach (Visitor visitor in data)      
        _visitors.Add(visitor, visitor.Id);      

      OnDataChanged();
    }

    public void Update(VisitorList updated, VisitorList results)
    {
      Google.Protobuf.Collections.RepeatedField<Visitor> data   = updated.Visitors;
      Google.Protobuf.Collections.RepeatedField<Visitor> result = results.Visitors;

      bool success = false;
      foreach (Visitor visitor in data)
      {
        Visitor resultedVisitor = result.FirstOrDefault();
        Visitor updatedVisitor = new Visitor(visitor);
        if (resultedVisitor != null)
          updatedVisitor.Id = resultedVisitor.Id;

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
      }

      if (success)
        OnDataUpdated(results);


      OnDataChanged();
    }

    private void OnDataChanged()
    {
      if (DataChanged != null)
        DataChanged();
    }
    private void OnDataUpdated(VisitorList list)
    {
      if (DataUpdated != null)
        DataUpdated(list);
    }


    public event DataChangedHandler DataChanged;
    public event DataUpdatedHandler<VisitorList> DataUpdated;

    private readonly VisitorHolder _visitors;
    private readonly PhotoHolder   _photos  ;
    
  }
}
