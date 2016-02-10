using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;

using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;

using BioData;
using System.Collections.ObjectModel;

using System.Windows.Data;
using System.Reflection;
using System.Globalization;
using BioService;
using BioModule.Utils;

namespace BioModule.ViewModels
{ 
  public class UserInformationViewModel : Screen, IUpdatable
  {    
    public UserInformationViewModel()
    {           
      DisplayName = "Information";
      IsEnabled = true;
    }

    #region Update
    public void Update(Person user)
    {
      User = user;
    }

    #endregion

    #region Interface
    public void Apply()
    {

    }

    public void Remove(bool all)
    {

    }

    #endregion

    #region UI
    public List<string> GenderSources
    {
      get { return Enum.GetNames(typeof(BioService.Person.Types.Gender)).ToList(); }
    }
    public List<string> RightsSources
    {
      get { return Enum.GetNames(typeof(BioService.Person.Types.Rights)).ToList(); }
    }

    private Person _user;
    public Person User
    {
      get { return _user; }
      set
      {
        if (_user != value)
        {
          _user = value;
          NotifyOfPropertyChange(() => GenderSources);
          NotifyOfPropertyChange(() => User);
        }
      }
    }


    public void OnDateofBirthChanged(string text)
    {
      try
      {      
        DateTime dt = DateTime.ParseExact(text, "M/d/yyyy", CultureInfo.InvariantCulture);
        User.Dateofbirth = dt.Ticks;
      }
      catch ( Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private bool _isEnabled;
    public bool IsEnabled
    {
      get { return _isEnabled; }
      set
      {
        if (_isEnabled != value)
        {
          _isEnabled = value;
          NotifyOfPropertyChange(() => IsEnabled);
        }
      }
    }

    #endregion    
  }
}
