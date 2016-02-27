using BioContracts;
using BioContracts.Services;
using BioService;
using Grpc.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BioGRPC.DatabaseClient
{
  public class PhotoDataClient : IDataClient<Photo, CommandPhoto>
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


    public async Task Select(CommandPhoto command)
    {
      try
      {
        PhotoList call = await _client.PhotoSelectAsync(command);
        _database.PhotoHolder.Update(call.Photos);
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

    public Task Delete(IList<long> targeIds)
    {
      return null;
    }

    private PhotoList _list;

    private readonly IProcessorLocator _locator;
    private readonly IBioSkyNetRepository _database;
    private readonly INotifier _notifier;
    private readonly BiometricDatabaseSevice.IBiometricDatabaseSeviceClient _client;
  }
}
