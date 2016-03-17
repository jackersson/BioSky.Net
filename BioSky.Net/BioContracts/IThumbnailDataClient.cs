using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IThumbnailDataClient
  {
    Task SetThumbnail(Person owner, Photo requested);
  }
}
