using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BioModule.DragDrop
{
  public interface IDragablePerformer
  {
    void ItemDropped(ObservableCollection<DragableItem> dragableItems, object obj);
    void ItemDragged(ObservableCollection<DragableItem> dragableItems, object obj);
    bool CanRemove();
  }
}
