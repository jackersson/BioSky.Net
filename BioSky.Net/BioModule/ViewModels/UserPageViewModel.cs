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
      Items.Add(new UserPhotoViewModel          (bioEngine, CurrentImageView, _locator, _windowManager));
     
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
          CurrentImageView.UpdateImage(photo, _database.LocalStorage.LocalStoragePath);

        DisplayName = (_user.Firstname + " " + _user.Lastname);
      }
      else
      {
        _user = new Person()
        {
            Firstname = ""
          , Lastname = ""
          , Thumbnail = 0
          , Gender = Person.Types.Gender.Male
          , Rights = Person.Types.Rights.Operator
          , Dbstate = DbState.Insert
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
    
    private void PersonHolder_DataUpdated(IList<Person> list, Result result)
    {
      _database.PersonHolder.DataUpdated -= PersonHolder_DataUpdated;

      Person person = null;      
      foreach (ResultPair currentResult in result.Status)
      {
        if (currentResult.Status == ResultStatus.Success)
        {
          if (currentResult.State == DbState.Insert)
            person = currentResult.Person;
          else if (currentResult.State == DbState.Update)          
            person = _user;    
          else if (currentResult.State == DbState.Remove)
          {
            Update(null);
            foreach (Person personDeleted in list)
            {
              MessageBox.Show("User " + personDeleted.Firstname + " " + personDeleted.Lastname + "\n" + "Successfully Removed");
            }
            return;
          }
        }        
      }

      Update(person);
      ResolveConnections();
    }

    public async void ResolveConnections()
    {
      if (_user.Id <= 0)
        return;

      if (CurrentImageView.CurrentImagePhoto == null)
        return;

      if (CurrentImageView.CurrentImagePhoto.Id <= 0)
        return;

      if ( _user.Thumbnail != CurrentImageView.CurrentImagePhoto.Id )
      {
        _user.Thumbnail = CurrentImageView.CurrentImagePhoto.Id;
        _user.Dbstate = DbState.Update;

        await UserUpdatePerformer((_userPageMode == UserPageMode.NewUser) ? DbState.Insert : DbState.Update);       
      }
      else  if ( CurrentImageView.CurrentImagePhoto.Personid != _user.Id )
      {
        CurrentImageView.CurrentImagePhoto.Personid = _user.Id;
        CurrentImageView.CurrentImagePhoto.Dbstate = DbState.Update;

        await PhotoUpdatePerformer();
      }

      if(_user.Thumbnail == CurrentImageView.CurrentImagePhoto.Id && CurrentImageView.CurrentImagePhoto.Personid == _user.Id)
        MessageBox.Show("User " + _user.Firstname + " " + _user.Lastname + "\n" + "Successfully Updated");     
    }

    public async Task PhotoUpdatePerformer()
    {
      Photo photo = CurrentImageView.CurrentImagePhoto;

      photo.Origin = PhotoOriginType.Loaded;

      PhotoList photoList = new PhotoList();
      photoList.Photos.Add(photo);

      _database.PhotoHolder.DataUpdated += PhotoHolder_DataUpdated;     

      await _bioService.DatabaseService.PhotoUpdateRequest(photoList);
    }

    private void PhotoHolder_DataUpdated(IList<Photo> list, Result result)
    {
      _database.PhotoHolder.DataUpdated -= PhotoHolder_DataUpdated;

      foreach (ResultPair currentResult in result.Status)
      {
        Photo photo = null;
        if (currentResult.Status == ResultStatus.Success)
        {
          photo = currentResult.Photo;
          if (currentResult.State == DbState.Insert)
          {
            if (photo != null)
            {
              string path = _database.LocalStorage.LocalStoragePath + "\\" + photo.FileLocation;
              CurrentImageView.SavePhoto(path);
              CurrentImageView.UpdateImage(photo, _database.LocalStorage.LocalStoragePath);
            }  
          }
          else if(currentResult.State == DbState.Update)
          {
            string foledrPath = _database.LocalStorage.LocalStoragePath + "\\";
            foreach(Photo photoUpdated in list)
            {
              if (photo.Id == photoUpdated.Id)
                if (photoUpdated.FileLocation != photo.FileLocation)
                {
                  try
                  {
                    if (File.Exists(foledrPath + photo.FileLocation))
                      File.Delete(foledrPath + photo.FileLocation);

                    CurrentImageView.UpdateImage(null, null);                    
                    CurrentImageView.MovePhoto(foledrPath + photoUpdated.FileLocation, foledrPath + photo.FileLocation);
                    CurrentImageView.UpdateImage(photo, foledrPath + photo.FileLocation);
                  }
                  catch (System.IO.IOException e)
                  {
                    Console.WriteLine(e.Message);
                    return;
                  }

                }
            }
          }                
        }
      }

      

      ResolveConnections();
    }   
    #endregion

    #region Interface

    public async void Apply()
    {
      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());

      if (result == true)     
      {
        foreach (IUpdatable updatableScreen in Items)
          updatableScreen.Apply();

        await UserUpdatePerformer((_userPageMode == UserPageMode.NewUser) ? DbState.Insert : DbState.Update);
        await PhotoUpdatePerformer();
      }
    }


    public async void Remove()
    {
      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());

      if (result == true)
      {
        foreach (IUpdatable updatableScreen in Items)
          updatableScreen.Remove(true);

        await UserUpdatePerformer(DbState.Remove);
      }
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
