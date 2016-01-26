using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using BioModule.DragDrop;


namespace BioModule.ViewModels
{
  public class DragableWithRemoveItem : IDragablePerformer
  {
    public void ItemDropped(ObservableCollection<DragableItem> dragableItems, object obj)
    {
      DragableItem dragItem = (DragableItem)obj;
      if (dragItem != null)
      {
        DragableItem newItem = dragItem.Clone();
        newItem.ItemEnabled = true;
        dragableItems.Add(newItem);
      }         
    }

    public void ItemDragged(ObservableCollection<DragableItem> dragableItems, object obj)
    {
      DragableItem dragItem = (DragableItem)obj;
      if (dragItem != null)
        dragableItems.Remove(dragItem);
    }

    public bool CanRemove()
    { 
      return true; 
    }
  }

  public class DragableWithDisabledItem :  IDragablePerformer
  {
    public void ItemDropped(ObservableCollection<DragableItem> dragableItems, object obj)
    {
      DragableItem dragItem = (DragableItem)obj;
      if (dragItem != null)
      {
        DragableItem di = dragableItems.Where(x => x.ItemContext == dragItem.ItemContext).FirstOrDefault();
        if (di != null)
          di.ItemEnabled = true;           
      }      
    }

    public void ItemDragged(ObservableCollection<DragableItem> dragableItems, object obj)
    {
      DragableItem dragItem = (DragableItem)obj;
      if (dragItem != null)
        dragItem.ItemEnabled = false;      
    }
    public bool CanRemove()
    {
      return false;
    }
  }


  public class DragablListBoxViewModel : Screen, IDragable
  {
    public delegate void RemoveDragableItemHandler(DragableItem item);
    public event RemoveDragableItemHandler ItemRemoved;

    public void OnItemRemoved( DragableItem item )
    {
      if (ItemRemoved != null)
        ItemRemoved(item);
    }


    public DragablListBoxViewModel(IDragablePerformer performer)
    {     
      _performer    = performer;
      DragableItems = new ObservableCollection<DragableItem>();
    }

    public void ItemDropped(object obj)
    {
      _performer.ItemDropped(DragableItems, obj);
    }

    public void ItemDragged(object obj)
    {
      _performer.ItemDragged(DragableItems, obj);
      SelectedItem = null;
      NotifyOfPropertyChange(() => SelectedItem);
    }
    public void OnRemove()
    {
      if (SelectedItem != null)
      {
        DragableItem item = SelectedItem;
        ItemDragged(SelectedItem);
        OnItemRemoved(item);
      }
      
    }
    public void OnMouseRightButtonDown( DragableItem IsDragableItem)
    {      
      if(_performer.CanRemove())
      {
        MenuRemoveStatus = (IsDragableItem != null);
        SelectedItem = IsDragableItem;
      }
    }
    public void Add(DragableItem di)
    {
      DragableItems.Add(di);
    }

    public bool ContainsItem(DragableItem di)
    {
      DragableItem dragableItem = DragableItems.Where(x => x.DisplayName == di.DisplayName).FirstOrDefault();
      if (dragableItem != null)
        return true;

      return false;
    }

    private ObservableCollection<DragableItem> _dragableItems;
    public ObservableCollection<DragableItem> DragableItems
    {
      get { return _dragableItems; }
      set
      {
        if (_dragableItems != value)
        {
          _dragableItems = value;
          NotifyOfPropertyChange(() => DragableItems);
        }
      }
    }

    private DragableItem _selectedItem;
    public DragableItem SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem != value)
        {
          _selectedItem = value;         
          NotifyOfPropertyChange(() => SelectedItem);
        }
      }
    }

    private bool _menuRemoveStatus;
    public bool MenuRemoveStatus
    {
      get { return _menuRemoveStatus; }
      set
      {
        if (_menuRemoveStatus != value)
        {
          _menuRemoveStatus = value;
          NotifyOfPropertyChange(() => MenuRemoveStatus);
        }
      }
    }
    public object GetContext()
    {
      return SelectedItem;
    }
  
    private readonly IDragablePerformer _performer;


  }
}
