using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BioGRPC.DatabaseClient
{

  public class PersonDataClient : IDataClient<Person, QueryPersons>, IThumbnailDataClient
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
  
    public async Task Select(QueryPersons command)
    {
      try
      {
        PersonList call = await _client.PersonSelectAsync(command);
       
        Task task = new Task(() => _database.Persons.Init(call.Persons));
        task.Start();
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task SetThumbnail(Person owner, Photo requested)
    {
      if (requested == null || owner == null)
        return;

      try
      {
        requested.Personid = owner.Id;
        Response responded = await _client.SetThumbnailAsync(requested);
        Console.WriteLine(responded);
        _database.Persons.SetThumbnail(owner, requested, responded);
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
  
      try
      {
        Person newPerson = await _client.AddPersonAsync(item);
        Console.WriteLine(newPerson);
        _database.Persons.Add(item, newPerson);
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
     
      try
      {
        Person udatedPerson = await _client.UpdatePersonAsync(item);
        Console.WriteLine(item);
        _database.Persons.Update(item, udatedPerson);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    } 
  
    public async Task Remove(IList<Person> targeIds)
    {
      if (targeIds == null || targeIds.Count <= 0)
        return;
  
  
      _list.Persons.Clear();
  
 
      foreach (Person item in targeIds)
      {       
        Person newItem = new Person()
        {
            Id = item.Id
         // , EntityState = EntityState.Deleted
          //, Dbresult = ResultStatus.Failed
        };
        _list.Persons.Add(newItem);       
      }
  
      if (_list.Persons.Count <= 0)
        return;
  
      try
      {
        //await Update(_list);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }    

    public async Task Remove(Person item)
    {
      if (item == null)
        return;

      try
      {
        Person deletedPerson = await _client.RemovePersonAsync(item);
        Console.WriteLine(deletedPerson);
        _database.Persons.Remove(item, deletedPerson);
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
