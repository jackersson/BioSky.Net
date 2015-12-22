using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using BioContracts;
using BioModule.ResourcesLoader;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

using BioModule.Model;
using BioModule.Utils;
using BioData;
using System.Reflection;
using System.Windows;

namespace BioModule.ViewModels
{
  public class UserPageViewModel : PropertyChangedBase
  {
    public UserPageViewModel( IBioEngine bioEngine )
    {
      _bioEngine = bioEngine;

      _tabPages = new ObservableCollection<ShellTabPage>();

      _tabPages.Add(new ShellTabPage() { Caption = "Information", ScreenViewModel = new UserInformationViewModel(_bioEngine) });
      _tabPages.Add(new ShellTabPage() { Caption = "Cards"      , ScreenViewModel = new UserContactlessCardViewModel() });

      CurrentImageView = new ImageViewModel();
    }

    public void Update(User user)
    {    
     
      _user = user == null ? 
      new User()
      {
          First_Name_ = ""
        , Last_Name_ = ""
        , Gender = Gender.Male.ToString()
        , Rights = Rights.Operator.ToString()
      }
      : user ;

      CurrentImageView.Update(_user.Photo);

      foreach (ShellTabPage tabPage in _tabPages )
      {
        MethodInfo method = tabPage.ScreenViewModel.GetType().GetMethod("Update");
        if (method != null)
          method.Invoke(tabPage.ScreenViewModel, new object[] { _user } );        
      }
    }

    public string Caption()
    {
      return (_user.First_Name_ == "" ) ? "Add New User" : (_user.First_Name_ + " " + _user.Last_Name_);
    }

    private ObservableCollection<ShellTabPage> _tabPages;
    public ObservableCollection<ShellTabPage> TabPages
    {
      get { return _tabPages; }
      private set
      {
        if (_tabPages != value)
        {
          _tabPages = value;
          NotifyOfPropertyChange(() => TabPages);
        }
      }
    }

    private ShellTabPage _selectedTabPage;
    public ShellTabPage SelectedTabPage
    {
      get { return _selectedTabPage; }
      set
      {
        if (_selectedTabPage == value)
          return;
        _selectedTabPage = value;
        NotifyOfPropertyChange(() => SelectedTabPage);
        NotifyOfPropertyChange(() => CurrentViewTab);
      }
    }

    public object CurrentViewTab
    {
      get { return _selectedTabPage == null ? null : _selectedTabPage.ScreenViewModel; }
    }

    private ImageViewModel _currentImageView;
    public ImageViewModel CurrentImageView
    {
      get { return _currentImageView; }
      private set
      {
        if (_currentImageView != value)
        {
          _currentImageView = value;
          NotifyOfPropertyChange(() => CurrentImageView);
        }
      }
    }

    public void SaveChanges()
    {
      _user.Photo = CurrentImageView.ImageFileName;
      _bioEngine.Database().AddUser(_user);
      _bioEngine.Database().SaveChanges();

      MessageBox.Show("User Successfully Added");
    }


    private User _user;
    private readonly IBioEngine _bioEngine;

    //************************************ Resources ****************************************************
    public BitmapSource UserDefaultImageIconSource
    {
      get { return ResourceLoader.UserDefaultImageIconSource; }
    }
    public BitmapSource DeleteIconSource
    {
      get { return ResourceLoader.DeleteIconSource; }
    }
    public BitmapSource OkIconSource
    {
      get { return ResourceLoader.OkIconSource; }
    }

    public BitmapSource CancelIconSource
    {
      get { return ResourceLoader.CancelIconSource; }
    }

    public BitmapSource UserInformationIconSource
    {
      get { return ResourceLoader.UserInformationIconSource; }
    }
    public BitmapSource UserFacesIconSource
    {
      get { return ResourceLoader.UserFacesIconSource; }
    }
    public BitmapSource UserFingerprintIconSource
    {
      get { return ResourceLoader.UserFingerprintIconSource; }
    }
    public BitmapSource UserIricesIconSource
    {
      get { return ResourceLoader.UserIricesIconSource; }
    }
    public BitmapSource UserContactlessCardsIconSource
    {
      get { return ResourceLoader.UserContactlessCardsIconSource; }
    }

  }
}
