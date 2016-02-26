using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace BioModule.Utils
{
  public interface IPagingCollectionView 
  {
    void MoveToPreviousPage();
    void MoveToNextPage();
    void Sort(SortDescription description);

    Predicate<object> Filtering { get; set; }
    PagingData GetPagingData();
  }
}
