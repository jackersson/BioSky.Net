using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

using BioData;
using BioContracts;

using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;
using System.IO;
using AForge.Video.DirectShow;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Drawing;
using BioModule.Utils;

using BioService;
using BioContracts.Services;
using Microsoft.Win32;
using BioContracts.Common;
using System.Windows;
using Grpc.Core;
using Google.Protobuf.Collections;
using System.Collections;

namespace BioModule.ViewModels
{

  public class UserPhotoViewModel : Screen, IUpdatable, IUserPhotoController
  {
    public UserPhotoViewModel( IUserPhotoUpdatable   imageViewer, 
                               IProcessorLocator locator     )
    {
      _locator             = locator      ;    
      _imageViewer         = imageViewer  ;
      _imageViewer.UpdatePhotoController(this);
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

      IsEnabled = true;
    }

    #region Update

    protected override void OnActivate()
    {
      base.OnActivate();
      RefreshData();
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
    }

    #endregion  

    #region Interface
    public void Apply(){}     

    public void OnMouseRightButtonDown(Photo photo)
    {
      CanSetThumbnail = (photo != null);
      SelectedItem = photo;
    }

    public async void OnSetThumbnail()
    {
      try  {
        Photo requested = null;
        if (_imageViewer.CurrentPhoto.Id > 0)
        {
          requested = new Photo() { Id = _imageViewer.CurrentPhoto.Id };
          await _bioService.ThumbnailDataClient.SetThumbnail(User, requested);
        }
      }
      catch (Exception e) {
        _notifier.Notify(e);
      }      
    }

    public async void Add(Photo photo)
    {     
      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (!result || photo == null)
        return;
      
      photo.Personid = User.Id;
      photo.Datetime = DateTime.Now.Ticks;

      try {
        await _bioService.PhotosDataClient.Add(User, photo);
      }
      catch (Exception e) {
        _notifier.Notify(e);
      }
    }

    public async void Remove(Photo photo)
    {
      _dialogsHolder.AreYouSureDialog.Show();
      var result = _dialogsHolder.AreYouSureDialog.GetDialogResult();

      if (!result || photo == null)
        return;

      try {
        await _bioService.PhotosDataClient.Remove(User, photo);
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

          _imageViewer.UpdateFromPhoto(value);

          CurrentPhotoIndex = UserImages.IndexOf(_imageViewer.CurrentPhoto);          

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
    #endregion

    #region Global Variables

    private readonly Enroller                 _enroller           ;
    private readonly IProcessorLocator        _locator            ;   
    private readonly IUserPhotoUpdatable          _imageViewer        ;
    private readonly IDatabaseService         _bioService         ;    
    private readonly IBioSkyNetRepository     _database           ;
    private readonly INotifier                _notifier           ;
    private BioContracts.Common.BioImageUtils _bioUtils           ;
    private readonly DialogsHolder            _dialogsHolder      ;


    #endregion
  }
}
