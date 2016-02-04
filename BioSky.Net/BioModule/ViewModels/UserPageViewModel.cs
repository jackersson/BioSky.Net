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
      _bioService = _locator.GetProcessor<IServiceManager>();
      _database   = _locator.GetProcessor<IBioSkyNetRepository>();

      CurrentImageView = new ImageViewModel();

      Items.Add(new UserInformationViewModel    ());
      Items.Add(new UserContactlessCardViewModel(bioEngine, _locator));
      Items.Add(new UserPhotoViewModel          (bioEngine, CurrentImageView, _locator));
     
      ActiveItem = Items[0];
      OpenTab();
      
      _methodInvoker = new FastMethodInvoker();

      

      DisplayName = "Add New User";
    }

   

    public void Update(Person user)
    {
      if (user != null)
      {
        _user = user.Clone();
        _userPageMode = UserPageMode.ExistingUser;


        string personFolder = "D:\\";
        Photo photo = null;
        bool photoExists = _database.PhotoHolder.DataSet.TryGetValue(_user.Thumbnail, out photo);

        if ( photoExists )
        {
          if (File.Exists(personFolder + "\\" + photo.FileLocation))
          {
            Uri uri = new Uri(personFolder + "\\" + photo.FileLocation);
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
          , Lastname = ""
          , Thumbnail = 0       
          , Gender = Person.Types.Gender.Male
          , Rights = Person.Types.Rights.Operator
          , Dbstate = DbState.Insert
        };

        _userPageMode = UserPageMode.NewUser;
        DisplayName = "Add New User";
      }      

      CurrentImageView.Update( _user);
      
      foreach (IScreen scrn in Items)
        _methodInvoker.InvokeMethod(scrn.GetType(), "Update", scrn, new object[] { _user });     
    }      

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

    public async void Remove()
    {
      await UserUpdatePerformer(DbState.Remove);    
    }

    public async Task UserUpdatePerformer( DbState state )
    {
      var result = _windowManager.ShowDialog(new YesNoDialogViewModel());
      if (result == true)
      {

        _user.Dbstate = state;

        if (CurrentImageView.CurrentImageUri != null)
        {
          IList<Photo> list = _database.PhotoHolderByPerson.GetPersonPhoto(_user.Id);

          string personFolder = "D:\\";

          foreach (Photo personPhoto in list)
          {
            Uri uri = new Uri(personFolder + "\\" + personPhoto.FileLocation);
            if (uri.OriginalString == CurrentImageView.CurrentImageUri.OriginalString)
            {
              _user.Thumbnail = personPhoto.Id;
            }
          }     

        }

        PersonList personList = new PersonList();
        personList.Persons.Add(_user);

        _database.PersonHolder.DataUpdated += PersonHolder_DataUpdated;

        await _bioService.DatabaseService.PersonUpdateRequest(personList);
      }     
    }

    //TODO show that person updated
    private void PersonHolder_DataUpdated(IList<Person> list, Result result)
    {      
      _database.PersonHolder.DataUpdated -= PersonHolder_DataUpdated;
      
      foreach(Person person in list)
      {
        MessageBox.Show("User " + person.Firstname + " " + person.Lastname + "\n" + result + " Updated");
      }
    }

    public async void Apply()
    {
      foreach (IUpdatable updatableScreen in Items)
        updatableScreen.Apply();

      await UserUpdatePerformer((_userPageMode == UserPageMode.NewUser) ? DbState.Insert : DbState.Update);           
    }

    private Person _user;

    private readonly FastMethodInvoker _methodInvoker;

    private readonly IProcessorLocator _locator;

    private IWindowManager _windowManager;

    private UserPageMode  _userPageMode ;

    private IBioSkyNetRepository _database;

    private readonly IServiceManager _bioService ;
    
  }
}
