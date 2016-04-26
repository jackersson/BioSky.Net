using System;
using BioModule.Utils;
using BioService;
using Caliburn.Micro;
using BioModule.BioModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Generic;

namespace BioModule.ViewModels
{

  public delegate void FingerChangedEventHandler(Finger finger);
  public interface IFingerSelector
  {
    Finger SelectedFinger { get; set; }

    event FingerChangedEventHandler FingerChanged; 
  }
  public class UserFingerViewModel : Screen, IUserBioItemsController, IFingerSelector
  {
    public UserFingerViewModel(IUserBioItemsUpdatable imageViewer)
    {
      DisplayName = "Fingers";

      _imageViewer = imageViewer;
      _imageViewer.UpdateBioItemsController(this);

      CreateFingers();
    }

    private void CreateFingers()
    {
      FingerButtonsDictionary.Add(Finger.LeftLittle, new Thickness(19 , 39 , 0, 0));
      FingerButtonsDictionary.Add(Finger.LeftRing  , new Thickness(65 , 10 , 0, 0));
      FingerButtonsDictionary.Add(Finger.LeftMiddle, new Thickness(110, 0  , 0, 0));
      FingerButtonsDictionary.Add(Finger.LeftIndex , new Thickness(154, 16 , 0, 0));
      FingerButtonsDictionary.Add(Finger.Any       , new Thickness(219, 123, 0, 0));

      FingerButtonsDictionary.Add(Finger.RightThumb , new Thickness(275, 123, 0, 0));
      FingerButtonsDictionary.Add(Finger.RightIndex , new Thickness(331, 16 , 0, 0));
      FingerButtonsDictionary.Add(Finger.RightMiddle, new Thickness(380, 0  , 0, 0));
      FingerButtonsDictionary.Add(Finger.RightRing  , new Thickness(425, 10 , 0, 0));
      FingerButtonsDictionary.Add(Finger.RightLittle, new Thickness(468, 39 , 0, 0));
      NotifyOfPropertyChange(() => FingerButtonsDictionary);
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


    #region UI

    private Finger _selectedFinger;
    public Finger SelectedFinger
    {
      get { return _selectedFinger; }
      set
      {
        if (_selectedFinger != value)
        {
          _selectedFinger = value;
          OnFingerChanged();
          NotifyOfPropertyChange(() => SelectedFinger);
        }
      }
    }

    private Dictionary<Finger, Thickness> _fingerButtonsDictionary;
    public Dictionary<Finger, Thickness> FingerButtonsDictionary
    {
      get
      {
        if (_fingerButtonsDictionary == null)
          _fingerButtonsDictionary = new Dictionary<Finger, Thickness>();
        return _fingerButtonsDictionary; }
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

    private void OnFingerChanged()
    {
      if (FingerChanged != null)
        FingerChanged(_selectedFinger);
    }
    public event FingerChangedEventHandler FingerChanged;

    #endregion

    public BioImageModelEnum PageEnum { get { return BioImageModelEnum.Fingers; }}
    public Person User { get { return _user; }}

    private Person                            _user            ;
    private IUserBioItemsUpdatable            _imageViewer     ;
  }  
}
