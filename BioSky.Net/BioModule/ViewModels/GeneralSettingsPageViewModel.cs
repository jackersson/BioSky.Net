using System;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using WPFLocalizeExtension.Engine;
using System.Globalization;
using BioContracts;
using System.Windows.Forms;
using BioModule.Utils;
using System.ComponentModel;
using System.Net;

namespace BioModule.ViewModels
{
  public class GeneralSettingsPageViewModel : Caliburn.Micro.Screen
  {
    public GeneralSettingsPageViewModel(IProcessorLocator locator)
     {
       DisplayName = "GeneralSettings";

       _database       = locator.GetProcessor<IBioSkyNetRepository>();
       _dialogsHolder  = locator.GetProcessor<DialogsHolder>();         
     }

    #region Update
    public void RefreshData()
    {
      if (_revertingGeneralSettings == null)
        _revertingGeneralSettings = new GeneralSettingsPropeties();

      GeneralSettings.SelectedLanguage = _database.LocalStorage.GetParametr(ConfigurationParametrs.Language              );
      GeneralSettings.LocalStoragePath = _database.LocalStorage.GetParametr(ConfigurationParametrs.MediaPathway          );
      string faceService               = _database.LocalStorage.GetParametr(ConfigurationParametrs.FaceServiceAddress    );
      string databaseService           = _database.LocalStorage.GetParametr(ConfigurationParametrs.DatabaseServiceAddress);
      string itemsPerPage              = _database.LocalStorage.GetParametr(ConfigurationParametrs.ItemsCountPerPage     );

      int count = 0;
      if (Int32.TryParse(itemsPerPage, out count))
        GeneralSettings.ItemsCountPerPage = count;

      string ip  ;
      string port;
      SeparateIpPort(faceService, out ip, out port);
      GeneralSettings.FaceService.IP   = ip;
      GeneralSettings.FaceService.Port = port;

      SeparateIpPort(databaseService, out ip, out port);
      GeneralSettings.DatabaseService.IP   = ip;
      GeneralSettings.DatabaseService.Port = port;

      _revertingGeneralSettings = GeneralSettings.Clone();
      RefreshUI();
    }

    public void SeparateIpPort(string full, out string ip, out string port)
    {   
      int i = full.IndexOf(":");
      ip   = (i != 0) ? full.Substring(0, i) : null;
      port = (i != 0) ? full.Substring(i + 1, full.Length - ip.Length - 1) : null;
    }
    public void Update() {}
    #endregion

    #region Interface
    public void LanguageChanged()
    {
      LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
      LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(GeneralSettings.SelectedLanguage);
    }

    public void ResetToDefault()
    {
      var result = _dialogsHolder.AreYouSureDialog.Show();
      if (!result.HasValue || !result.Value)
        return;

      _database.LocalStorage.ReturnToDefault();
      RefreshData();
              
    }

    public bool CanRevert{
      get
      {
         return   _revertingGeneralSettings.DatabaseService.IP   != GeneralSettings.DatabaseService.IP
               || _revertingGeneralSettings.DatabaseService.Port != GeneralSettings.DatabaseService.Port
               || _revertingGeneralSettings.FaceService.IP       != GeneralSettings.FaceService.IP
               || _revertingGeneralSettings.FaceService.Port     != GeneralSettings.FaceService.Port
               || _revertingGeneralSettings.ItemsCountPerPage    != GeneralSettings.ItemsCountPerPage
               || _revertingGeneralSettings.LocalStoragePath     != GeneralSettings.LocalStoragePath
               || _revertingGeneralSettings.SelectedLanguage     != GeneralSettings.SelectedLanguage;
      }
    }

    public bool CanApply{
      get
      {
        return   CanRevert 
              && GeneralSettings.DatabaseService.IP   != string.Empty
              && GeneralSettings.DatabaseService.Port != string.Empty
              && GeneralSettings.FaceService.IP       != string.Empty
              && GeneralSettings.FaceService.Port     != string.Empty  
              && GeneralSettings.LocalStoragePath     != string.Empty
              && GeneralSettings.SelectedLanguage     != string.Empty;
      }
    }

    private void RefreshUI(object sender = null, PropertyChangedEventArgs e = null)
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
      var result = _dialogsHolder.AreYouSureDialog.Show();
      if (!result.HasValue || !result.Value)
        return;

      Save();
      RefreshData();
      
    }

