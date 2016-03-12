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
  public class PersonDataClient : IDataClient<Person, QueryPersons>
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
        //PersonList call = await _client.PersonUpdateAsync(list);
       // Console.Write(call);
        //Console.WriteLine();
        //_database.Persons.Update(list, call);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
  
    public async Task Select(QueryPersons command)
    {
      try
      {
        PersonList call = await _client.PersonSelectAsync(command);
        Console.WriteLine(call);       
        _database.Persons.Init(call.Persons);
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
        Console.WriteLine(item);
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
  
      _list.Persons.Clear();
  
      //TODO ResultStatus None 
      //item.Dbresult = ResultStatus.Failed;
      //item.EntityState = EntityState.Modified;
      _list.Persons.Add(item);
  
      try
      {
        //await AddPerson(_list);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
  
  
  
    public async Task Delete(IList<Person> targeIds)
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
        await Update(_list);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }    

    public Task Delete(Person targetItem)
    {
      throw new NotImplementedException();
    }

    /*
    public async Task UsersDeletePerformer(EntityState state)
    {
      PersonList personList = new PersonList();

      foreach (long id in SelectedItemIds)
      {
        Person person = new Person() { Id = id, EntityState = EntityState.Deleted };
        personList.Persons.Add(person);
      }

      try
      {
        _database.Persons.DataUpdated += UpdateData;
        await _bioService.DatabaseService.PersonUpdate(personList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }
    }

    private void UpdateData(PersonList list)
    {
      _database.Persons.DataUpdated -= UpdateData;

      if (list != null)
      {
        Person person = list.Persons.FirstOrDefault();
        if (person != null)
        {
          if (person.EntityState == EntityState.Deleted)
          {
            if (list.Persons.Count > 1)
              MessageBox.Show(list.Persons.Count + " users successfully Deleted");
            else
              MessageBox.Show("User successfully Deleted");
          }
        }
      }
    }
    */

    /*
  private void UpdateData(PersonList list)
  {
    _database.Persons.DataUpdated -= UpdateData;

    if (list != null)
    {
      Person person = list.Persons.FirstOrDefault();
      if (person != null)
      {
        if (person.Cards.Count > 1)
          MessageBox.Show(person.Cards.Count + " cards successfully Deleted");
        else
          MessageBox.Show("Card successfully Deleted");
      }
    }
  }
  

    private void UpdateData(PersonList list)
    {
      _database.Persons.DataUpdated -= UpdateData;

      if (list != null)
      {
        Person person = list.Persons.FirstOrDefault();
        if (person != null)
        {
          if (person.EntityState == EntityState.Deleted)
          {
            person = null;
            MessageBox.Show("User successfully Deleted");
          }
          else if (person.EntityState == EntityState.Added)
          {
            MessageBox.Show("User successfully Added");
          }
          else
          {
            MessageBox.Show("User successfully Updated");
          }

          Update(person);
        }
      }
    }

       public void UserUpdatePerformer()
    {     
      Photo photo = CurrentPhotoImageView.CurrentImagePhoto;
      if (photo == null)
        return;
      
      photo.OriginType = PhotoOriginType.Loaded;

      Photo thumbnail = _database.PhotoHolder.GetValue(_user.Thumbnailid);
      if (thumbnail != null)
      {
        if (thumbnail.GetHashCode() != photo.GetHashCode())
          _user.Thumbnail = photo;
      }
      else
        _user.Thumbnail = photo;  
           
    }

    */
    private PersonList _list;
  
    private readonly IProcessorLocator _locator;
    private readonly IBioSkyNetRepository _database;
    private readonly INotifier _notifier;
    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
