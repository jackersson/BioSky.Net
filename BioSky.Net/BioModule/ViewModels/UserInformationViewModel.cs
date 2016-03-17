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
using BioContracts;

namespace BioModule.ViewModels
{
  public class UserInformationViewModel : Screen, IUpdatable
  {
    public UserInformationViewModel(IProcessorLocator locator)
    {
      _locator = locator;
      _database = _locator.GetProcessor<IBioSkyNetRepository>();

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
    public void Apply() { }

    #endregion

    #region UI    
    private Person _user;
    public Person User
    {
      get { return _user; }
      set
      {
        if (_user != value)
        {
          _user = value;
          NotifyOfPropertyChange(() => User);
          NotifyOfPropertyChange(() => FirstName);
          NotifyOfPropertyChange(() => LastName);
        }
      }
    }
    [System.ComponentModel.DataAnnotations.Required]
    public string FirstName
    {
      get { return _user.Firstname;}
      set
      {
        if (String.IsNullOrEmpty(value))
        {
          throw new ArgumentException("Name can not be empty.");
        }
        if (value.Length > 4)
        {
          throw new ArgumentException("name can not be longer than 4 charectors");
        }

        _user.Firstname = value;
        NotifyOfPropertyChange(() => FirstName);        
      }
    }

    public string LastName
    {
      get { return _user.Lastname; }
      set
      {
        _user.Lastname = value;
        NotifyOfPropertyChange(() => LastName);
      }
    }

    public void OnDateofBirthChanged(string text)
    {
      try
      {
        string dateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        DateTime dt = DateTime.ParseExact(text, dateFormat, CultureInfo.InvariantCulture);
        User.Dateofbirth = dt.Ticks;
      }
      catch (Exception ex)
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


    public string[] CountryNames
    {
      get { return _database.BioCultureSources.CountriesNames; }
    }
    public List<string> RightsSources
    {
      get { return _database.BioCultureSources.RightsSources; }
    }
    public List<string> GenderSources
    {
      get { return _database.BioCultureSources.GenderSources; }
    }
    #endregion

    private readonly IProcessorLocator _locator;
    private readonly IBioSkyNetRepository _database;
  }
}
