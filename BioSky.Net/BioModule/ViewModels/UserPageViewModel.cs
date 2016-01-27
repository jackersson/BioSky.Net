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
//using static BioFaceService.Person.Types;

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

      CurrentImageView = new ImageViewModel();

      Items.Add(new UserInformationViewModel    ());
      Items.Add(new UserContactlessCardViewModel(bioEngine, _locator));
      Items.Add(new UserPhotoViewModel          (bioEngine, CurrentImageView));
     
      ActiveItem = Items[0];
      OpenTab();

      _methodInvoker = new FastMethodInvoker();

      DisplayName = "Add New User";
    }

    public void Update(Person user)
    {
      if (user != null)
      {
        _user = user;
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
     

    public async void Apply()
    {
      //CommandPerson cmd = new CommandPerson();
     // await _locator.GetProcessor<IServiceManager>().DatabaseService.PersonRequest(cmd);

    //  var result = _windowManager.ShowDialog(new YesNoDialogViewModel());
      //Console.WriteLine(result);
      //var result = _windowManager.ShowDialog(new AboutDialogViewModel());
      //var result = _windowManager.ShowDialog(new LoginDialogViewModel());
      
/*
      if (result == true)
      {
        if (_userPageMode == UserPageMode.NewUser)
          _bioEngine.Database().AddUser(_user);
        else
          _bioEngine.Database().UpdateUser(_user);

        MessageBox.Show(_userPageMode == UserPageMode.NewUser
                        ? "User Successfully Added"
                        : "Successfully updated");

        foreach (IScreen scrn in Items)
        {
          MethodInfo method = scrn.GetType().GetMethod("Apply");
          if (method != null)
            method.Invoke(scrn, null);
        } 
      } */    
    }

   
    private Person _user;

    private readonly FastMethodInvoker _methodInvoker;

    private readonly IProcessorLocator _locator;

    private IWindowManager _windowManager;

    private UserPageMode  _userPageMode ; 
    
  }
}
