using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Data;
using System.Collections;
using System.ComponentModel;

namespace BioModule.Utils
{
  public class PagingCollectionView : ListCollectionView
  {
    private readonly IList _innerList;
    private readonly int _itemsPerPage;

    private int _currentPage = 1;

    public PagingCollectionView(IList innerList, int itemsPerPage)
      : base(innerList)
    {
      this._innerList = innerList;
      this._itemsPerPage = itemsPerPage;
    }  

    public override int Count
    {
      get
      {
        //all pages except the last
        if (CurrentPage < PageCount)
          return this._itemsPerPage;

        //last page
        int remainder = _innerList.Count % this._itemsPerPage;

        return remainder == 0 ? this._itemsPerPage : remainder;
      }
    }

    public int CurrentPage
    {
      get { return this._currentPage; }
      set
      {
        this._currentPage = value;
        this.OnPropertyChanged(new PropertyChangedEventArgs("CurrentPage"));
      }
    }

    public int ItemsPerPage { get { return this._itemsPerPage; } }

    public int PageCount
    {
      get
      {
        return (this._innerList.Count + this._itemsPerPage - 1)
            / this._itemsPerPage;
      }
    }

    private int EndIndex
    {
      get
      {
        var end = this._currentPage * this._itemsPerPage - 1;
        return (end > this._innerList.Count) ? this._innerList.Count : end;
      }
    }

    private int StartIndex
    {
      get { return (this._currentPage - 1) * this._itemsPerPage; }
    }

    public override object GetItemAt(int index)
    {
      var offset = index % (this._itemsPerPage);
      return base.GetItemAt(this.StartIndex + offset);
    }

    public void MoveToNextPage()
    {
      if (this._currentPage < this.PageCount)
      {
        this.CurrentPage += 1;
      }
      this.Refresh();
    }

    public void MoveToPreviousPage()
    {
      if (this._currentPage > 1)
      {
        this.CurrentPage -= 1;
      }
      this.Refresh();
    }
  }
}
