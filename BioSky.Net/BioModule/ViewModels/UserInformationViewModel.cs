﻿using System;
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
    public UserInformationViewModel(IProcessorLocator locator)
    {
      _locator = locator;
      _database = _locator.GetProcessor<IBioSkyNetRepository>();

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
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The First Name must only contain letters (a-z, A-Z).")]  
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
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "The Last Name must only contain letters (a-z, A-Z).")]
    public string LastName
    {
      get { return (_user != null) ? _user.Lastname : string.Empty; }
      set
      {
        _user.Lastname = value;
        NotifyOfPropertyChange(() => LastName);
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
    private readonly IValidator           _validator;
    private readonly IProcessorLocator    _locator;
    private readonly IBioSkyNetRepository _database;
  }
}
