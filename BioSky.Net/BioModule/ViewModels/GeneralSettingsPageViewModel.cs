using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using System.Collections.ObjectModel;
using BioModule.Resources.langs;
using WPFLocalizeExtension.Engine;
using System.Globalization;
using BioContracts;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;
using BioModule.ViewModels;
using BioModule.Utils;

namespace BioModule.ViewModels
{
  public class GeneralSettingsPropeties
  {
   public string SelectedLanguage    { get; set; }
   public string LocalStoragePath    { get; set; }
   public string FaceServiceIP       { get; set; }
   public string FaceServicePort     { get; set; }
   public string DatabaseServiceIP   { get; set; }
   public string DatabaseServicePort { get; set; }
   public int ItemsCountPerPage      { get; set; }

  }

  public class GeneralSettingsPageViewModel : Caliburn.Micro.Screen
  {
     public GeneralSettingsPageViewModel(IProcessorLocator locator)
     {
       DisplayName = "GeneralSettings";

       _locator       = locator;

       _database      = _locator.GetProcessor<IBioSkyNetRepository>();
       _dialogsHolder  = _locator.GetProcessor<DialogsHolder>();

       _languages = new ObservableCollection<string>();
       _languages.Add("en");
       _languages.Add("ru-RU");
       _languages.Add("uk-UA");         
     }

    #region Update

    public void RefreshData()
    {
      if (_revertingGeneralSettings == null)
        _revertingGeneralSettings = new GeneralSettingsPropeties();

      SelectedLanguage       = _database.LocalStorage.GetParametr(ConfigurationParametrs.Language);
      LocalStoragePath       = _database.LocalStorage.GetParametr(ConfigurationParametrs.MediaPathway);
      string faceService     = _database.LocalStorage.GetParametr(ConfigurationParametrs.FaceServiceAddress);
      string databaseService = _database.LocalStorage.GetParametr(ConfigurationParametrs.DatabaseServiceAddress);

      int count = 0;
      string s = _database.LocalStorage.GetParametr(ConfigurationParametrs.ItemsCountPerPage);
      if (Int32.TryParse(s, out count))
        ItemsCountPerPage = count;

      SeparateIpPort(faceService, out _faceServiceIP, out _faceServicePort);
      SeparateIpPort(databaseService, out _databaseServiceIP, out _databaseServicePort);

      NotifyOfPropertyChange(() => FaceServiceIP);
      NotifyOfPropertyChange(() => FaceServicePort);
      NotifyOfPropertyChange(() => DatabaseServiceIP);
      NotifyOfPropertyChange(() => DatabaseServicePort);


      _revertingGeneralSettings.SelectedLanguage    = SelectedLanguage;
      _revertingGeneralSettings.LocalStoragePath    = LocalStoragePath;
      _revertingGeneralSettings.ItemsCountPerPage   = ItemsCountPerPage;
      _revertingGeneralSettings.FaceServiceIP       = FaceServiceIP;
      _revertingGeneralSettings.FaceServicePort     = FaceServicePort;
      _revertingGeneralSettings.DatabaseServiceIP   = DatabaseServiceIP;
      _revertingGeneralSettings.DatabaseServicePort = DatabaseServicePort;

      RefreshUI();
    }

    public void SeparateIpPort(string full, out string ip, out string port)
    {
      int i = full.IndexOf(":");
      if (i != 0)
      {
        ip = full.Substring(0, i);
        port = full.Substring(i + 1, full.Length - ip.Length - 1);
      }
      else
      {
        ip = null;
        port = null;
      }
    }
    public void Update()
    {

    }

    #endregion

    #region Interface

    public void LanguageChanged()
    {
      LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
      LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(SelectedLanguage);
    }

    public void ResetToDefault()
    {
      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (result == true)
      {
        _database.LocalStorage.ReturnToDefault();
        RefreshData();
      }        
    }

    public bool CanRevert
    {
      get
      {
        return   _revertingGeneralSettings.DatabaseServiceIP   != DatabaseServiceIP
              || _revertingGeneralSettings.DatabaseServicePort != DatabaseServicePort
              || _revertingGeneralSettings.FaceServiceIP       != FaceServiceIP
              || _revertingGeneralSettings.FaceServicePort     != FaceServicePort
              || _revertingGeneralSettings.ItemsCountPerPage   != ItemsCountPerPage
              || _revertingGeneralSettings.LocalStoragePath    != LocalStoragePath
              || _revertingGeneralSettings.SelectedLanguage    != SelectedLanguage;

      }
    }

