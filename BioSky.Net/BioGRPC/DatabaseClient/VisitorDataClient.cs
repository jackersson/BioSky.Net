using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioGRPC.DatabaseClient
{ 
  public class VisitorDataClient : IDataClient<Visitor, CommandVisitors>
  {
    public VisitorDataClient( IProcessorLocator locator
                            , BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client )
    {
      _client   = client;
      _locator  = locator;

      _database = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier = _locator.GetProcessor<INotifier>();

      _list = new VisitorList();
    }

    private async Task Update(VisitorList list)
    {
      try
      {
        VisitorList call = await _client.VisitorUpdateAsync(list);
        _database.Visitors.Update(list, call);       
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Select(CommandVisitors command)
    {
      try
      {
        VisitorList call = await _client.VisitorSelectAsync(command);
        _database.Visitors.Init(call);        
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Add(Visitor item)
    {
      if (item == null)
        return;

      _list.Visitors.Clear();

      //TODO ResultStatus None 
      item.Dbresult    = ResultStatus.Failed;
      item.EntityState = EntityState.Added  ;
      _list.Visitors.Add(item);    

      try
      {
        await Update(_list);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Update(Visitor item)
    {
      if (item == null)
        return;

      _list.Visitors.Clear();

      //TODO ResultStatus None 
      item.Dbresult    = ResultStatus.Failed;
      item.EntityState = EntityState.Modified;
      _list.Visitors.Add(item);

      try
      {
        await Update(_list);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }



    public async Task Delete( IList<long> targeIds)
    {
      if (targeIds == null || targeIds.Count <=0 )
        return;


      _list.Visitors.Clear();

      Dictionary<long, Visitor> dictionary = _database.VisitorHolder.DataSet;
      foreach (long id in targeIds)
      {       
        Visitor item = null;      
        if (dictionary.TryGetValue(id, out item))
        {
          Visitor newItem = new Visitor()
          {
              Id = id
            , EntityState = EntityState.Deleted
            , Dbresult    = ResultStatus.Failed
            , Photoid = item.Photoid
          };
          _list.Visitors.Add(newItem);
        }
      }

      if (_list.Visitors.Count <= 0)
        return;

      try
      {
        await Update(_list);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    //_database.Visitors.DataUpdated += UpdateData;

        /*
    private void UpdateData(VisitorList list)
    {
      _database.Visitors.DataUpdated -= UpdateData;

      if (list != null)
      {
        Visitor visitor = list.Visitors.FirstOrDefault();
        if (visitor != null)
        {
          if (visitor.EntityState == EntityState.Deleted)
          {
            if (list.Visitors.Count > 1)
              MessageBox.Show(list.Visitors.Count + " visitors successfully Deleted");
            else
              MessageBox.Show("Visitor successfully Deleted");
          }
        }
      }
    }   
    */

    private VisitorList _list;

    private readonly IProcessorLocator    _locator   ;
    private readonly IBioSkyNetRepository _database  ;    
    private readonly INotifier            _notifier  ;
    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
