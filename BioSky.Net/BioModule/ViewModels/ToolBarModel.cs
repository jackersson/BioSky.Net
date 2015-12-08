using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;



namespace BioModule.ViewModels
{
  class ToolBarModel : PropertyChangedBase
  {
    public BitmapSource AddPersonIconSource
    {
      get { return ResourceLoader.AddUserIconSource; }
    }

    public BitmapSource AddLocationIconSource
    {
      get { return ResourceLoader.AddLocationIconSource; }
    }

    public BitmapSource JournalListIconSource
    {
      get { return ResourceLoader.JournalListIconSource; }
    }

    public BitmapSource UsersListIconSource
    {
      get { return ResourceLoader.UsersListIconSource; }
    }
  }
}
