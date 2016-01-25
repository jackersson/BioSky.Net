using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using BioModule.DragDrop;

namespace BioModule.ViewModels
{ 
  public class DragPreviewViewModel : Screen, IDragablePreview
  {    
    public void Update( object obj )
    {
      DragableItem item = (DragableItem)obj;
      if (item == null)
        return;

      DisplayName = item.DisplayName;    
    }  
  }
}
