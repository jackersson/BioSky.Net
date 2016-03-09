using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BioModule.Utils
{
  public class DeviceItemBase<T> : PropertyChangedBase
  {
    public T ItemContext { get; set; }

    private bool _itemActive;
    public bool ItemActive
    {
      get { return _itemActive; }
      set
      {
        if (_itemActive != value)
        {
          _itemActive = value;
          NotifyOfPropertyChange(() => ItemActive);
        }
      }
    }

    private bool _itemEnabled;
    public bool ItemEnabled
    {
      get { return _itemEnabled; }
      set
      {
        if (_itemEnabled != value)
        {
          _itemEnabled = value;
          NotifyOfPropertyChange(() => ItemEnabled);
        }
      }
    }

    public DeviceItemBase<T> Clone()
    {
      return new DeviceItemBase<T>()
      {
        ItemContext = this.ItemContext
        , ItemActive = this.ItemActive
        , ItemEnabled = this.ItemEnabled
      };
    }
  }
}
