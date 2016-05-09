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
    public PersonDataClient(IProcessorLocator locator )
    {
     
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
        //requested.Personid = owner.Id;
        Response responded = await _client.SetThumbnailAsync(requested);
        Console.WriteLine(responded);
        _database.Persons.SetThumbnail(owner, requested, responded);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public async Task Add(Person requested)
    {
      if (requested == null)
        return;
  
      try
      {
        Person responded = await _client.AddPersonAsync(requested);
        Console.WriteLine(responded);
        _database.Persons.Add(requested, responded);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }    

    public async Task Update(Person requested)
    {
      if (requested == null)
        return;  
     
      try
      {
        Person responded = await _client.UpdatePersonAsync(requested);
        Console.WriteLine(requested);
        _database.Persons.Update(requested, responded);
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
      Person requested = new Person() { Id = item.Id };
      try
      {

        Person response = await _client.RemovePersonAsync(requested);
        Console.WriteLine(response);
        _database.Persons.Remove(requested, response);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public void Update(BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client)
    {
      _client = client;
    }

    private PersonList _list;
  
    private readonly IProcessorLocator _locator;
    private readonly IBioSkyNetRepository _database;
    private readonly INotifier _notifier;
    private BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
