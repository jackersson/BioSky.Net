using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BioGRPC.DatabaseClient
{ 
  public class VisitorDataClient : IDataClient<Visitor, QueryVisitors>
  {
    public VisitorDataClient( IProcessorLocator locator
                            , BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client )
    {
      _client   = client;
      _locator  = locator;

      _database = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier = _locator.GetProcessor<INotifier>();
      _uiDispatcher = _locator.GetProcessor<Dispatcher>();
      _list = new VisitorList();
    }

    private async Task Update(VisitorList list)
    {
      try
      {
        //VisitorList call = await _client.VisitorUpdateAsync(list);
        //_database.Visitors.Update(list, call);       
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Select(QueryVisitors command)
    {
      try
      {
         VisitorList call = await _client.SelectVisitorsAsync(command);
        _database.Visitors.Init(call.Visitors);        
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

      try
      {
        Visitor newVisitor = await _client.AddVisitorAsync(item);
        Console.WriteLine(newVisitor);
        _uiDispatcher.Invoke( () => _database.Visitors.Add(item, newVisitor) );
      }
      catch (RpcException e) {
        _notifier.Notify(e);
      }      
    }

    public async Task Update(Visitor item)
    {
      if (item == null)
        return;

      _list.Visitors.Clear();

      //TODO ResultStatus None 
      //item.Dbresult    = ResultStatus.Failed;
      //item.EntityState = EntityState.Modified;
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

    public async Task Remove( IList<Visitor> targetItems)
    {
      if (targetItems == null || targetItems.Count <=0 )
        return;


      _list.Visitors.Clear();

    
      foreach (Visitor item in targetItems)
      {         
         Visitor newItem = new Visitor()
         {
             Id = item.Id
           //, EntityState = EntityState.Deleted
           //, Dbresult    = ResultStatus.Failed
           , Photoid = item.Photoid
         };
         _list.Visitors.Add(newItem);        
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

    public Task Remove(Visitor targetItem)
    {
      throw new NotImplementedException();
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

    private readonly Dispatcher           _uiDispatcher;
    private readonly IProcessorLocator    _locator   ;
    private readonly IBioSkyNetRepository _database  ;    
    private readonly INotifier            _notifier  ;
    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
