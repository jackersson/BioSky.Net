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
using BioFaceService;
using System.IO;

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

      CurrentImageView = new ImageViewModel();
      _bioUtils = new BioContracts.Common.BioImageUtils();

      Items.Add(new UserInformationViewModel    ());
      Items.Add(new UserContactlessCardViewModel(bioEngine, _locator));
      Items.Add(new UserPhotoViewModel          (bioEngine, CurrentImageView, _locator));
     
      ActiveItem = Items[0];
      OpenTab();
      
      _methodInvoker = new FastMethodInvoker();     

      DisplayName = "Add New User";
    }

    #region Update

    public void Update(Person user)
    {
      if (user != null)
      {
        _user = user.Clone();
        _userPageMode = UserPageMode.ExistingUser;
        
        Photo photo = null;
        bool photoExists = _database.PhotoHolder.DataSet.TryGetValue(_user.Thumbnail, out photo);

        if (photoExists)
        {
          string photoLocation = _database.LocalStorage.LocalStoragePath + "\\" + photo.FileLocation;

          if (File.Exists(photoLocation))
          {
            Uri uri = new Uri(photoLocation);
            CurrentImageView.UpdateImage(uri);
          }
        }

        DisplayName = (_user.Firstname + " " + _user.Lastname);
      }
      else
      {
        _user = new Person()
        {
          Firstname = ""
          ,
          Lastname = ""
          ,
          Thumbnail = 0
          ,
          Gender = Person.Types.Gender.Male
          ,
          Rights = Person.Types.Rights.Operator
          ,
          Dbstate = DbState.Insert
        };

        _userPageMode = UserPageMode.NewUser;
        DisplayName = "Add New User";
      }

      CurrentImageView.Update(_user);

      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { _user });
    }      


    #endregion
    
    #region Database




    #endregion

    #region BioService

    public async Task UserUpdatePerformer(DbState state)
    {
      _user.Dbstate = state;

      PersonList personList = new PersonList();
      personList.Persons.Add(_user);

      _database.PersonHolder.DataUpdated += PersonHolder_DataUpdated;

      await _bioService.DatabaseService.PersonUpdateRequest(personList);
    }

    //TODO show that person updated
    private void PersonHolder_DataUpdated(IList<Person> list, Result result)
    {
      _database.PersonHolder.DataUpdated -= PersonHolder_DataUpdated;

      foreach (Person person in list)
      {
        MessageBox.Show("User " + person.Firstname + " " + person.Lastname + "\n" + result + " Updated");
      }
    }

    public async Task GetCurrentImage()
    {
      if (CurrentImageView.CurrentImageUri != null)
      {
        IList<Photo> list = _database.PhotoHolderByPerson.GetPersonPhoto(_user.Id);

        string personFolder = _database.LocalStorage.LocalStoragePath;
        string currentImagePath = CurrentImageView.CurrentImageUri.OriginalString;

        foreach (Photo personPhoto in list)
        {
          if (personFolder + " " + personPhoto.FileLocation == currentImagePath)
          {
            _user.Thumbnail = personPhoto.Id;
          }
        }
      }
      else
      {
        if (CurrentImageView.CurrentImageBitmap != null)
        {
          PhotoList photoList = new PhotoList();
          Google.Protobuf.ByteString description = _bioUtils.ImageToByteString(CurrentImageView.CurrentImageBitmap);

          Photo photo = new Photo()
          {
            Dbstate = DbState.Insert
          ,
            Description = description
          ,
            FileLocation = ""
          ,
            FirLocation = ""
          ,
            Personid = _user.Id
          ,
            Type = Photo.Types.PhotoSizeType.Full
          };

          photoList.Photos.Add(photo);

          _database.PhotoHolder.DataUpdated += PhotoHolder_DataUpdated;

          await _bioService.DatabaseService.PhotoUpdateRequest(photoList);
        }
      }
    }

    private void PhotoHolder_DataUpdated(IList<Photo> list, Result result)
    {
      _database.PersonHolder.DataUpdated -= PersonHolder_DataUpdated;
      foreach (ResultPair currentResult in result.Status)
      {
        Photo photo = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
            photo = currentResult.Photo;
          if (photo != null)
          {
            string savePath = _database.LocalStorage.LocalStoragePath + "\\" + photo.FileLocation;
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            byte[] data = photo.Description.ToByteArray();
            var fs = new BinaryWriter(new FileStream(savePath, FileMode.CreateNew, FileAccess.Write));
            fs.Write(data);
            fs.Close();

            _user.Thumbnail = photo.Id;
          }
        }
      }
    }
    #endregion

    #region Interface

    public async void Apply()
    {
      foreach (IUpdatable updatableScreen in Items)
        updatableScreen.Apply();

      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());

      if (result == true)     
      {
        await UserUpdatePerformer((_userPageMode == UserPageMode.NewUser) ? DbState.Insert : DbState.Update);
        await GetCurrentImage();
        await UserUpdatePerformer(DbState.Update);
      }
    }


    public async void Remove()
    {
      await UserUpdatePerformer(DbState.Remove);
    }

    #endregion

    #region UI
    public void OpenTab()
    {
      ActiveItem.Activate();
    }

    private ImageViewModel _currentImageView;
    public ImageViewModel CurrentImageView
    {
      get { return _currentImageView; }
      private set
      {
        if (_currentImageView != value)
        {
          _currentImageView = value;
          NotifyOfPropertyChange(() => CurrentImageView);
        }
      }
    }

    #endregion

    #region Global Variables

    private Person _user;

    private BioContracts.Common.BioImageUtils _bioUtils;

    private readonly FastMethodInvoker _methodInvoker;

    private readonly IProcessorLocator _locator;

    private IWindowManager _windowManager;

    private UserPageMode _userPageMode;

    private IBioSkyNetRepository _database;

    private readonly IServiceManager _bioService;

    #endregion
  }
}
