using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;

namespace BioModule.ViewModels
{
  public class PhotoInfoExpanderViewModel : Screen
  {
    public PhotoInfoExpanderViewModel()
    {
      Age = "34567890";
      Gender = "Male"; 
    }

    private string _age;
    public string Age
    {
      get { return _age; }
      set
      {
        if (_age != value)
        {
          _age = value;
          NotifyOfPropertyChange(() => Age);
        }
      }
    }

    private string _gender;
    public string Gender
    {
      get { return _gender; }
      set
      {
        if (_gender != value)
        {
          _gender = value;
          NotifyOfPropertyChange(() => Gender);
        }
      }
    }
  }
}
