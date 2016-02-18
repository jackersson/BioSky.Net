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
  public class UserPhotoViewModel : Screen, IUpdatable
  {
    public UserPhotoViewModel( IBioEngine        bioEngine, IImageUpdatable imageViewer
                             , IProcessorLocator locator  , IWindowManager  windowManager)
    {
      _locator             = locator;
      _bioEngine           = bioEngine;
      _imageViewer         = imageViewer;
      _windowManager       = windowManager;
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      _database            = _locator.GetProcessor<IBioSkyNetRepository>();

      DisplayName = "Photo";

      _serviceManager = locator.GetProcessor<IServiceManager>();

      UserImages      = new AsyncObservableCollection<Photo>();
      IsItemThumbnail = new ObservableCollection<bool>()      ;

      _bioUtils = new BioImageUtils();
      
      _enroller = new Enroller(_captureDeviceEngine, _serviceManager);

      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();

      CaptureDeviceConnected = false;


      _database.Persons.DataChanged += RefreshData;
      _database.PhotoHolder.DataChanged += RefreshData;      
    }  

    #region Update
    public void Update(Person user)
    {
      if (user == null)
        return;

      if (user.EntityState == EntityState.Added)
        return;

      IsEnabled = true;
      _user = user;

      RefreshData();
    }

    #endregion

    #region Database
    private void RefreshData()
    {
      if (_user == null)
        return;

      IList<Photo> list = _database.PhotoHolder.Data.Where(  x=>x.Personid == _user.Id 
                                                          && x.OriginType == PhotoOriginType.Loaded).ToList();

      if (list == null)
        return;

      UserImages.Clear();
     
      foreach (Photo personPhoto in list)
      {
        UserImages.Add(personPhoto);
        if (personPhoto.Id == _user.Thumbnailid)
          IsItemThumbnail.Add(true);
        else
          IsItemThumbnail.Add(false);
      }
    }

    #endregion

    #region BioService

    #region CaptureDevices
    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CaptureDevicesNames);
      if (ActiveCaptureDevice == null)
        NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }
    protected override void OnActivate()
    {
      CaptureDeviceConnected = false;
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;
      RefreshData();
      base.OnActivate();
    }

    protected override void OnDeactivate(bool close)
    {
      CaptureDeviceConnected = false;

      if (ActiveCaptureDevice != null)
        _captureDeviceEngine.Unsubscribe(OnNewFrame, ActiveCaptureDevice);

      SelectedCaptureDevice = null;
      //_imageViewer.Clear();

      CaptureDevicesNames.CollectionChanged -= CaptureDevicesNames_CollectionChanged;

      base.OnDeactivate(close);
    }
    public void EnrollFromCamera()
    {
      if (CaptureDeviceConnected && !_enroller.Busy)
      {
        _serviceManager.FaceService.EnrollFeedbackChanged += FaceService_EnrollFeedbackChanged;
        EnrollmentData data = new EnrollmentData();
        _enroller.Start(ActiveCaptureDevice, data);
      }

      if (!CaptureDeviceConnected)      
        MessageBox.Show("Choose CaptureDevice first ! ");      

      if (_enroller.Busy)      
        MessageBox.Show("Wait for finnishing previous operation");
      
    }
    public void Subscribe()
    {
      CaptureDeviceConnected = false;
      if (SelectedCaptureDevice == null)
        return;

      if (ActiveCaptureDevice != null)
        _captureDeviceEngine.Unsubscribe(OnNewFrame, ActiveCaptureDevice);

      ActiveCaptureDevice = SelectedCaptureDevice;

      if (!_captureDeviceEngine.CaptureDeviceActive(ActiveCaptureDevice))
        _captureDeviceEngine.Add(ActiveCaptureDevice);

      _captureDeviceEngine.Subscribe(OnNewFrame, ActiveCaptureDevice);
    }
    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      if (bitmap == null)
        return;

      CaptureDeviceConnected = true;

      _imageViewer.UpdateImage(ref bitmap);
    }
    #endregion  

    private async void FaceService_EnrollFeedbackChanged(object sender, EnrollmentFeedback feedback)
    {
      
      if (feedback.Progress == 100)
      {
        _serviceManager.FaceService.EnrollFeedbackChanged -= FaceService_EnrollFeedbackChanged;

        NewPhoto = new Photo();
        NewPhoto = _enroller.GetImage();
        Photo feedbackPhoto = feedback.Photo;


        if (NewPhoto != null && feedbackPhoto != null)
        {
          PersonList personList = new PersonList();
          feedbackPhoto.EntityState = EntityState.Added    ;
          feedbackPhoto.Description = NewPhoto.Description ;         
          feedbackPhoto.Personid    = _user.Id             ;
          feedbackPhoto.SizeType    = PhotoSizeType.Full   ;
          feedbackPhoto.OriginType  = PhotoOriginType.Loaded;

          Person personWithPhoto = new Person() { Id = _user.Id };
          personWithPhoto.Photos.Add(feedbackPhoto);

          personList.Persons.Add(personWithPhoto);

          //_database.PhotoHolder.DataUpdated += PhotoHolder_DataUpdated;

          try
          {
            await _serviceManager.DatabaseService.PersonUpdate(personList);
          }          
          catch (RpcException e)
          {
            Console.WriteLine(e.Message);            
          }
        }

      }
      if (_imageViewer != null)
        _imageViewer.ShowProgress(feedback.Progress, feedback.Success);      
    }

    public async Task PhotoDeletePerformer()
    {
      PersonList personList = new PersonList();

      Person person = new Person()
      {
        EntityState = EntityState.Unchanged
        , Id        = _user.Id
      };

      Photo photo = SelectedItem;
      photo.EntityState = EntityState.Deleted;

      person.Photos.Add(photo);
      personList.Persons.Add(person);   

      try
      {
        _database.Persons.DataUpdated += UpdateData;
        await _serviceManager.DatabaseService.PersonUpdate(personList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }
    }

    private void UpdateData(PersonList list)
    {
      _database.Persons.DataUpdated -= UpdateData;

      if (list != null)
      {
        Person person = list.Persons.FirstOrDefault();
        if (person != null)
        {
          foreach (Photo photo in person.Photos)
          {
            if(photo.Id == _user.Thumbnail.Id)            
              _imageViewer.UpdateImage(null, null);             
          }

          if (person.Photos.Count > 1)
            MessageBox.Show(person.Photos.Count + " photos successfully Deleted");
          else
            MessageBox.Show("Photo successfully Deleted");
        }
      }
    }

    public async Task ThumbnailUpdatePerformer()
    {
      _user.EntityState = EntityState.Modified;
      _user.Thumbnailid = SelectedItem.Id;

      PersonList personList = new PersonList();
      personList.Persons.Add(_user);

      try
      {
        _database.Persons.DataUpdated += UpdateThumbnail;
        await _serviceManager.DatabaseService.PersonUpdate(personList);
      }
      catch (RpcException e)
      {
        Console.WriteLine(e.Message);
      }
    }

    private void UpdateThumbnail(PersonList list)
    {
      _database.Persons.DataUpdated -= UpdateData;

      if (list != null)
      {
        Person person = list.Persons.FirstOrDefault();
        if (person != null)        
          MessageBox.Show("Thumbnail successfully updated");
        
      }
    }

    #endregion

    #region Interface
    public void Apply()
    {

    }
    public void EnrollFromPhoto()
    {
      if (!_enroller.Busy)
      {
        Photo photo = _imageViewer.UploadPhoto();

        if (photo == null || photo.Description.Length <= 0)
        {
          MessageBox.Show("Upload New photo");
          return;
        }

        _serviceManager.FaceService.EnrollFeedbackChanged += FaceService_EnrollFeedbackChanged;
        EnrollmentData data = new EnrollmentData();
        _enroller.Start(photo, data);       
      }
      else     
        MessageBox.Show("Wait for finnishing previous operation");
    }
    public void OnSelectionChange()
    {
      /*
      if (SelectedItem != null)
        _imageViewer.UpdateImage(SelectedItem, _database.LocalStorage.LocalStoragePath);*/
    }

    public void UploadClick()
    {
      Photo photo = _imageViewer.UploadPhoto();      
    }   
   
    public async void DeletePhoto()
    {
      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());

      if (result == true)
      {
        if (SelectedItem == null)
          return;

        try
        {
          await PhotoDeletePerformer();
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        } 
      } 
    } 

    public void Remove(bool all)
    {
      if(all)
      {
        PhotoList photoList = new PhotoList();

        foreach(Photo photo in UserImages)
        {
          photo.EntityState = EntityState.Deleted;
          photoList.Photos.Add(photo);
        }
      }
    }

    public void OnMouseRightButtonDown(Photo photo)
    {
      CanSetThumbnail = (photo != null);
      SelectedItem = photo;
    }

    public async void OnSetThumbnail()
    {
      _imageViewer.UpdateImage(SelectedItem, _database.LocalStorage.LocalStoragePath);
      _user.Thumbnailid = SelectedItem.Id;
      await ThumbnailUpdatePerformer();
    }

    #endregion

    #region UI

    public BitmapSource CaptureDeviceConnectedIcon
    {
      get { return CaptureDeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }
    public string AvaliableDevicesCount
    {
      get { return String.Format("Available Devices ({0})", _captureDevicesNames.Count); }
    }

    private ObservableCollection<bool> _isItemThumbnail;
    public ObservableCollection<bool> IsItemThumbnail
    {
      get { return _isItemThumbnail; }
      set
      {
        if (_isItemThumbnail != value)
        {
          _isItemThumbnail = value;

          NotifyOfPropertyChange(() => IsItemThumbnail);
        }
      }
    }

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

    private string _activeCaptureDevice;
    public string ActiveCaptureDevice
    {
      get { return _activeCaptureDevice; }
      set
      {
        if (_activeCaptureDevice != value)
        {
          _activeCaptureDevice = value;

          NotifyOfPropertyChange(() => ActiveCaptureDevice);
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
    
    private AsyncObservableCollection<string> _captureDevicesNames;
    public AsyncObservableCollection<string> CaptureDevicesNames
    {
      get { return _captureDevicesNames; }
      set
      {
        if (_captureDevicesNames != value)
        {
          _captureDevicesNames = value;

          NotifyOfPropertyChange(() => CaptureDevicesNames);
        }
      }
    }

    private Photo _newPhoto;
    public Photo NewPhoto
    {
      get { return _newPhoto; }
      set
      {
        if (_newPhoto != value)
        {
          _newPhoto = value;

          NotifyOfPropertyChange(() => NewPhoto);
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

          NotifyOfPropertyChange(() => SelectedItem);
          NotifyOfPropertyChange(() => CanDeleteItem);
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

    private bool _captureDeviceConnected;
    public bool CaptureDeviceConnected
    {
      get { return _captureDeviceConnected; }
      set
      {
        if (_captureDeviceConnected != value)
        {
          _captureDeviceConnected = value;
          NotifyOfPropertyChange(() => CaptureDeviceConnected);
          NotifyOfPropertyChange(() => CaptureDeviceConnectedIcon);
        }
      }
    }

    private string _selectedCaptureDevice;
    public string SelectedCaptureDevice
    {
      get { return _selectedCaptureDevice; }
      set
      {
        if (_selectedCaptureDevice != value)
        {
          _selectedCaptureDevice = value;
          NotifyOfPropertyChange(() => SelectedCaptureDevice);

          Subscribe();
        }
      }
    }

    #endregion

    #region Global Variables

    private readonly Enroller                 _enroller           ;
    private readonly IProcessorLocator        _locator            ;
    private readonly IBioEngine               _bioEngine          ;
    private readonly ICaptureDeviceEngine     _captureDeviceEngine;
    private          IWindowManager           _windowManager      ;
    private readonly IImageUpdatable          _imageViewer        ;
    private readonly IServiceManager          _serviceManager     ;
    private readonly IBioSkyNetRepository     _database           ;
    private BioContracts.Common.BioImageUtils _bioUtils           ;

    #endregion

  }
}
