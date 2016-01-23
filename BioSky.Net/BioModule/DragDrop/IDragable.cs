using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioModule.DragDrop
{
  public interface IDragable
  {
    void ItemDropped(object obj);
    void ItemDragged(object obj);

    object GetContext();
  }
}
