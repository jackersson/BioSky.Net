using BioModule.Utils;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioService;
using BioContracts;
using System.Windows.Media.Imaging;
using BioModule.ResourcesLoader;

namespace BioModule.ViewModels
{
  public class UserIrisViewModel : Screen, IUserBioItemsController
  {

    public UserIrisViewModel(IUserBioItemsUpdatable imageViewer)
    {
      DisplayName = "Irises";

      _imageViewer = imageViewer;
      _imageViewer.UpdateBioItemsController(this);

      UserIrises = new AsyncObservableCollection<BitmapSource>();

      UserIrises.Add(ResourceLoader.IrisScanImageIconSource);
    }

    public void Update(Person user)
    {
      if (user == null || (user != null && user.Id <= 0))
        return;

      _user = user;
    }
    protected override void OnActivate()
    {
      base.OnActivate();

      _imageViewer.SetBioImageModel(BioImageModelType);
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
    }

    public void OnMouseRightButtonDown(BitmapSource photo)
    {

    }

    public bool CanNext
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public bool CanPrevious
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public BioImageModelType BioImageModelType { get { return BioImageModelType.Irises; }}

    public Person User { get{ return _user; }}

    public void Add(Photo photo)
    {
      throw new NotImplementedException();
    }

    public void Next()
    {
      throw new NotImplementedException();
    }

    public void Previous()
    {
      throw new NotImplementedException();
    }

    public void Remove(Photo photo)
    {
      throw new NotImplementedException();
    }

    private AsyncObservableCollection<BitmapSource> _userIrises;
    public AsyncObservableCollection<BitmapSource> UserIrises
    {
      get { return _userIrises; }
      set
      {
        if (_userIrises != value)
        {
          _userIrises = value;
          NotifyOfPropertyChange(() => UserIrises);
        }
      }
    }

    private Person _user;
    private IUserBioItemsUpdatable _imageViewer;
  }
}
