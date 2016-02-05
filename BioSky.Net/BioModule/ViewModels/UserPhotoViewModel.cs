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

      UserImages = new AsyncObservableCollection<Photo>();

      _bioUtils = new BioContracts.Common.BioImageUtils();
      
      _enroller = new Enroller(_captureDeviceEngine, _serviceManager);

      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();

      _database.PhotoHolder.DataChanged += RefreshData;      
    }  

    #region Update
    public void Update(Person user)
    {
      if (user == null)
        return;

      if (user.Dbstate == DbState.Insert)
        return;

      IsEnabled = true;
      _user = user;

      RefreshData();
    }

    #endregion

    #region Database
    private void RefreshData()
    {
      IList<Photo> list = _database.PhotoHolderByPerson.GetPersonPhoto(_user.Id);

      if (list == null)
        return;

      UserImages.Clear();

      /*
      foreach (Photo personPhoto in list)
      {
        UserImages.Add(personPhoto);
      }
       */
    }

    private void RefreshData(IList<Photo> list, Result result)
    {
      if (list == null)
        return;     

      /*
      foreach (Photo personPhoto in list)
      {
        UserImages.Add(personPhoto);
      }      
       * */
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
      _imageViewer.Clear();

      CaptureDevicesNames.CollectionChanged -= CaptureDevicesNames_CollectionChanged;

      base.OnDeactivate(close);
    }
    public void EnrollFromCamera()
    {
      if (CaptureDeviceConnected)
      {
        _serviceManager.FaceService.EnrollFeedbackChanged += FaceService_EnrollFeedbackChanged;
        EnrollmentData data = new EnrollmentData();
        _enroller.Start(ActiveCaptureDevice, data);
      }
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
          PhotoList photoList = new PhotoList();
          feedbackPhoto.Dbstate = DbState.Insert;
          feedbackPhoto.Description = NewPhoto.Description;
          feedbackPhoto.Personid = _user.Id;
          feedbackPhoto.Type = PhotoSizeType.Full;


          photoList.Photos.Add(feedbackPhoto);

          _database.PhotoHolder.DataUpdated += PhotoHolder_DataUpdated;

          await _serviceManager.DatabaseService.PhotoUpdateRequest(photoList);
        }

      }
      if (_imageViewer != null)
        _imageViewer.ShowProgress(feedback.Progress, feedback.Success);


      Console.WriteLine(feedback);
    }
    private void PhotoHolder_DataUpdated(IList<Photo> list, Result result)
    {
      _database.PhotoHolder.DataUpdated -= PhotoHolder_DataUpdated;

      Photo photo = null;      
      foreach (ResultPair currentResult in result.Status)
      {
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
          {
            photo = currentResult.Photo;

            if (photo != null)
            {
              string savePath = _bioEngine.Database().LocalStorage.LocalStoragePath + "\\" + photo.FileLocation;
              Directory.CreateDirectory(Path.GetDirectoryName(savePath));

              byte[] data = NewPhoto.Description.ToByteArray();
              var fs = new BinaryWriter(new FileStream(savePath, FileMode.CreateNew, FileAccess.Write));
              fs.Write(data);
              fs.Close(); 
            }               
          }
          else if (currentResult.State == DbState.Remove)
          {
            string folderPath = _bioEngine.Database().LocalStorage.LocalStoragePath + "\\";

            _imageViewer.UpdateImage(null, null);

            foreach(Photo deletedPhoto in list)
            {
              UserImages.Remove(deletedPhoto);
              
              File.Delete(folderPath + deletedPhoto.FileLocation);
              try
              {
                System.IO.File.Delete(folderPath + deletedPhoto.FileLocation);
              }
              catch (System.IO.IOException e)
              {
                Console.WriteLine(e.Message);
                return;
              }
            }
            
            MessageBox.Show("Photo successfully Removed");
          }
          RefreshData();  
        }
      }
    }

    


    #endregion

    #region Interface
    public void Apply()
    {

    }
    public void EnrollFromPhoto()
    {
      UploadClick();
    }
    public void OnSelectionChange()
    {
      if (SelectedItem != null)
        _imageViewer.UpdateImage(SelectedItem, _database.LocalStorage.LocalStoragePath);
    }

    public void UploadClick()
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
      openFileDialog.InitialDirectory = Environment.CurrentDirectory;

      if (openFileDialog.ShowDialog() == true)
      {
        string filename = openFileDialog.FileName;
        if (File.Exists(filename))
        {
          Bitmap bmp = (Bitmap)Image.FromFile(filename);

          Google.Protobuf.ByteString description = _bioUtils.ImageToByteString(bmp);

          NewPhoto = new Photo()
          {
            Dbstate = DbState.Insert
            , Description = description
            , FileLocation = ""
            , FirLocation = ""
            , Personid = _user.Id
            , Type = PhotoSizeType.Full
          };

          _imageViewer.UpdateImage(NewPhoto, filename);

          PhotoList photoList = new PhotoList();
          photoList.Photos.Add(NewPhoto);          
        }
      }
    }   
   
    public async void DeletePhoto()
    {
      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());

      if (result == true)
      {
        if (SelectedItem == null)
          return;

        SelectedItem.Dbstate = DbState.Remove;

        PhotoList photoList = new PhotoList();
        photoList.Photos.Add(SelectedItem);

        _database.PhotoHolder.DataUpdated += PhotoHolder_DataUpdated;

        await _serviceManager.DatabaseService.PhotoUpdateRequest(photoList);
      } 
    } 

    public async void Remove(bool all)
    {
      if(all)
      {
        PhotoList photoList = new PhotoList();

        foreach(Photo photo in UserImages)
        {
          photo.Dbstate = DbState.Remove;
          photoList.Photos.Add(photo);
        }

        _database.PhotoHolder.DataUpdated += PhotoHolder_DataUpdated;

        await _serviceManager.DatabaseService.PhotoUpdateRequest(photoList);
      }

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

/*
    private AsyncObservableCollection<Uri> _userImages;
    public AsyncObservableCollection<Uri> UserImages
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
    }*/

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
        }
      }
    }

    private Person _user;
    private Person User
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
    private bool CaptureDeviceConnected
    {
      get { return _captureDeviceConnected; }
      set
      {
        if (_captureDeviceConnected != value)
        {
          _captureDeviceConnected = value;
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
