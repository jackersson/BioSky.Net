using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
   public interface INotificationDialog
  {
    void Update(List<TreeItem> list, string title = "LocationNotificationDialog");
    void Show();
  }

  public class TreeItem
  {
    public TreeItem()
    {
      this.Members = new List<TreeItem>();
    }

    public string Name { get; set; }

    public bool IsSuccess { get; set; }

    private List<TreeItem> _members;
    public List<TreeItem> Members
    {
      get { return (_members == null) ? new List<TreeItem>() : _members; }

      set
      {
        if (value != _members)
          _members = value;
      }
    }
  }
  }
