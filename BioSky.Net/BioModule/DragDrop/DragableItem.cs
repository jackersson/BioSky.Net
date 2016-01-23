using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;

namespace BioModule.DragDrop
{
  public class DragableItem : PropertyChangedBase
  {
    public object ItemContext { get; set; }

    private bool _itemEnabled;
    public bool ItemEnabled
    {
      get { return _itemEnabled; }
      set
      {
        if ( _itemEnabled != value)
        {
          _itemEnabled = value;         
          NotifyOfPropertyChange(() => ItemEnabled);
          NotifyOfPropertyChange(() => DisplayName);
        }
      }
    }
    public DragableItem Clone()
    {
      return new DragableItem() { ItemContext = this.ItemContext, ItemEnabled = this.ItemEnabled, DisplayName = this.DisplayName };
    }

    public string _displayName;
    public string DisplayName
    {
      get;
      set;
    }
  }
}
