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

using BioModule.Utils;
using BioData;
using System.Reflection;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using BioService;
using System.IO;
using Grpc.Core;

using WPFLocalizeExtension.Extensions;
using WPFLocalizeExtension.Providers;
using XAMLMarkupExtensions.Base;

namespace BioModule.ViewModels
{
  enum UserPageMode
  {
     NewUser
   , ExistingUser
  }
  public class UserPageViewModel : Conductor<IScreen>.Collection.OneActive
  {   
    public UserPageViewModel(IProcessorLocator locator, IWindowManager windowManager) : base()
    {
      _locator = locator;
      _windowManager = windowManager;

      IBioEngine bioEngine = _locator.GetProcessor<IBioEngine>();
      _bioService          = _locator.GetProcessor<IServiceManager>();
      _database            = _locator.GetProcessor<IBioSkyNetRepository>();

      CurrentPhotoImageView = new PhotoImageViewModel(_locator, _windowManager);
      UserPhotoView = new UserPhotoViewModel(bioEngine, CurrentPhotoImageView, _locator, _windowManager);

      CurrentPhotoImageView.FeedbackPhotoReceive += UserPhotoView.GetFeedBackPhoto;
      //CurrentImageView.EnrollFromCameraChanged += UserPhotoView.EnrollFromCamera;

      _bioUtils = new BioContracts.Common.BioImageUtils();

      Items.Add(new UserInformationViewModel    ());
      Items.Add(new UserContactlessCardViewModel(bioEngine, _locator, _windowManager));
      Items.Add(UserPhotoView);
     
      ActiveItem = Items[0];
      OpenTab();
      
      _methodInvoker = new FastMethodInvoker();     

      DisplayName = "AddNewUser";
    }

    #region Update

    public void Update(Person user)
    {
      if (user != null && user.Thumbnail != null)
      {
        //Add User as Visitor
        UpdateFromVisitor(user);
        return;
      }

      if (user != null)
      {
        _user = user.Clone();
        _revertUser = user.Clone();
        _userPageMode = UserPageMode.ExistingUser;

       
        Photo photo = _database.PhotoHolder.GetValue(_user.Thumbnailid);

        CurrentPhotoImageView.UpdateImage(photo, _database.LocalStorage.LocalStoragePath);

        DisplayName = (_user.Firstname + " " + _user.Lastname);
      }
      else
      {
        _user = new Person()
        {
            Firstname = ""
          , Lastname = ""
          , Thumbnailid = 0
          , Gender = Person.Types.Gender.Male
          , Rights = Person.Types.Rights.Operator
          , EntityState = EntityState.Added
        };

        _userPageMode = UserPageMode.NewUser;
        DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:AddNewUser");

        //DisplayName = "AddNewUser";
        CurrentPhotoImageView.UpdateImage(null, null);

      }

      //CurrentImageView.Update(_user);

      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { _user });
    }
    
    public void UpdateFromVisitor(Person user)
    {
      _user = user.Clone();
      _revertUser = user.Clone();
      CurrentPhotoImageView.UpdateImageFromPath(_database.LocalStorage.LocalStoragePath + user.Thumbnail.FileLocation);
     // CurrentImageView.Update(_user);
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:AddNewUser");
    }


    #endregion
    
    #region Database




    #endregion

    #region BioService

    public async Task UserUpdatePerformer(EntityState state)
    {
      _user.EntityState = state;     

      if(_user.EntityState != EntityState.Deleted)
      {
        Photo photo = CurrentPhotoImageView.CurrentImagePhoto;
        if (photo != null)
        {
          photo.OriginType = PhotoOriginType.Loaded;

          Photo thumbnail = _database.PhotoHolder.GetValue(_user.Thumbnailid);
          if (thumbnail != null)
          {
            if (thumbnail.GetHashCode() != photo.GetHashCode())
              _user.Thumbnail = photo;
          }
          else
            _user.Thumbnail = photo;

        } 
      }   


      PersonList personList = new PersonList();
      personList.Persons.Add(_user);   

      try
      {
        _database.Persons.DataUpdated += UpdateData;
        _database.PhotoHolder.DataChanged += UpdatePhoto;

        await _bioService.DatabaseService.PersonUpdate(personList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }      
    }

    public void UpdatePhoto()
    {
      _database.PhotoHolder.DataChanged -= UpdatePhoto;

      Photo photo = _database.PhotoHolder.GetValue(_user.Thumbnailid);

      CurrentPhotoImageView.UpdateImage(photo, _database.LocalStorage.LocalStoragePath);
    }
    
    private void UpdateData(PersonList list)
    {
      _database.Persons.DataUpdated -= UpdateData;

      if (list != null)
      {
        Person person = list.Persons.FirstOrDefault();
        if (person != null)
        {
          if (person.EntityState == EntityState.Deleted)
          {
            person = null;
            MessageBox.Show("User successfully Deleted");
          }
          else if (person.EntityState == EntityState.Added)
          {
            MessageBox.Show("User successfully Added");
          }
          else
          {
            MessageBox.Show("User successfully Updated");
          }

          Update(person);
        }
      }
    }   

    #endregion

    #region Interface

    public async void Apply()
    {   

      var result = _windowManager.ShowDialog(DialogsHolder.AreYouSureDialog);

      if (result == true)     
      {
        try
        {
          await UserUpdatePerformer((_userPageMode == UserPageMode.NewUser) ? EntityState.Added : EntityState.Modified); 
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        } 
      }
    }

    public void Revert()
    {
      if (_revertUser.EntityState == EntityState.Added)
        Update(null);
      else
        Update(_revertUser);
    }


    public async void Remove()
    {
      var result = _windowManager.ShowDialog(DialogsHolder.AreYouSureDialog);

      if (result == true)
      {
        try
        {
          await UserUpdatePerformer(EntityState.Deleted);
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        } 
      }   
    }

    #endregion

    #region UI
    public void OpenTab()
    {
      ActiveItem.Activate();
    }

    private PhotoImageViewModel _currentPhotoImageView;
    public PhotoImageViewModel CurrentPhotoImageView
    {
      get { return _currentPhotoImageView; }
      private set
      {
        if (_currentPhotoImageView != value)
        {
          _currentPhotoImageView = value;
          NotifyOfPropertyChange(() => CurrentPhotoImageView);
        }
      }
    }

    private UserPhotoViewModel _userPhotoView;
    public UserPhotoViewModel UserPhotoView
    {
      get { return _userPhotoView; }
      private set
      {
        if (_userPhotoView != value)
        {
          _userPhotoView = value;
          NotifyOfPropertyChange(() => UserPhotoView);
        }
      }
    }

    #endregion

    #region Global Variables

    private Person                            _revertUser   ;
    private Person                            _user         ;
    private BioContracts.Common.BioImageUtils _bioUtils     ;
    private readonly FastMethodInvoker        _methodInvoker;
    private readonly IProcessorLocator        _locator      ;
    private IWindowManager                    _windowManager;
    private UserPageMode                      _userPageMode ;
    private IBioSkyNetRepository              _database     ;
    private readonly IServiceManager          _bioService   ;
    #endregion
  }
}
