﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Caliburn.Micro;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Drawing;
using System.Reflection;

using BioModule.ResourcesLoader;
using System.ComponentModel;
using System.Collections.ObjectModel;

using BioData;
using System.Windows.Input;
using System.Windows.Data;

using BioModule.Utils;
using BioService;
using BioContracts;
using Google.Protobuf.Collections;
using Grpc.Core;
using WPFLocalizeExtension.Extensions;
using BioContracts.Services;

namespace BioModule.ViewModels
{
  public class UsersViewModel : Screen
  {
    public UsersViewModel(IProcessorLocator locator)
    {
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:Users_");

      _locator       = locator;     
      _bioService    = _locator.GetProcessor<IServiceManager>().DatabaseService;
      _database      = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier      = _locator.GetProcessor<INotifier>();
      _selector = _locator.GetProcessor<ViewModelSelector>();
      _selectedPersons = new ObservableCollection<Person>();
      PageController   = new PageControllerViewModel();

      _database.Persons.DataChanged      += RefreshData;      
      //_database.PhotoHolder.DataChanged  += RefreshData;

      IsDeleteButtonEnabled = false;   
    }
    #region Database
    private void RefreshData()
    {
      if (!IsActive)
        return;

      Users = null;
      Users = _database.Persons.Data;

      if (Users == null || Users.Count <= 0)
        return;

      UsersCollectionView = null;
      UsersCollectionView = new PagingCollectionView(Users, PAGES_COUNT);    

      PageController.UpdateData(UsersCollectionView);     
    }  

    #endregion    
   

    #region Interface
    public async void OnDeleteUsers()
    {
      var result = false;// _windowManager.ShowDialog(DialogsHolder.AreYouSureDialog);
      
      if (result == false)
        return;

      try
      {
        await _bioService.PersonDataClient.Delete(SelectedPersons);
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }      
    }



    public void OnSelectionChanged(SelectionChangedEventArgs e)
    {
      IList selectedRecords   = e.AddedItems as IList;
      IList unselectedRecords = e.RemovedItems as IList;

      if (selectedRecords == null || unselectedRecords == null)
        return;

      foreach (Person currentUser in selectedRecords)      
        SelectedPersons.Add(currentUser);      

      foreach (Person currentUser in unselectedRecords)
        SelectedPersons.Remove(currentUser);  
           
      IsDeleteButtonEnabled = (SelectedPersons.Count >= 1) ? true : false;
     
    }
    protected override void OnActivate()
    {
      base.OnActivate();
      RefreshData();
    }
    public void ShowUserPage(bool isExistingUser)
    {

      if (!isExistingUser)
      {
        _selector.ShowContent(ShowableContentControl.TabControlContent
                             , ViewModelsID.UserPage
                             , new object[] { null });
      }
      else
      {
        foreach (Person person in SelectedPersons)
        {
          _selector.ShowContent(ShowableContentControl.TabControlContent
                                , ViewModelsID.UserPage
                                , new object[] { person });

        }
      }
    }
    public void OnSearchTextChanged(string SearchText)
    {     
      UsersCollectionView.Filtering = item =>
      {
        Person vitem = item as Person;
        if (vitem == null) return false;

        if (String.IsNullOrEmpty(SearchText))
          return true;

        if ((item as Person).Firstname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
            (item as Person).Lastname.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0)        
          return true;
        
        return false;
      };
    }
    public void OnMouseRightButtonDown(Person user)
    {
      CanOpenInNewTab = (user != null);
      SelectedItem = user;
    }

    public void OnMouseDoubleClick(Person user)
    {
      if (user == null)
        return;
      
      _selector.ShowContent(ShowableContentControl.TabControlContent, ViewModelsID.UserPage
                           , new object[] { user });      
    }
    #endregion

    #region UI
    private IPagingCollectionView _usersCollectionView;
    public IPagingCollectionView UsersCollectionView
    {
      get { return _usersCollectionView; }
      set
      {
        if (_usersCollectionView != value)
        {
          _usersCollectionView = value;

          NotifyOfPropertyChange(() => UsersCollectionView);
        }
      }
    }

    private PageControllerViewModel _pageController;
    public PageControllerViewModel PageController
    {
      get { return _pageController; }
      set
      {
        if (_pageController != value)
        {
          _pageController = value;
          NotifyOfPropertyChange(() => PageController);
        }
      }
    }

    private Person _selectedItem;
    public Person SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem != value)
        {
          _selectedItem = value;
          NotifyOfPropertyChange(() => SelectedItem);
        }
      }
    }

    private bool _canOpenInNewTab;
    public bool CanOpenInNewTab
    {
      get { return _canOpenInNewTab; }
      set
      {
        if (_canOpenInNewTab != value)
        {
          _canOpenInNewTab = value;
          NotifyOfPropertyChange(() => CanOpenInNewTab);
        }
      }
    }

    private AsyncObservableCollection<Person> _users;
    public AsyncObservableCollection<Person> Users
    {
      get { return _users; }
      set
      {
        if (_users != value)
        {
          _users = value;
          NotifyOfPropertyChange(() => Users);
        }
      }
    }

    private bool _isSelected;
    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        if (_isSelected != value)
        {
          _isSelected = value;
          NotifyOfPropertyChange(() => IsSelected);
        }
      }
    }

    private bool _isDeleteButtonEnabled;
    public bool IsDeleteButtonEnabled
    {
      get { return _isDeleteButtonEnabled; }
      set
      {
        if (_isDeleteButtonEnabled != value)
        {
          _isDeleteButtonEnabled = value;
          NotifyOfPropertyChange(() => IsDeleteButtonEnabled);
        }
      }
    }

    private ObservableCollection<Person> _selectedPersons;
    public ObservableCollection<Person> SelectedPersons
    {
      get { return _selectedPersons; }
      set
      {
        if (_selectedPersons != value)
        {
          _selectedPersons = value;
          NotifyOfPropertyChange(() => SelectedPersons);
        }
      }
    }


    #endregion

    #region Global Variables  
    private readonly IProcessorLocator    _locator   ;
    private readonly ViewModelSelector    _selector  ;   
    private readonly IDatabaseService     _bioService;
    private readonly IBioSkyNetRepository _database  ;
    private readonly INotifier            _notifier  ;

    private int PAGES_COUNT = 10;
    #endregion
  } 
}
