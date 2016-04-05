using System;
using BioModule.Utils;
using BioService;
using Caliburn.Micro;

namespace BioModule.ViewModels
{
  public class UserFingerViewModel : Screen, IUserBioItemsController
  {
    public UserFingerViewModel(IUserBioItemsUpdatable imageViewer)
    {
      DisplayName = "Fingers";

      _imageViewer = imageViewer;

      _imageViewer.UpdateBioItemsController(this);
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

      _imageViewer.ChangeBioImageModel(PageEnum);
    }

    protected override void OnDeactivate(bool close)
    {
      base.OnDeactivate(close);
    }

    public void OnFingerCheck(int finger)
    {
      Console.WriteLine(finger);
    }

    public bool IsFingerExist
    {
      get { return true; }
    }

    public void Add(Photo photo)
    {
      throw new NotImplementedException();
    }

    public void Remove(Photo photo)
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

    public PhotoViewEnum PageEnum { get { return PhotoViewEnum.Fingers; }}
    public Person User { get { return _user; }}

    private Person                 _user       ;
    private IUserBioItemsUpdatable _imageViewer;
  }
}
