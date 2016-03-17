using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Data;
using System.Collections;
using System.ComponentModel;
using BioModule.ViewModels;

namespace BioModule.Utils
{
  public class PagingCollectionView : ListCollectionView, IPagingCollectionView
  {    
    public PagingCollectionView( IList innerList, int itemsPerPage)
                               : base(innerList)
    {
      this._innerList = innerList;
      this._itemsPerPage = itemsPerPage;      
    }

    public void Sort(SortDescription description)
    {
      this.SortDescriptions.Add(description);
    }
    public int ItemsCount
    {
      get { return this._innerList.Count; }
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

    public int EndIndex
    {
      get
      {
        var end = this._currentPage * this._itemsPerPage;
        return (end > this._innerList.Count) ? this._innerList.Count : end;
      }
    }

    public int StartIndex
    {
      get { return (this._currentPage - 1) * this._itemsPerPage; }
    }

    public override object GetItemAt(int index)
    {
      var offset = index % (this._itemsPerPage);
      int targetIndex = this.StartIndex + offset;

      if (this._innerList.Count <= targetIndex)
        return null;
      return base.GetItemAt(this.StartIndex + offset);
    }

    public void MoveToNextPage()
    {
      if (this._currentPage < this.PageCount)      
        this.CurrentPage += 1;
      
      this.Refresh();
    }

    public void MoveToPreviousPage()
    {
      if (this._currentPage > 1)      
        this.CurrentPage -= 1;
      
      this.Refresh();
    }


    public Predicate<object> Filtering
    {
      get { return this.Filter;   }
      set { this.Filter = value;  }
    }

    private readonly IList _innerList;
    private readonly int _itemsPerPage;

    private int _currentPage = 1;
  } 
}
