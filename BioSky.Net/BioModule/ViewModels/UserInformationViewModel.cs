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
  public class UserInformationViewModel : PropertyChangedBase
  {

    public UserInformationViewModel()
    {
      _firstNameBox = new System.Windows.Controls.TextBox();
    }

    public BitmapSource OkIconSource
    {
      get { return ResourceLoader.OkIconSource; }
    }

    public BitmapSource CancelIconSource
    {
      get { return ResourceLoader.CancelIconSource; }
    }

    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }

    private System.Windows.Controls.TextBox _firstNameBox;
    public System.Windows.Controls.TextBox FirstNameBox
    {
      get
      {
        return _firstNameBox;
      }
      set
      {
        if (_firstNameBox != value)
          _firstNameBox = value;

        NotifyOfPropertyChange(() => FirstNameBox);
      }
    }

     public void UserInfo(string firstName)
    {

      string s = firstName;


      Console.WriteLine(s);
    } 

  }
}
