using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioModule.Utils;

using WPFLocalizeExtension.Engine;
using BioModule.Resources.langs;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using BioContracts;


namespace BioModule.ViewModels
{
  public class MainMenuViewModel : Screen
  {
    public MainMenuViewModel( IProcessorLocator locator)
    {   
      _viewModelSelector = locator.GetProcessor<ViewModelSelector>();
      _windowManager     = locator.GetProcessor<IWindowManager>();
      //_languages = new ObservableCollection<string>();
      //_languages.Add("en");
      //_languages.Add("ru-RU");
    }

    public void OpenTabAddNewPerson()
    {
      _viewModelSelector.ShowContent( ShowableContentControl.TabControlContent,  ViewModelsID.UserPage);
    }

    public void OpenTabAddNewLocation()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.FlyoutControlContent, ViewModelsID.LocationSettings);
    }

    public void OpenTabVisitors()
    {
      _viewModelSelector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.VisitorsPage);
    }
       

    //****************************************************Language****************************************************
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

    public void LanguageChanged()
    {
      LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
      LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo(SelectedLanguage);
    }

    //***************************************************************************************************************
    public void UpdateUserPassword(bool register, string name, System.Security.SecureString password)
    {
      Console.WriteLine(register + " " + name + " " + password);

    } 
    public async void ShowAboutDialog()
    {
      
      _windowManager.ShowDialog(new AboutDialogViewModel());      
    }
    public void ShowLogInDialog()
    {      
      var result = _windowManager.ShowDialog(new LoginDialogViewModel(this));      
    }

    private ViewModelSelector          _viewModelSelector;
    private readonly IWindowManager    _windowManager;
  }
}
