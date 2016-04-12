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
       SelectedLanguage        = _database.LocalStorage.GetParametr(ConfigurationParametrs.Language)              ;
       LocalStoragePath        = _database.LocalStorage.GetParametr(ConfigurationParametrs.MediaPathway)          ;
       string faceService      = _database.LocalStorage.GetParametr(ConfigurationParametrs.FaceServiceAddress)    ;
       string databaseService  = _database.LocalStorage.GetParametr(ConfigurationParametrs.DatabaseServiceAddress);

       SeparateIpPort(faceService    , out _faceServiceIP    , out _faceServicePort    );
       SeparateIpPort(databaseService, out _databaseServiceIP, out _databaseServicePort);
    }

     public void SeparateIpPort(string full, out string ip, out string port)
     {
       int i = full.IndexOf(":");
       if(i != 0)
       {
         ip   = full.Substring(0, i);
         port = full.Substring(i + 1, full.Length - ip.Length - 1);
       }
       else
       {
         ip   = null;
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
        Save();
    }

    private void Save()
    {
      ILocalStorage storage = _database.LocalStorage;
      storage.UpdateParametr(ConfigurationParametrs.MediaPathway, LocalStoragePath);
      storage.UpdateParametr(ConfigurationParametrs.FaceServiceAddress, FaceServiceIP + ":" + FaceServicePort);
      storage.UpdateParametr(ConfigurationParametrs.DatabaseServiceAddress, DatabaseServiceIP + ":" + DatabaseServicePort);
      storage.UpdateParametr(ConfigurationParametrs.Language, SelectedLanguage);
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
         }
       }
     }

    #endregion

    #region Global Variables

    private readonly IProcessorLocator    _locator      ;
    private readonly IBioSkyNetRepository _database     ;
    private readonly DialogsHolder        _dialogsHolder;

    #endregion



  }
}