    public bool CanApply
    {
      get
      {
        return   CanRevert 
              && DatabaseServiceIP   != string.Empty
              && DatabaseServicePort != string.Empty
              && FaceServiceIP       != string.Empty
              && FaceServicePort     != string.Empty  
              && LocalStoragePath    != string.Empty
              && SelectedLanguage    != string.Empty;

      }
    }

    private void RefreshUI()
    {
      NotifyOfPropertyChange(() => CanRevert);
      NotifyOfPropertyChange(() => CanApply);
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      RefreshData();
    }

    public void Apply()
    {
      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (result == true)
      {
        Save();
        RefreshData();
      }
    }

    private void Save()
    {
      ILocalStorage storage = _database.LocalStorage;
      storage.UpdateParametr(ConfigurationParametrs.MediaPathway, LocalStoragePath + "\\");
      storage.UpdateParametr(ConfigurationParametrs.FaceServiceAddress, FaceServiceIP + ":" + FaceServicePort);
      storage.UpdateParametr(ConfigurationParametrs.DatabaseServiceAddress, DatabaseServiceIP + ":" + DatabaseServicePort);
      storage.UpdateParametr(ConfigurationParametrs.Language, SelectedLanguage);
      storage.UpdateParametr(ConfigurationParametrs.ItemsCountPerPage, ItemsCountPerPage.ToString());
    }

    public void Revert()
     {
      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (result == true)       
         RefreshData();       
     }

     public void OpenFolder()
     {
       FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

       var result = folderBrowserDialog.ShowDialog();

       if (result == DialogResult.OK)       
         LocalStoragePath = folderBrowserDialog.SelectedPath;       
     }

    #endregion

    #region UI

    private ObservableCollection<string> _languages;
     public ObservableCollection<string> Languages
     {
       get { return _languages; }
       set
       {
         if (_languages != value)
         {
           _languages = value;
           NotifyOfPropertyChange(() => Languages);
         }
       }
     }

     private string _selectedLanguage;
     public string SelectedLanguage
     {
       get { return _selectedLanguage; }
       set
       {
         if (_selectedLanguage != value)
         {
           _selectedLanguage = value;
           
           NotifyOfPropertyChange(() => SelectedLanguage);
          RefreshUI();
        }
      }
     }

     private string _localStoragePath;
     public string LocalStoragePath
     {
       get { return _localStoragePath; }
       set
       {
         if (_localStoragePath != value)
         {
           _localStoragePath = value;
          
          NotifyOfPropertyChange(() => LocalStoragePath);
          RefreshUI();
        }
      }
     }

     private string _faceServiceIP;
     public string FaceServiceIP
     {
       get { return _faceServiceIP; }
       set
       {
         if (_faceServiceIP != value)
         {
           _faceServiceIP = value;
          NotifyOfPropertyChange(() => FaceServiceIP);
          RefreshUI();
        }
      }
     }

     private string _faceServicePort;
     public string FaceServicePort
     {
       get { return _faceServicePort; }
       set
       {
         if (_faceServicePort != value)
         {
           _faceServicePort = value;
         
          NotifyOfPropertyChange(() => FaceServicePort);
          RefreshUI();
        }
      }
     }

     private string _databaseServiceIP;
     public string DatabaseServiceIP
     {
       get { return _databaseServiceIP; }
       set
       {
         if (_databaseServiceIP != value)
         {
           _databaseServiceIP = value;
          
          NotifyOfPropertyChange(() => DatabaseServiceIP);
          RefreshUI();

        }
      }
     }

     private string _databaseServicePort;
     public string DatabaseServicePort
     {
       get { return _databaseServicePort; }
       set
       {
         if (_databaseServicePort != value)
         {
           _databaseServicePort = value;
       
          NotifyOfPropertyChange(() => DatabaseServicePort);
          RefreshUI();
        }
      }
     }

    private int _itemsCountPerPage;
    public int ItemsCountPerPage
    {
      get { return _itemsCountPerPage; }
      set
      {
        if (_itemsCountPerPage != value)
        {
          _itemsCountPerPage = value;
          
          NotifyOfPropertyChange(() => ItemsCountPerPage);
          RefreshUI();
        }
      }
    }

    #endregion

    #region Global Variables

    private readonly IProcessorLocator        _locator                 ;
    private readonly IBioSkyNetRepository     _database                ;
    private readonly DialogsHolder            _dialogsHolder           ;
    private          GeneralSettingsPropeties _revertingGeneralSettings;

    #endregion



  }
}
