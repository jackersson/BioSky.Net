using System;
using System.Linq;
using Caliburn.Micro;
using BioContracts;
using BioModule.Utils;
using BioService;
using WPFLocalizeExtension.Extensions;
using BioContracts.Services;
using BioContracts.BioTasks.Utils;
using BioData.Holders.Utils;

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
      CurrentPhotoImageView.PhotoChanged += CurrentPhotoChanged;

      base.OnActivate();
      Update(_user);
    }

    protected override void OnDeactivate(bool close)
    {
      CurrentPhotoImageView.PhotoChanged -= CurrentPhotoChanged;
      base.OnDeactivate(close);
    }

    private void CurrentPhotoChanged()
    {
      NotifyOfPropertyChange(() => CanApply);
      NotifyOfPropertyChange(() => CanRevert);
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

    private void OnBioImageModelChanged(BioImageModelEnum bioImageModel)
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
      user.Gender      = Gender.Male;
      user.Rights      = Rights.Operator;
     
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

      Photo photo =_user.Photos.Where(x => x.Id == _user.Thumbnailid).FirstOrDefault();

      CurrentPhotoImageView.UpdateFromPhoto(photo);

      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { _user });
    }

    #endregion

    #region Interface            
    public async void Apply()
    {
      /*
      if(_userPageMode == UserPageMode.NewUser)
      {
        if (!CurrentPhotoImageView.IsValid)
        {
          _dialogs.CustomTextDialog.Update("Warning", "Upload personal photo from File or Camera, please!", DialogStatus.Error);
          _dialogs.CustomTextDialog.Show();
          return;
        }
      }
      */
      
      bool? result = _dialogs.AreYouSureDialog.Show();
   
      if ( !result.HasValue || (!result.Value ))
        return;  
            
      try
      {
        if (_userPageMode == UserPageMode.ExistingUser)
        {
          Person updatedUser = UserForUpdate();
          await _bioService.PersonDataClient.Update(updatedUser);
        }
        else
        {
          //Photo thumbnail = CurrentPhotoImageView.CurrentPhoto;          
          //_user.Thumbnail = thumbnail;
          //_user.Thumbnail.Personid = _user.Id;
          //_user.Thumbnail.Datetime = DateTime.Now.Ticks;         
          
          await _bioService.PersonDataClient.Add(_user);
        }    
      }
      catch (Exception e) {
        _notifier.Notify(e);
      }       
    }

    public Person UserForUpdate()
    {
      Person updatedPerson = new Person() { Id = _user.Id } ;
      #region info fields
      if ( !string.Equals(_user.Firstname, _revertUser.Firstname))
        updatedPerson.Firstname = _user.Firstname;

      if ( !string.Equals(_user.Lastname, _revertUser.Lastname) )
        updatedPerson.Lastname = _user.Lastname;

      if (_user.Dateofbirth != _revertUser.Dateofbirth)
        updatedPerson.Dateofbirth = (_user.Dateofbirth != 0) ? _user.Dateofbirth : -1;

      if (_user.Gender != _revertUser.Gender)
        updatedPerson.Gender = _user.Gender;

      if (!string.Equals(_user.Email, _revertUser.Email) )
        updatedPerson.Email = !string.IsNullOrEmpty(_user.Email) ? _user.Email : ProtoFieldsUtils.FIELD_DELETE_STATE;

      if (!string.Equals(_user.Country, _revertUser.Country))
        updatedPerson.Country = !string.IsNullOrEmpty(_user.Country) ? _user.Country : ProtoFieldsUtils.FIELD_DELETE_STATE;

      if (!string.Equals(_user.City, _revertUser.City))
        updatedPerson.City = !string.IsNullOrEmpty(_user.City) ? _user.City : ProtoFieldsUtils.FIELD_DELETE_STATE;

      if (!string.Equals(_user.Comments, _revertUser.Comments))
        updatedPerson.Comments = !string.IsNullOrEmpty(_user.Comments) ? _user.Comments : ProtoFieldsUtils.FIELD_DELETE_STATE;

      if (_user.Rights != _revertUser.Rights)
        updatedPerson.Rights = _user.Rights;

      #endregion
      /*
      Photo currentPhoto = CurrentPhotoImageView.CurrentPhoto;
      if (_user.Thumbnailid != currentPhoto.Id || currentPhoto.Id <= 0)
      {
        if(currentPhoto.Id <= 0)
        {
          //currentPhoto.Personid = _user.Id;
         // currentPhoto.Datetime = DateTime.Now.Ticks;
        }

        updatedPerson.Thumbnail = currentPhoto;
      }
      */

      return updatedPerson;
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

      try {              
        await _bioService.PersonDataClient.Remove(_user);      
      }
      catch (Exception e)
      {
        _notifier.Notify(e);
      }       
    }

    #endregion

    #region UI

    /*
    public override int GetHashCode()
    {   
      return _user != null ?_user.Id.GetHashCode() : DisplayName.GetHashCode();
    }*/
    
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
      get { return IsActive && _userPageMode == UserPageMode.ExistingUser 
                   && (!_user.Equals(_revertUser) || CurrentPhotoImageView.CurrentPhoto == null 
                   || CurrentPhotoImageView.CurrentPhoto.Id != _user.Thumbnailid || CurrentPhotoImageView.CurrentPhoto.Id <= 0); }
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
    private BioImageUtils _bioUtils     ;
    private readonly FastMethodInvoker        _methodInvoker;
    private readonly IProcessorLocator        _locator      ;
    private readonly INotifier                _notifier     ;
    private UserPageMode                      _userPageMode ;
    private IBioSkyNetRepository              _database     ;
    private readonly IDatabaseService         _bioService   ;

    #endregion
  }
}
