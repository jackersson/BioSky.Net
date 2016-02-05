using BioContracts;
using BioContracts.Services;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioGRPC.ForeignUpdators
{


  public class UpdatePairRequestWithDependency<ProtoTypeFirst, ProtoTypeSecond>
  {
    public UpdatePairRequestWithDependency( IDatabaseService dataService
                                          , IBioSkyNetRepository database)
    {
      _dataService = dataService;
      _database = database;
    }

    public virtual void BeginRequest(ProtoTypeFirst valueFirst, ProtoTypeSecond valueSecond)
    {
      _protoTypeFirst  = valueFirst;
      _protoTypeSecond = valueSecond;
    }
   

    protected ProtoTypeFirst  _protoTypeFirst ;
    protected ProtoTypeSecond _protoTypeSecond;

    protected readonly IDatabaseService     _dataService;
    protected readonly IBioSkyNetRepository _database   ;

  }
  class UpdatePairRequestVisitoPhoto
  {
    public delegate void UpdateRequestByForeignKeyHandler(Photo photo, Visitor visitor);

    public class UpdateDependencyRequestVisitorPhoto : UpdatePairRequestWithDependency<Photo, Visitor>
    {
      public UpdateDependencyRequestVisitorPhoto( IDatabaseService dataService
                                                 , IBioSkyNetRepository database) 
                                                 : base(dataService, database) 
      {
      
      }

      public override async void BeginRequest(Photo photo, Visitor visitor)
      {
        if (photo == null || visitor == null)
          return;

        base.BeginRequest(photo, visitor);     

        _database.VisitorHolder.DataUpdated += VisitorHolder_DataUpdated;
        _database.PhotoHolder.DataUpdated   += PhotoHolder_DataUpdated;

        await _dataService.PhotoUpdateRequest  (photo  );
        await _dataService.VisitorUpdateRequest(visitor);
      }

      private void VisitorHolder_DataUpdated(IList<Visitor> list, Result result)
      {
        _database.VisitorHolder.DataUpdated -= VisitorHolder_DataUpdated;
      }

      private void PhotoHolder_DataUpdated(IList<Photo> list, Result result)
      {
        _database.PhotoHolder.DataUpdated -= PhotoHolder_DataUpdated;
      }
           
    }
  }
}
