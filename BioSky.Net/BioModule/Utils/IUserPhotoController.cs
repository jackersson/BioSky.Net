using BioModule.ViewModels;
using BioService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.Utils
{
  public interface IUserBioItemsController
  {
    void Add   (Photo photo);
    void Remove(Photo photo);
    void Next();
    void Previous();
    bool CanNext           { get; }
    bool CanPrevious       { get; }
    BioImageModelType BioImageModelType { get; }
    Person User            { get; }
  }
}
