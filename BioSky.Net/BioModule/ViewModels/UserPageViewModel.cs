﻿using System;
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

namespace BioModule.ViewModels
{
  public class UserPageViewModel : PropertyChangedBase
  {

    IBioEngine _bioEngine;

    public UserPageViewModel( IBioEngine bioEngine )
    {
      _bioEngine = bioEngine;

      _tabPages = new ObservableCollection<ShellTabPage>();

      _tabPages.Add(new ShellTabPage() { Caption = "Information", ScreenViewModel = new UserInformationViewModel(_bioEngine) });
      _tabPages.Add(new ShellTabPage() { Caption = "Cards", ScreenViewModel = new UserContactlessCardViewModel() });

      CurrentImageView = new ImageViewModel();
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
