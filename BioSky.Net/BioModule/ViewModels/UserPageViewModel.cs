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
using BioContracts.Services;

namespace BioModule.ViewModels
{
  enum UserPageMode
  {
     NewUser
   , ExistingUser
  }
  public class UserPageViewModel : Conductor<IScreen>.Collection.OneActive
  {   
    public UserPageViewModel(IProcessorLocator locator) : base()
    {
      _locator    = locator;
          
      _bioService = _locator.GetProcessor<IServiceManager>().DatabaseService;
      _database   = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier   = _locator.GetProcessor<INotifier>();

      CurrentPhotoImageView = new PhotoImageViewModel(_locator);
      UserPhotoView         = new UserPhotoViewModel (CurrentPhotoImageView, _locator);

      _bioUtils = new BioContracts.Common.BioImageUtils();

      Items.Add(new UserInformationViewModel    ());
      Items.Add(new UserContactlessCardViewModel(_locator));
      Items.Add(UserPhotoView);
     
      ActiveItem = Items[0];
      OpenTab();
      
      _methodInvoker = new FastMethodInvoker();     

      DisplayName = "AddNewUser";
    }

    #region Update

    private bool ContainRequiredFields(Person user)
    {
      if (user == null)
        return false;

      return (user.Firstname != "" && user.Firstname != "");
    }

    private Person ResetUser(Person user)
    {
      if (user == null)
        return null;

      user.Firstname   = "";
      user.Lastname    = "";
      user.Thumbnailid = user.Thumbnailid <= 0 ? 0 : user.Thumbnailid;
      user.Gender      = Person.Types.Gender.Male;
      user.Rights      = Person.Types.Rights.Operator;
     
      _userPageMode = UserPageMode.NewUser;
      _revertUser   = null;
      DisplayName = LocExtension.GetLocalizedValue<string>("BioModule:lang:AddNewUser");

      return user;  
    }

    public void Update(Person user)
    {      
      if (user != null )
      {
        _user = user.Clone();
        if (ContainRequiredFields(user))
        {
          _revertUser = user.Clone();
          _userPageMode = UserPageMode.ExistingUser;
          DisplayName = (_user.Firstname + " " + _user.Lastname);
        }
        else        
          _user = ResetUser(user);                   
      }
      else      
        _user = ResetUser(new Person());      

      Photo photo = _database.PhotoHolder.GetValue(_user.Thumbnailid);
      CurrentPhotoImageView.UpdateImage(photo, _database.LocalStorage.LocalStoragePath);

      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { _user });
    }
    
    #endregion
    
    #region Interface

    public async void Apply()
    {
      var result = false;//_windowManager.ShowDialog(DialogsHolder.AreYouSureDialog);

      if (!result )
        return;  
      
      try
      {
        if (_userPageMode == UserPageMode.ExistingUser)
          await _bioService.PersonDataClient.Update(_user);
        else
          await _bioService.PersonDataClient.Add(_user);      
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }       
    }

    public void Revert()
    {
      if ( _revertUser != null)     
        Update(_revertUser);
    }

    public async void Remove()
    {
      var result = false;// _windowManager.ShowDialog(DialogsHolder.AreYouSureDialog);

      if (!result)
        return;
      
      try
      {
        await _bioService.PersonDataClient.Delete(_user);      
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
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
    private readonly INotifier                _notifier     ;
    private UserPageMode                      _userPageMode ;
    private IBioSkyNetRepository              _database     ;
    private readonly IDatabaseService         _bioService   ;

    #endregion
  }
}
