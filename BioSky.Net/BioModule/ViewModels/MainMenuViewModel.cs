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

namespace BioModule.ViewModels
{
  public class MainMenuViewModel : Screen
  {

    public MainMenuViewModel(ViewModelSelector viewModelSelector)
    {
      _viewModelSelector = viewModelSelector;
      _languages = new ObservableCollection<string>();
      _languages.Add("en");
      _languages.Add("ru-RU");
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

    private ViewModelSelector _viewModelSelector;

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

    //****************************************************************************************************************
  }
}
