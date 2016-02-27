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
  public class PersonDataClient : IDataClient<Person, CommandPersons>
  {
    public PersonDataClient(IProcessorLocator locator
                             , BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client)
    {
      _client = client;
      _locator = locator;
  
      _database = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier = _locator.GetProcessor<INotifier>();
  
      _list = new PersonList();
    }
  
    private async Task Update(PersonList list)
    {
      try
      {
        PersonList call = await _client.PersonUpdateAsync(list);
        _database.Persons.Update(list, call);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
  
    public async Task Select(CommandPersons command)
    {
      try
      {
        PersonList call = await _client.PersonSelectAsync(command);
        _database.Persons.Init(call);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
  
    public async Task Add(Person item)
    {
      if (item == null)
        return;
  
      _list.Persons.Clear();
  
      //TODO ResultStatus None 
      item.Dbresult = ResultStatus.Failed;
      item.EntityState = EntityState.Added;
      _list.Persons.Add(item);
  
      try
      {
        await Update(_list);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
  
    public async Task Update(Person item)
    {
      if (item == null)
        return;
  
      _list.Persons.Clear();
  
      //TODO ResultStatus None 
      item.Dbresult = ResultStatus.Failed;
      item.EntityState = EntityState.Modified;
      _list.Persons.Add(item);
  
      try
      {
        await Update(_list);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
  
  
  
    public async Task Delete(IList<long> targeIds)
    {
      if (targeIds == null || targeIds.Count <= 0)
        return;
  
  
      _list.Persons.Clear();
  
      Dictionary<long, Person> dictionary = _database.PersonHolder.DataSet;
      foreach (long id in targeIds)
      {
        Person item = null;
        if (dictionary.TryGetValue(id, out item))
        {
          Person newItem = new Person()
          {
            Id = id
            ,
            EntityState = EntityState.Deleted
            ,
            Dbresult = ResultStatus.Failed
          };
          _list.Persons.Add(newItem);
        }
      }
  
      if (_list.Persons.Count <= 0)
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
  
    private PersonList _list;
  
    private readonly IProcessorLocator _locator;
    private readonly IBioSkyNetRepository _database;
    private readonly INotifier _notifier;
    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
