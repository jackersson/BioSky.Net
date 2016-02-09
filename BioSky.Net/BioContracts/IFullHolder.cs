using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface IFullHolder<TList>
  {
    void Init(TList list);

    void Update(TList updated, TList results);


    event DataChangedHandler         DataChanged;
    event DataUpdatedHandler<TList>  DataUpdated;
 
  }
}
