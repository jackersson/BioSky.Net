using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BioUITest.ViewModels
{

  public class ItemType : PropertyChangedBase
  {
    public string ItemText
    {
      get { return (ItemEnabled) ? "True" : "False"; }
    }

    private bool _itemEnabled;
    public bool ItemEnabled
    {
      get
      {
        return _itemEnabled;
      }
      set
      {
        if (_itemEnabled != value)
        {
          _itemEnabled = value;
          NotifyOfPropertyChange(() => ItemEnabled);
          NotifyOfPropertyChange(() => ItemText);
        }
      }
    }
  }

  public class BioVideoPlayerViewModel : PropertyChangedBase
  {

    public BioVideoPlayerViewModel()
    {
      ItemTest = new ObservableCollection<ItemType>();

      //ItemTest.Add(new ItemType() { ItemEnabled = false });
      //ItemTest.Add(new ItemType() { ItemEnabled = true });
      // ItemTest = _itemTest;
    }

    public ObservableCollection<ItemType> _itemTest;
    public ObservableCollection<ItemType> ItemTest
    {
      get { return _itemTest; }
      set
      {
        if (_itemTest != value)
        {
          _itemTest = value;
          NotifyOfPropertyChange(() => ItemTest);
        }
      }
    }

    public ItemType _selectedItemTest;
    public ItemType SelectedItemTest
    {
      get { return _selectedItemTest; }
      set
      {
        if (_selectedItemTest != value)
        {
          _selectedItemTest = value;
          NotifyOfPropertyChange(() => SelectedItemTest);
        }
      }
    }
     
    public void RemoveItem(object obj)
    {
      Console.Write("f");
    }

    public void EnableItem()
    {
      foreach (ItemType it in ItemTest)
        it.ItemEnabled = !it.ItemEnabled;

      //ItemTest.Add(new ItemType() { ItemEnabled = true });
    }
  }
}
