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

    public bool CanRevert{ get{ return !GeneralSettings.Equals(_revertingGeneralSettings);}}

    public bool CanApply{ get { return   CanRevert && !GeneralSettings.IsEmpty; }}

    private void RefreshUI(object sender = null, PropertyChangedEventArgs e = null)
    {
      NotifyOfPropertyChange(() => CanRevert);
      NotifyOfPropertyChange(() => CanApply);
    }
    protected override void OnActivate()
    {
      base.OnActivate();
      RefreshData();
      GeneralSettings.Activate();
      GeneralSettings.PropertyChanged += RefreshUI;
    }

    protected override void OnDeactivate(bool close)
    {
      GeneralSettings.Deactivate();
      GeneralSettings.PropertyChanged -= RefreshUI;
      base.OnDeactivate(close);
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
      storage.UpdateParametr(ConfigurationParametrs.MediaPathway          , string.Format("{0}\\", GeneralSettings.LocalStoragePath));
      storage.UpdateParametr(ConfigurationParametrs.FaceServiceAddress    , string.Format("{0}:{1}", GeneralSettings.FaceService.IP    , GeneralSettings.FaceService    .Port));
      storage.UpdateParametr(ConfigurationParametrs.DatabaseServiceAddress, string.Format("{0}:{1}", GeneralSettings.DatabaseService.IP, GeneralSettings.DatabaseService.Port));
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
          _generalSettings = new GeneralSettingsPropeties();        
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
      settings.DatabaseService = DatabaseService.CopyFrom();
      settings.FaceService     = FaceService    .CopyFrom();

      settings.ItemsCountPerPage    = ItemsCountPerPage;
      settings.LocalStoragePath     = LocalStoragePath ;
      settings.SelectedLanguage     = SelectedLanguage ;
      return settings;
    }

    public override bool Equals(object obj)
    {
      return (this.GetHashCode() == obj.GetHashCode());
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hash = 17;
        hash = hash * 23 + DatabaseService  .GetHashCode();
        hash = hash * 23 + FaceService      .GetHashCode();
        hash = hash * 23 + LocalStoragePath .GetHashCode();
        hash = hash * 23 + SelectedLanguage .GetHashCode();
        hash = hash * 23 + ItemsCountPerPage.GetHashCode();
        return hash;
      }          
    }
    public void Activate()
    {
      FaceService    .PropertyChanged += OnSettingsChanged;
      DatabaseService.PropertyChanged += OnSettingsChanged;
    }

    public void Deactivate()
    {
      FaceService    .PropertyChanged -= OnSettingsChanged;
      DatabaseService.PropertyChanged -= OnSettingsChanged;
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
          _faceService = new FullIpAdress();        
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
          _databaseService = new FullIpAdress();       
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
    public bool IsEmpty
    {
      get
      {
        return    DatabaseService.IsEmpty
               && FaceService.IsEmpty
               && string.IsNullOrEmpty(LocalStoragePath)
               && string.IsNullOrEmpty(SelectedLanguage);
      }
    }
  }
  public class FullIpAdress : PropertyChangedBase
  {
    public FullIpAdress CopyFrom()
    {
      FullIpAdress copy = new FullIpAdress();
      copy.IP   = IP;
      copy.Port = Port;
      return copy;
    }
    public override int GetHashCode()
    {
      unchecked
      {
        int hash = 13;
        hash = hash * 23 + IP.GetHashCode();
        hash = hash * 23 + Port.GetHashCode();
        return hash;
      }
    }
    public bool IsEmpty{ get{ return !string.IsNullOrEmpty(IP) && !string.IsNullOrEmpty(Port); }}

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
