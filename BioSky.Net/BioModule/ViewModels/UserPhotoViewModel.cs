using System;
using Caliburn.Micro;
using BioContracts;
using BioModule.Utils;

using BioService;
using BioContracts.Services;
using BioContracts.Common;
using WPFLocalizeExtension.Extensions;
using BioContracts.BioTasks;
using BioContracts.BioTasks.Utils;

namespace BioModule.ViewModels
{

  public class UserPhotoViewModel : Screen, IUpdatable, IUserBioItemsController
  {
    public UserPhotoViewModel( IUserBioItemsUpdatable   imageViewer, 
                               IProcessorLocator locator     )
    {
      _locator             = locator      ;    
      _imageViewer         = imageViewer  ;
      _imageViewer.UpdateBioItemsController(this);
      //_imageViewer.U

      _database            = _locator.GetProcessor<IBioSkyNetRepository>();
      _bioService          = _locator.GetProcessor<IServiceManager>().DatabaseService    ;
      _dialogsHolder       = _locator.GetProcessor<DialogsHolder>();
      _notifier            = _locator.GetProcessor<INotifier>();

      DisplayName = "Photo";      

      UserImages   = new AsyncObservableCollection<Photo>();
      _bioUtils    = new BioImageUtils();
      
      _database.Persons.DataChanged     += RefreshData;
      //_database.PhotoHolder.DataChanged += RefreshData;  
      PhotoAvailableText = LocExtension.GetLocalizedValue<string>("BioModule:lang:NoAvailablePhotos");

      IsEnabled = true;
    }

    #region Update

    protected override void OnActivate()
    {
      base.OnActivate();
      RefreshData();

      _imageViewer.ChangeBioImageModel(PageEnum);
    }

    public void Update(Person user)
    {
      if (user == null || (user != null && user.Id <= 0) )
        return;
      
      _user = user;
      RefreshData();

      IsEnabled = true;      
    }

    #endregion

    #region Database
    private void RefreshData()
    {
      if (!IsActive || _user == null)
        return;
     
      UserImages.Clear();
     
      foreach (Photo personPhoto in _user.Photos)      
        UserImages.Add(personPhoto);
      
      if(UserImages.Count > 0)      
        PhotoAvailableText = LocExtension.GetLocalizedValue<string>("BioModule:lang:YourPhotos");      
      else
        PhotoAvailableText = LocExtension.GetLocalizedValue<string>("BioModule:lang:NoAvailablePhotos");
    }

    #endregion  

    #region Interface
    public void Apply(){}     

    public void OnMouseRightButtonDown(Photo photo)
    {
      CanSetThumbnail = (photo != null);
      SelectedItem = photo;
    }

    public void OnDeletePhoto()
    {
      if (SelectedItem == null)
        return;

      Remove(SelectedItem);
    }

    public async void OnSetThumbnail()
    {
      try {
        Photo currentPhoto = _imageViewer.CurrentPhoto;

        if (currentPhoto == null)
          return;

        if (currentPhoto.Id > 0)
        {
          Photo requested = new Photo() { Id = currentPhoto.Id };
          await _bioService.ThumbnailDataClient.SetThumbnail(User, requested);
        }
      }
      catch (Exception e) {
        _notifier.Notify(e);
      }      
    }

    public async void Add(Photo photo)
    {          
      var result = _dialogsHolder.AreYouSureDialog.Show();

      if (!result.HasValue || !result.Value || photo == null)
        return;
      
      photo.OwnerId  = User.Id;
      photo.Datetime = DateTime.Now.Ticks;

      try {
        await _bioService.PhotosDataClient.Add(User.Id, photo);
      }
      catch (Exception e) {
        _notifier.Notify(e);
      }
    }

    public async void Remove(Photo photo)
    {
      if (photo == null)
        return;

      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (!result || photo == null)
        return;

      try {
        await _bioService.PhotosDataClient.Remove(User.Id, photo);
      }
      catch (Exception e) {
        _notifier.Notify(e);
      }
    }
           
