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
using System.ComponentModel;
using BioContracts.BioTasks.Utils;

namespace BioModule.ViewModels
{
  public enum UserPageMode
  {
    NewUser
   , ExistingUser
  }

  public interface IUserUpdatable
  {
    void Update(Person user);
  }

  public class UserPageViewModel : Conductor<IScreen>.Collection.OneActive, IUserUpdatable
  {   
    public UserPageViewModel(IProcessorLocator locator) : base()
    {
      _locator    = locator;

      _dialogs    = _locator.GetProcessor<DialogsHolder>();
      _bioService = _locator.GetProcessor<IServiceManager>().DatabaseService;
      _database   = _locator.GetProcessor<IBioSkyNetRepository>();
      _notifier   = _locator.GetProcessor<INotifier>();

      CurrentPhotoImageView = new BioImageViewModel(_locator);

      CurrentPhotoImageView.BioImageModelChanged += OnBioImageModelChanged;

      UserPhotoView = new UserPhotoViewModel (CurrentPhotoImageView, _locator);
      
      _bioUtils = new BioImageUtils();

      UserInformationViewModel uinfoModel = new UserInformationViewModel(_locator, CurrentPhotoImageView);

      uinfoModel.PropertyChanged        += UserDataChanged;
      uinfoModel.ValidationStateChanged += UinfoModel_ValidationStateChanged;

      Items.Add(uinfoModel);
      Items.Add(new UserContactlessCardViewModel(_locator, CurrentPhotoImageView));
      Items.Add(UserPhotoView);
      Items.Add(new UserFingerViewModel(CurrentPhotoImageView));
      Items.Add(new UserIrisViewModel(CurrentPhotoImageView));

      ActiveItem = Items[0];
      OpenTab();
      
      _methodInvoker = new FastMethodInvoker();     

      DisplayName = "AddNewUser";

      _database.Persons.DataChanged += Persons_DataChanged;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      Update(_user);
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
    }

    private void UserDataChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {  
      NotifyOfPropertyChange(() => CanApply );  
      NotifyOfPropertyChange(() => CanRevert);  
    }


    private void UinfoModel_ValidationStateChanged(bool state)
    {
      IsValid = state;      
      NotifyOfPropertyChange(() => CanRevert);
    }

    private void Persons_DataChanged()
    {
      if (_userPageMode == UserPageMode.ExistingUser)
      {        
        Person user = _database.Persons.GetValue(_user.Id);
        Update(user);
      }      
      else
      {
        Person user = _database.Persons.GetValue(_user);
        Update(user);
      }
    }   

    #region Update

    private void OnBioImageModelChanged(PhotoViewEnum bioImageModel)
    {

      foreach(IScreen item in Items)
      {
        if(item is IUserBioItemsController)
        {
          IUserBioItemsController item2 = item as IUserBioItemsController;
          if (item2.PageEnum == bioImageModel)
            ActivateItem(item);
        }

      }
    }
    private bool ContainRequiredFields(Person user)
    {
      if (user == null)
        return false;

      return (user.Firstname != "" && user.Lastname != "");
    }

    private Person ResetUser(Person user)
    {
      if (user == null)
        return null;
      
      user.Firstname   = "";
      user.Lastname    = "";
      //user.Thumbnailid = user.Thumbnailid <= 0 ? 0 : user.Thumbnailid;
      user.Gender      = Person.Types.Gender.Male;
      user.Rights      = Person.Types.Rights.Operator;
     
      _userPageMode = UserPageMode.NewUser;
      //_user         = null;
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

      NotifyOfPropertyChange(() => CanDelete);
      
      CurrentPhotoImageView.UpdateFromPhoto(_user.Thumbnail);

      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { _user });
    }

    #endregion

    #region Interface            
    public async void Apply()
    {

      if (!CurrentPhotoImageView.IsValid)
      {
        _dialogs.CustomTextDialog.Update("Warning", "Upload personal photo from File or Camera, please!", DialogStatus.Error);
        _dialogs.CustomTextDialog.Show();
        return;
      }
      
      bool? result = _dialogs.AreYouSureDialog.Show();
   
      if ( !result.HasValue || (!result.Value ))
        return;  
            
      try
      {
        if (_userPageMode == UserPageMode.ExistingUser)
          await _bioService.PersonDataClient.Update(_user);
        else
        {
          Photo thumbnail = CurrentPhotoImageView.CurrentPhoto;          
          _user.Thumbnail = thumbnail;
          _user.Thumbnail.Personid = _user.Id;
          _user.Thumbnail.Datetime = DateTime.Now.Ticks;         
          
          await _bioService.PersonDataClient.Add(_user);
        }    
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }       
    }

    public void Revert()
    {
      if (_user != null)     
        Update(_revertUser);
    }

    public async void Remove()
    {
      bool? result = _dialogs.AreYouSureDialog.Show();

      if (!result.HasValue || (!result.Value))
        return;

      try
      {       
        Person personToDelete = new Person() { Id = _user.Id };
        await _bioService.PersonDataClient.Remove(_user);      
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }       
    }

    #endregion

    #region UI

    
    public override int GetHashCode()
    {   
      return _user != null ?_user.Id.GetHashCode() : DisplayName.GetHashCode();
    }
    
    public UserPageMode GetUserPageMode()
    {
      return _userPageMode;
    }

    public bool CanApply
    {
      get {       
        return IsActive && IsValid  && ( _userPageMode == UserPageMode.NewUser || CanRevert)  ;
      }
    }

    public bool CanRevert
    {
      get { return IsActive && _userPageMode == UserPageMode.ExistingUser && !_user.Equals(_revertUser); }
    }

    public bool CanDelete
    {
      get { return IsActive && _user != null && _user.Id > 0; }
    }

    private bool _isValid;
    public bool IsValid
    {
      get { return _isValid; }
      set
      {
        if ( _isValid != value)
        {
          _isValid = value;
          NotifyOfPropertyChange(() => IsValid  );
          NotifyOfPropertyChange(() => CanApply);
        }
      }
    }

    public void OpenTab()
    {      
      CurrentPhotoImageView.ActivateWith(this);      
      ActiveItem.Activate();
    }

    private BioImageViewModel _currentPhotoImageView;
    public BioImageViewModel CurrentPhotoImageView
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
    private readonly DialogsHolder            _dialogs      ;
    private          BioImageUtils            _bioUtils     ;
    private readonly FastMethodInvoker        _methodInvoker;
    private readonly IProcessorLocator        _locator      ;
    private readonly INotifier                _notifier     ;
    private UserPageMode                      _userPageMode ;
    private IBioSkyNetRepository              _database     ;
    private readonly IDatabaseService         _bioService   ;

    #endregion
  }
}
