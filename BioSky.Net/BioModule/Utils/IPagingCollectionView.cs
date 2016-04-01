using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace BioModule.Utils
{
  public delegate void FilterChangedEventHandler();
  public interface IPageController
  {
    int EndIndex     { get; }
    int StartIndex   { get; }

    int ItemsCount   { get; }
    int ItemsPerPage { get; }

    void MoveToPreviousPage();
    void MoveToNextPage();

    event FilterChangedEventHandler FilterChanged;
  }

  public interface IPagingCollectionView : IPageController
  {    
    void Sort(SortDescription description);

    Predicate<object> Filtering { get; set; }    
  }
}
