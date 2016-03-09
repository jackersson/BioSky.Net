using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace BioGRPC.DatabaseClient
{
  public class PhotoDataClient : IDataClient<Photo, QueryPhoto>
  {
    public PhotoDataClient(  IProcessorLocator locator
                            , BiometricDatabaseSevice.IBiometricDatabaseSeviceClient client)
    {
      _client = client;
      _locator = locator;

      _database = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier = _locator.GetProcessor<INotifier>();

      _list = new PhotoList();

      //_database.PhotoHolderByPerson.FullPhotoRequested += PhotoHolderByPerson_FullPhotoRequested;
    }

    /*
    private async void PhotoHolderByPerson_FullPhotoRequested(PhotoList list)
    {
      CommandPhoto cmd = new CommandPhoto();
      cmd.Description = true;

      foreach (Photo ph in list.Photos)      
        cmd.TargetPhoto.Add(new Photo() { Id = ph.Id });

      await PhotosSelect(cmd);
    }
    */


    public async Task Select(QueryPhoto command)
    {
      try
      {
        //PhotoList call = await _client.PhotoSelectAsync(command);
        //_database.PhotoHolder.Update(call.Photos);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }

    public Task Add(Photo item)
    {
      return null;
    }

    public Task Update(Photo item)
    {
      return null;
    }

    public Task Delete(IList<Photo> targePhotos)
    {
      return null;
    }

    public Task Delete(Photo targetItem)
    {
      throw new NotImplementedException();
    }

    /*
    private void UpdateData(PersonList list)
    {
      _database.Persons.DataUpdated -= UpdateData;

      if (list != null)
      {
        Person person = list.Persons.FirstOrDefault();
        if (person != null)
        {
          foreach (Photo photo in person.Photos)
          {
            if (photo.Id == _user.Thumbnail.Id)
              _imageViewer.UpdateImage(null, null);
          }

          if (person.Photos.Count > 1)
            MessageBox.Show(person.Photos.Count + " photos successfully Deleted");
          else
            MessageBox.Show("Photo successfully Deleted");
        }
      }
    }
    */
    /*

    public async Task PhotoDeletePerformer()
    {
      PersonList personList = new PersonList();

      Person person = new Person()
      {
        EntityState = EntityState.Unchanged
        ,
        Id = _user.Id
      };

      Photo photo = SelectedItem;
      photo.EntityState = EntityState.Deleted;

      person.Photos.Add(photo);
      personList.Persons.Add(person);

      try
      {
        _database.Persons.DataUpdated += UpdateData;
        await _serviceManager.DatabaseService.PersonUpdate(personList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }
    }
    */
    /*
    public async Task ThumbnailUpdatePerformer()
    {
      _user.EntityState = EntityState.Modified;
      _user.Thumbnailid = SelectedItem.Id;

      PersonList personList = new PersonList();
      personList.Persons.Add(_user);

      try
      {
        _database.Persons.DataUpdated += UpdateThumbnail;
        await _bioService.PersonDataClient.Delete(personWithPhoto); ;
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }
    }
    
    public async void GetFeedBackPhoto(Photo feedbackPhoto)
    {
      if (feedbackPhoto == null)
        return;

      Person personWithPhoto = new Person() { Id = _user.Id };
      feedbackPhoto.Personid = _user.Id;
      personWithPhoto.Photos.Add(feedbackPhoto);

      try
      {
        await _bioService.PersonDataClient.Update(personWithPhoto);
      }
      catch (RpcException e)
      {
        _notifier.Notify(e);
      }
    }
    */
    private PhotoList _list;

    private readonly IProcessorLocator _locator;
    private readonly IBioSkyNetRepository _database;
    private readonly INotifier _notifier;
    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
