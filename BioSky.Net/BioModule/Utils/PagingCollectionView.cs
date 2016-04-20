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
     // this._innerList    = innerList   ;
      this._itemsPerPage = itemsPerPage;      
    }

    public void Sort(SortDescription description)
    {
      this.SortDescriptions.Add(description);
    }
    public int ItemsCount
    {
      get { return this.InternalList.Count; }
    }

    
    public override int Count
    {
      get
      {
        //all pages except the last
        if (CurrentPage < PageCount)
          return this._itemsPerPage;

        //last page
        int remainder = InternalList.Count % this._itemsPerPage;

        return remainder == 0 ? Math.Min(InternalList.Count, this._itemsPerPage ) : remainder;
      }
    }
    

    public int CurrentPage
    {
      get {


        int previousCount = (_currentPage - 1) * this._itemsPerPage;
        return (previousCount > this.InternalList.Count) ? 1 : _currentPage;
        //return this._currentPage;

      }
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
        return (InternalList.Count + this._itemsPerPage - 1)
            / this._itemsPerPage;
      }
    }

    public int EndIndex
    {
      get
      {
        var end = CurrentPage * this._itemsPerPage;
        return (end > InternalList.Count) ? InternalList.Count : end;
      }
    }

    public int StartIndex
    {
      get { return (CurrentPage - 1) * this._itemsPerPage; }
    }

    public override object GetItemAt(int index)
    {
      
      var offset = index % (this._itemsPerPage);
      int targetIndex = this.StartIndex + offset;

      try
      {
        return base.GetItemAt(targetIndex);
      }
      catch ( Exception e)
      {
        Console.WriteLine(e.Message);
        return null;
      }
      
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
      set
      {
        OnFilterChanged();
        this.Filter = value;
      }
    }

    public event FilterChangedEventHandler FilterChanged;   

    private void OnFilterChanged()
    {
      if (FilterChanged != null)
        FilterChanged();
    }

    // private readonly IList _innerList;
    private readonly int _itemsPerPage;

    private int _currentPage = 1;
  } 
}