    private void Save()
    {
      ILocalStorage storage = _database.LocalStorage;
      storage.UpdateParametr(ConfigurationParametrs.MediaPathway          , GeneralSettings.LocalStoragePath     + "\\");
      storage.UpdateParametr(ConfigurationParametrs.FaceServiceAddress    , GeneralSettings.FaceService.IP     + ":" + GeneralSettings.FaceService.Port);
      storage.UpdateParametr(ConfigurationParametrs.DatabaseServiceAddress, GeneralSettings.DatabaseService.IP + ":" + GeneralSettings.DatabaseService.Port);
      storage.UpdateParametr(ConfigurationParametrs.Language              , GeneralSettings.SelectedLanguage);
      storage.UpdateParametr(ConfigurationParametrs.ItemsCountPerPage     , GeneralSettings.ItemsCountPerPage.ToString());
    }

    public void Revert()
     {
      var result = _dialogsHolder.AreYouSureDialog.Show();
      if (!result.HasValue || !result.Value)
        return;

       RefreshData();       
     }

     public void OpenFolder()
     {
       FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

       var result = folderBrowserDialog.ShowDialog();
       if (result == DialogResult.OK)
        GeneralSettings.LocalStoragePath = folderBrowserDialog.SelectedPath;       
     }
    #endregion

    #region UI
    private ObservableCollection<string> _languages;
     public ObservableCollection<string> Languages{
      get
      {
        if (_languages == null)
        {
          _languages = new ObservableCollection<string>();
          _languages.Add("en");
          _languages.Add("ru-RU");
          _languages.Add("uk-UA");
        }
        return _languages;}}

    private GeneralSettingsPropeties _generalSettings;
    public GeneralSettingsPropeties GeneralSettings{
      get
      {
        if (_generalSettings == null)
        {
          _generalSettings = new GeneralSettingsPropeties();
          _generalSettings.PropertyChanged += RefreshUI;
        }
        return _generalSettings;
      }
    }
    #endregion

    #region Global Variables 
    private readonly IBioSkyNetRepository     _database                ;
    private readonly DialogsHolder            _dialogsHolder           ;
    private          GeneralSettingsPropeties _revertingGeneralSettings;
    #endregion
  }
  public class GeneralSettingsPropeties : PropertyChangedBase
  {
    public GeneralSettingsPropeties Clone()
    {
      GeneralSettingsPropeties settings = new GeneralSettingsPropeties();
      settings.DatabaseService.IP   = DatabaseService.IP  ;
      settings.DatabaseService.Port = DatabaseService.Port;
      settings.FaceService.Port     = FaceService.Port    ;
      settings.FaceService.IP       = FaceService.IP      ;


      settings.ItemsCountPerPage    = ItemsCountPerPage   ;
      settings.LocalStoragePath     = LocalStoragePath    ;
      settings.SelectedLanguage     = SelectedLanguage    ;
      return settings;
    }

    public void OnSettingsChanged(object sender, PropertyChangedEventArgs e){NotifyOfPropertyChange();}

    private string _selectedLanguage;
    public string SelectedLanguage{
      get { return _selectedLanguage; }
      set {
        if (_selectedLanguage != value)
        {
          _selectedLanguage = value;
          NotifyOfPropertyChange(() => SelectedLanguage);
        }
      }
    }

    private string _localStoragePath;
    public string LocalStoragePath{
      get { return _localStoragePath; }
      set {
        if (_localStoragePath != value)
        {
          _localStoragePath = value;
          NotifyOfPropertyChange(() => LocalStoragePath);          
        }
      }
    }

    private FullIpAdress _faceService;
    public FullIpAdress FaceService {
      get {
        if (_faceService == null)
        {
          _faceService = new FullIpAdress();
          _faceService.PropertyChanged += OnSettingsChanged;
        }
        return _faceService;
      }
      set {
        if (_faceService != value)
        {
          _faceService = value;
          NotifyOfPropertyChange(() => FaceService);
        }
      }
    }

    private FullIpAdress _databaseService;
    public FullIpAdress DatabaseService {
      get {
        if (_databaseService == null)
        {
          _databaseService = new FullIpAdress();
          _databaseService.PropertyChanged += OnSettingsChanged;
        }
        return _databaseService;
      }
      set {
        if (_databaseService != value)
        {
          _databaseService = value;
          NotifyOfPropertyChange(() => DatabaseService);
        }
      }
    }

    private int _itemsCountPerPage;
    public int ItemsCountPerPage {
      get { return _itemsCountPerPage; }
      set {
        if (_itemsCountPerPage != value)
        {
          _itemsCountPerPage = value;
          NotifyOfPropertyChange(() => ItemsCountPerPage);
        }
      }
    }
  }
  public class FullIpAdress : PropertyChangedBase
  {
    private string _ip;
    public string IP {
      get { return _ip; }
      set {
        if (_ip != value)
        {
          _ip = value;
          NotifyOfPropertyChange(() => IP);
        }
      }
    }

    private string _port;
    public string Port {
      get { return _port; }
      set {
        if (_port != value)
        {
          _port = value;
          NotifyOfPropertyChange(() => Port);
        }
      }
    }
  }
}
