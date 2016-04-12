using BioService;
using System.Threading.Tasks;

namespace BioContracts.Services
{
  public interface IThumbnailDataClient
  {
    Task SetThumbnail(Person owner, Photo requested);
  }
}
