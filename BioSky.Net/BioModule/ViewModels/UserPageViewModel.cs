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
     

        DisplayName = (_user.Firstname + " " + _user.Lastname);
      }
      else
      {
        _user = new Person()
        {
            Firstname = ""
          , Lastname = ""
          , Thumbnail = 0
          , Gender = BioFaceService.Person.Types.Gender.Male
          , Rights = BioFaceService.Person.Types.Rights.Operator
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
        PersonList personList = new PersonList();
        personList.Persons.Add(_user);

        _bioService.DatabaseService.PersonUpdated += DatabaseService_PersonUpdated;

        await _bioService.DatabaseService.PersonUpdateRequest(personList);
      }     
    }

    public async void Apply()
    {

      foreach (IUpdatable scrn in Items)
        scrn.Apply();

      await UserUpdatePerformer((_userPageMode == UserPageMode.NewUser) ? DbState.Insert : DbState.Update);
           
    }

   
    private Person PersonUpdateResultProcessing(PersonList list, Result result, Person personToUpdate)
    {
      /*
      _bioService.DatabaseService.PersonUpdated -= DatabaseService_PersonUpdated;

      IBioSkyNetRepository database = _locator.GetProcessor<IBioSkyNetRepository>();

      string message = "";

      foreach (ResultPair rp in result.Status)
      {
        Person person = null;
        if (rp.Status == ResultStatus.Success )
        {
          if (rp.State == DbState.Insert)
          {
            person = personToUpdate;
            person.Id = rp.Id;
          }
          else          
            person = list.Persons.Where(x => x.Id == rp.Id).FirstOrDefault();       

          database.UpdatePerson(person, rp.State);
     
        }
        else
        {
          if (rp.State == DbState.Insert)
            message += rp.Status.ToString() + " " + rp.State.ToString() + " " + personToUpdate.Firstname + " " + personToUpdate.Lastname + "\n";
        }

        if (person != null )
          message += rp.Status.ToString() + " " + rp.State.ToString() + " " + person.Firstname + " " + person.Lastname + "\n";

        personToUpdate = person;
      }

      MessageBox.Show(message);
      */
      return personToUpdate;
    }

    private void DatabaseService_PersonUpdated( PersonList list, Result result)
    {
      Person person = PersonUpdateResultProcessing(list, result, _user);
      Update(person);      
    }
   
    private Person _user;

    private readonly FastMethodInvoker _methodInvoker;

    private readonly IProcessorLocator _locator;

    private IWindowManager _windowManager;

    private UserPageMode  _userPageMode ;

    private readonly IServiceManager _bioService ;
    
  }
}
