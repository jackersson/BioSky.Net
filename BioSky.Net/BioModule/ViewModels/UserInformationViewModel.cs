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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using BioModule.Validation;

namespace BioModule.ViewModels
{ 

  public delegate void ValidationStateEventHandler(bool state);
  public class UserInformationViewModel : Screen, IUpdatable, IDataErrorInfo
  {
    public event ValidationStateEventHandler ValidationStateChanged;
    public UserInformationViewModel(IProcessorLocator locator , IUserBioItemsUpdatable imageViewer)
    {
      _locator     = locator;
      _database    = _locator.GetProcessor<IBioSkyNetRepository>();
      _imageViewer = imageViewer;

      _validator = new BioValidator();

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
    protected override void OnActivate()
    {
      base.OnActivate();

      _imageViewer.ChangeBioImageModel(BioImageModelEnum.Faces);
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
    }

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

    [Required(ErrorMessage = "You must enter a First Name.")]
    [RegularExpression(@"^[a-zA-Zа-яА-яїі `\-]{2,}$", ErrorMessage = "The First Name must only contain letters (a-z, A-Z, а-я, А-Я, `).")]  
    public string FirstName
    {    
      get { return (_user != null) ? _user.Firstname : string.Empty; }
      set
      {      
        _user.Firstname = value;
        NotifyOfPropertyChange(() => FirstName);        
      }
    }

    [Required(ErrorMessage = "You must enter a Last Name.")]
    [RegularExpression(@"^[a-zA-Zа-яА-яїі `\-]{2,}$", ErrorMessage = "The Last Name must only contain letters (a-z, A-Z, а-я, А-Я, `).")]
    public string LastName
    {
      get { return (_user != null) ? _user.Lastname : string.Empty; }
      set
      {
        _user.Lastname = value;
        NotifyOfPropertyChange(() => LastName);
      }
    }

    [RegularExpression(@"^[a-zA-Z0-9_\.]+@[a-zA-Z_]+?\.[a-zA-Z]{2,63}$", ErrorMessage = "Invalid email (Check for '@', '.')")]
    public string Email
    {
      get { return (_user != null) ? _user.Email : string.Empty; }
      set
      {
        _user.Email = value;
        NotifyOfPropertyChange(() => Email);
      }
    }

    //[RegularExpression(@"^[1-9`\-. :]{1,24}$", ErrorMessage = "Invalid datetime (Check for ':' '.' '-')")]
    public long Dateofbirth
    {
      get { return (_user != null) ? _user.Dateofbirth : 0; }
      set
      {
        _user.Dateofbirth = value;
        NotifyOfPropertyChange(() => Dateofbirth);
      }
    }

    [RegularExpression(@"^[a-zA-Zа-яА-яїі `\-]{2,}$", ErrorMessage = "The City must only contain letters (a-z, A-Z, а-я, А-Я, `).")]
    public string City
    {
      get { return (_user != null) ? _user.City : string.Empty; }
      set
      {
        _user.City = value;
        NotifyOfPropertyChange(() => City);
      }
    }

    public string Comments
    {
      get { return (_user != null) ? _user.Comments : string.Empty; }
      set
      {
        _user.Comments = value;
        NotifyOfPropertyChange(() => Comments);
      }
    }

    public Person.Types.Rights Rights
    {
      get { return (_user != null) ? _user.Rights : Person.Types.Rights.Custom; }
      set
      {
        _user.Rights = value;
        NotifyOfPropertyChange(() => Rights);
      }
    }

    public string Country
    {
      get { return (_user != null) ? _user.Country : string.Empty; }
      set
      {
        _user.Country = value;
        NotifyOfPropertyChange(() => Country);
      }
    }

    public Person.Types.Gender Gender
    {
      get { return (_user != null) ? _user.Gender : Person.Types.Gender.None; }
      set
      {
        _user.Gender = value;
        NotifyOfPropertyChange(() => Gender);
      }
    }



    #region Validation    
    public string this[string columnName]
    {
      get {        
        string temp = string.Join(Environment.NewLine, _validator.Validate(this, columnName).Select(x => x.Message));
         
        Error = string.IsNullOrEmpty(temp) ? string.Join(Environment.NewLine, _validator.Validate(this).Select(x => x.Message))
                                            : temp;        
        return temp;
      }
    }

    public string _error;
    public string Error
    {
      get { return _error; }
      set {
        if (_error != value)
        {
          _error = value;
          OnValidationStateChanged(string.IsNullOrEmpty(Error));
        }        
      }
    }

    private void OnValidationStateChanged(bool state)
    {
      if (ValidationStateChanged != null)
        ValidationStateChanged(state);
    }        
    #endregion

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
    private readonly IValidator             _validator  ;
    private readonly IProcessorLocator      _locator    ;
    private readonly IBioSkyNetRepository   _database   ;
    private          IUserBioItemsUpdatable _imageViewer;
  }
}