    public void Next()
    {
      Console.WriteLine("OnNext");
      return;


      if (!CanNext || CurrentPhotoIndex + 1 > UserImages.Count )
        return;

      SelectedItem = UserImages[CurrentPhotoIndex + 1];

      
    }

    public void Previous()
    {
      Console.WriteLine("OnPrevious");
      return;

      if (!CanPrevious || CurrentPhotoIndex - 1 >= 0)
        return;

      SelectedItem = UserImages[CurrentPhotoIndex - 1];
    }

    #endregion

    #region UI   
    
    public bool CanNext     { get { return CurrentPhotoIndex < UserImages.Count - 1; } }
    public bool CanPrevious { get { return CurrentPhotoIndex > 0; } }

    private bool _canSetThumbnail;
    public bool CanSetThumbnail
    {
      get { return _canSetThumbnail; }
      set
      {
        if (_canSetThumbnail != value)
        {
          _canSetThumbnail = value;
          NotifyOfPropertyChange(() => CanSetThumbnail);
        }
      }
    }

    private string _photoAvailableText;
    public string PhotoAvailableText
    {
      get { return _photoAvailableText; }
      set
      {
        if (_photoAvailableText != value)
        {
          _photoAvailableText = value;
          NotifyOfPropertyChange(() => PhotoAvailableText);
        }
      }
    }

    private bool _isEnabled;
    public bool IsEnabled
    {
      get { return _isEnabled; }
      set
      {
        if (_isEnabled != value)
        {
          _isEnabled = value;
          NotifyOfPropertyChange(() => IsEnabled);
        }
      }
    }

    private AsyncObservableCollection<Photo> _userImages;
    public AsyncObservableCollection<Photo> UserImages
    {
      get { return _userImages; }
      set
      {
        if (_userImages != value)
        {
          _userImages = value;
          NotifyOfPropertyChange(() => UserImages);
        }
      }
    }        

    private Photo _selectedItem;
    public Photo SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        if (_selectedItem != value)
        {
          _selectedItem = value;

          if(_selectedItem != null)
          {
            _imageViewer.UpdateFromPhoto(value);

            CurrentPhotoIndex = UserImages.IndexOf(_imageViewer.CurrentPhoto);
          }       

          NotifyOfPropertyChange(() => SelectedItem );
          NotifyOfPropertyChange(() => CanDeleteItem);
          NotifyOfPropertyChange(() => CanNext      );
          NotifyOfPropertyChange(() => CanPrevious  );
        }
      }
    }

    private int _currentPhotoIndex;
    private int CurrentPhotoIndex
    {
      get { return _currentPhotoIndex; }
      set
      {
        if ( _currentPhotoIndex != value )
        {
          _currentPhotoIndex = value;
          NotifyOfPropertyChange(() => CurrentPhotoIndex);
        }
      }
    }

    private bool _canDeleteItem;
    public bool CanDeleteItem
    {
      get { return SelectedItem != null; }      
    }

    private Person _user;
    public Person User
    {
      get { return _user; }
      set
      {
        if (_user != value)
        {
          _user = value;
          NotifyOfPropertyChange(() => User);
        }
      }
    }

    public BioImageModelEnum PageEnum { get { return BioImageModelEnum.Faces; } }
    #endregion

    #region Global Variables

    private readonly FaceEnroller             _enroller           ;
    private readonly IProcessorLocator        _locator            ;   
    private readonly IUserBioItemsUpdatable   _imageViewer        ;
    private readonly IDatabaseService         _bioService         ;    
    private readonly IBioSkyNetRepository     _database           ;
    private readonly INotifier                _notifier           ;
    private          BioImageUtils            _bioUtils           ;
    private readonly DialogsHolder            _dialogsHolder      ;


    #endregion
  }
}
