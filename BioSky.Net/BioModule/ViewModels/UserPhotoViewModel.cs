﻿using System;
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

using BioFaceService;
using BioContracts.Services;
using Microsoft.Win32;
using BioContracts.Common;

namespace BioModule.ViewModels
{

 

  public class UserPhotoViewModel : Screen, IUpdatable
  {
    public UserPhotoViewModel(IBioEngine bioEngine, IImageUpdatable imageViewer, IProcessorLocator locator)
    {
      _locator             = locator;
      _bioEngine           = bioEngine;
      _imageViewer         = imageViewer;
      _captureDeviceEngine = locator.GetProcessor<ICaptureDeviceEngine>();
      
      DisplayName = "Photo";

      _serviceManager = locator.GetProcessor<IServiceManager>();

      UserImages = new ObservableCollection<Uri>();

      _enroller = new Enroller(_captureDeviceEngine, _serviceManager);

      CaptureDevicesNames = _bioEngine.CaptureDeviceEngine().GetCaptureDevicesNames();    
    }

    public void Update(Person user)
    {
      if (user == null)
        return;

      _user = user;


      string personFolder = _bioEngine.Database().LocalStorage.PersonsStoragePath + "\\" + _user.Id;
      Directory.CreateDirectory(personFolder);

      UserImages.Clear();
      DirectoryInfo personImageDir = new DirectoryInfo(personFolder);
      foreach (FileInfo personImageFile in personImageDir.GetFiles("*.jpg"))
      {
        Uri uri = new Uri(personImageFile.FullName);
        UserImages.Add(uri);
      }
      
    }

    protected override void OnActivate()
    {
      CaptureDeviceConnected = false;
   
      CaptureDevicesNames.CollectionChanged += CaptureDevicesNames_CollectionChanged;
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

    public void OnSelectionChange()
    {
      if (SelectedItem != null)      
        _imageViewer.UpdateImage(SelectedItem);       
    }

    private void OnNewFrame(object sender, ref Bitmap bitmap)
    {
      if (bitmap == null)
        return;

      CaptureDeviceConnected = true;

      _imageViewer.UpdateImage(ref bitmap);     
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

    public void EnrollFromCamera()
    {
     
      if (CaptureDeviceConnected)
      {
        _serviceManager.FaceService.EnrollFeedbackChanged += FaceService_EnrollFeedbackChanged;
        EnrollmentData data = new EnrollmentData();
        _enroller.Start(ActiveCaptureDevice, data);
      }       
    }

    private void FaceService_EnrollFeedbackChanged(object sender, EnrollmentFeedback feedback)
    {
      if (feedback.Progress == 100)
      {
        _serviceManager.FaceService.EnrollFeedbackChanged -= FaceService_EnrollFeedbackChanged;
        BioImage image = _enroller.GetImage();
        Photo feedbackPhoto = feedback.Photo;
        

        if ( image != null && feedbackPhoto != null )
        {
          PhotoList photoList = new PhotoList();          
          Photo photo = new Photo() { Dbstate      = DbState.Insert
                                    , Description  = image.Description
                                    , FileLocation = feedbackPhoto.FileLocation
                                    , FirLocation  = feedbackPhoto.FirLocation
                                    , Personid     = _user.Id
                                    , Type = Photo.Types.PhotoSizeType.Full
                                    };

          photoList.Photos.Add(photo);


          string savePath = _bioEngine.Database().LocalStorage.LocalStoragePath +  "\\" + feedbackPhoto.FileLocation;
          Directory.CreateDirectory(Path.GetDirectoryName(savePath));
          

          byte[] data = image.Description.ToByteArray();
          var fs = new BinaryWriter(new FileStream(savePath, FileMode.CreateNew, FileAccess.Write));
          fs.Write(data);
          fs.Close();

          _serviceManager.DatabaseService.PhotoUpdateRequest(photoList);
        }        
       
      }
      if (_imageViewer != null)
        _imageViewer.ShowProgress(feedback.Progress, feedback.Success);
      

      Console.WriteLine(feedback);
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
          //_enroller.Start(bmp);
        }        
      }
    }

    public void EnrollFromPhoto()
    {
      UploadClick();
    }

    public void DeletePhoto()
    {
      Console.Write("Delete Photos");
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

    private ObservableCollection<Uri> _userImages;
    public ObservableCollection<Uri> UserImages
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

    private Uri _selectedItem;
    public Uri SelectedItem
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

    private void CaptureDevicesNames_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      NotifyOfPropertyChange(() => CaptureDevicesNames);
      if(ActiveCaptureDevice == null)
        NotifyOfPropertyChange(() => AvaliableDevicesCount);
    }

    public void Apply()
    {
     
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

          NotifyOfPropertyChange(() => CaptureDevicesNames  );          
        }
      }
    }

    public string AvaliableDevicesCount
    {
      get { return String.Format("Available Devices ({0})", _captureDevicesNames.Count); }
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

    public BitmapSource CaptureDeviceConnectedIcon
    {
      get { return CaptureDeviceConnected ? ResourceLoader.OkIconSource : ResourceLoader.ErrorIconSource; }
    }


    private readonly Enroller             _enroller           ;
    private readonly IProcessorLocator    _locator            ;
    private readonly IBioEngine           _bioEngine          ;
    private readonly ICaptureDeviceEngine _captureDeviceEngine;
    private readonly IImageUpdatable      _imageViewer        ;
    private readonly IServiceManager      _serviceManager     ;
  }
}
